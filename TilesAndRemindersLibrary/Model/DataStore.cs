using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersLibrary.Helpers;
using TilesAndRemindersLibrary.Model.DataItems;
using TilesAndRemindersLibrary.Workers;
using Windows.Storage;
using System.Reflection;

namespace TilesAndRemindersLibrary.Model
{
    public class AdditionalDataTasks
    {
        public Task ResetPrimaryTileTask { get; set; }

        public async Task AwaitAll()
        {
            var properties = typeof(AdditionalDataTasks).GetTypeInfo().DeclaredProperties.Where(i => i.CanRead && i.PropertyType == typeof(Task));

            foreach (var p in properties)
            {
                Task t = p.GetValue(this) as Task;

                if (t != null)
                    await t;
            }
        }
    }

    public static class DataStore
    {
        public static event EventHandler<DataItemTask> OnTaskAdded;
        public static event EventHandler<DataItemTask> OnTaskUpdated;
        public static event EventHandler<int> OnTaskRemoved;

        public static Task<AdditionalDataTasks> AddTaskAsync(DataItemTask task)
        {
            return Execute(delegate
            {
                _conn.Insert(task);

                if (OnTaskAdded != null)
                    OnTaskAdded(null, task);

                var additional = new AdditionalDataTasks();

                ToastHelper.HandleOnTaskAdded(task);
                additional.ResetPrimaryTileTask = ResetPrimaryTileWorker.Instance.Start();

                return additional;
            });
        }

        public static Task<AdditionalDataTasks> SetTaskCompletionStatusAsync(int taskId, bool isComplete)
        {
            return Execute(delegate
            {
                DataItemTask task = _conn.Find<DataItemTask>(taskId);

                if (task == null || task.IsComplete == isComplete)
                    return new AdditionalDataTasks();

                task.IsComplete = isComplete;

                return UpdateTaskHelper(task);
            });
        }

        public static Task<AdditionalDataTasks> UpdateTaskAsync(DataItemTask task)
        {
            return Execute(delegate
            {
                return UpdateTaskHelper(task);
            });
        }

        /// <summary>
        /// Caller must have already established lock and is on background thread
        /// </summary>
        /// <param name="task"></param>
        private static AdditionalDataTasks UpdateTaskHelper(DataItemTask task)
        {
            _conn.InsertOrReplace(task);

            if (OnTaskUpdated != null)
                OnTaskUpdated(null, task);

            ToastHelper.HandleOnTaskUpdated(task);

            var additional = new AdditionalDataTasks();

            additional.ResetPrimaryTileTask = ResetPrimaryTileWorker.Instance.Start();

            return additional;
        }

        public static Task<AdditionalDataTasks> RemoveTaskAsync(int taskId)
        {
            return Execute(delegate
            {
                _conn.Delete<DataItemTask>(taskId);

                if (OnTaskRemoved != null)
                    OnTaskRemoved(null, taskId);

                ToastHelper.HandleOnTaskRemoved(taskId);

                return new AdditionalDataTasks()
                {
                    ResetPrimaryTileTask = ResetPrimaryTileWorker.Instance.Start()
                };
            });
        }

        /// <summary>
        /// Returns an array of tasks that are either (1) incomplete or (2) complete but still due in the future.
        /// </summary>
        /// <returns></returns>
        public static Task<DataItemTask[]> GetCurrentTasksAsync()
        {
            return Execute(delegate
            {
                return _conn.Table<DataItemTask>().Where(IsTaskCurrentFunction).ToArray();
            });
        }

        public static readonly Func<DataItemTask, bool> IsTaskCurrentFunction = (task => !task.IsComplete || task.StartTime >= DateTime.Now);

        /// <summary>
        /// Returns an array of tasks that are incomplete.
        /// </summary>
        /// <returns></returns>
        public static Task<DataItemTask[]> GetIncompleteTasksAsync()
        {
            return Execute(delegate
            {
                return _conn.Table<DataItemTask>().Where(IsTaskIncompleteFunction).ToArray();
            });
        }

        public static readonly Func<DataItemTask, bool> IsTaskIncompleteFunction = (task => !task.IsComplete);


        #region Execute

        /// <summary>
        /// Just a wrapper for the other Execute wrapper, which returns void rather than requiring a return type.
        /// </summary>
        private static Task Execute(Action action)
        {
            return Execute(delegate
            {
                action.Invoke();
                return true;
            });
        }

        /// <summary>
        /// Spins off a new thread, establishes a lock, initializes the connection/tables if not initialized, and then executes the action. This should be used whenever reading/updating the database, so that the database access doesn't block the UI thread.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private static Task<T> Execute<T>(Func<T> function)
        {
            return Task.Run(delegate
            {
                lock (_lock)
                {
                    // Initialize if not initialized yet
                    Initialize();

                    // And then run the desired action
                    return function.Invoke();
                }
            });
        }

        #endregion

        #region Initialization

        private static SQLiteConnection _conn;
        private static readonly object _lock = new object();

        /// <summary>
        /// Assumes thread/lock is already established. Initializes the connection and tables if not already initialized.
        /// </summary>
        private static void Initialize()
        {
            if (_conn != null)
                return;

            // Create the connection
            _conn = new SQLiteConnection(

                // Provide the platform (WinRT)
                sqlitePlatform: new SQLitePlatformWinRT(),

                // Provide the full path where you want the database stored
                databasePath: Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database.db")

                );

            // Create tables (if they already exist, this does nothing)
            _conn.CreateTable<DataItemTask>();
        }

        #endregion
    }
}
