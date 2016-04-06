using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersLibrary;
using TilesAndRemindersLibrary.Helpers;
using TilesAndRemindersLibrary.Model;
using TilesAndRemindersLibrary.Model.DataItems;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TilesAndRemindersApp.ViewModel
{
    public class CoreViewModel : BindableBase
    {
        private List<BasePageViewModel> _pages = new List<BasePageViewModel>();
        private Frame _frame;
        CoreDispatcher _dispatcher;

        public CoreViewModel(Frame frame)
        {
            _frame = frame;
            _dispatcher = Window.Current.Dispatcher;

            DataStore.OnTaskAdded += DataStore_OnTaskAdded;
            DataStore.OnTaskUpdated += DataStore_OnTaskUpdated;
            DataStore.OnTaskRemoved += DataStore_OnTaskRemoved;
        }

        /// <summary>
        /// Use this method when data has changed in a background task and we need to reset the view data. Assumes caller is on background thread, this method will dispatch to UI.
        /// </summary>
        public void ResetDataFromBackgroundTaskChange()
        {
            var dontWait = ExecuteOnUI(async delegate
            {
                // If the tasks haven't been loaded yet, do nothing
                if (!_getCurrentTasksTask.IsCompleted)
                    return;

                var currTasks = await DataStore.GetCurrentTasksAsync();

                // Remove ones that no longer exist
                foreach (var inViewModel in _getCurrentTasksTask.Result.ToArray())
                {
                    if (!currTasks.Any(i => i.Id == inViewModel.Id))
                        _getCurrentTasksTask.Result.Remove(inViewModel);
                }

                // Add/update tasks
                foreach (var task in currTasks)
                {
                    var existing = _getCurrentTasksTask.Result.FirstOrDefault(i => i.Id == task.Id);

                    // If exists, update
                    if (existing != null)
                        existing.CopyFrom(task);

                    // Otherwise add new
                    else
                        AddTask(task);
                }

                RemoveAllTasksThatAreNoLongerCurrent();
            });
        }

        private void DataStore_OnTaskUpdated(object sender, DataItemTask updatedTask)
        {
            if (_getCurrentTasksTask.IsCompleted)
            {
                var dontWait = ExecuteOnUI(delegate
                {
                    var existing = _getCurrentTasksTask.Result.FirstOrDefault(i => i.Id == updatedTask.Id);

                    if (existing != null)
                    {
                        existing.CopyFrom(updatedTask);
                        RemoveAllTasksThatAreNoLongerCurrent();
                    }
                });
            }
        }

        private void DataStore_OnTaskRemoved(object sender, int taskId)
        {
            if (_getCurrentTasksTask.IsCompleted)
            {
                var dontWait = ExecuteOnUI(delegate
                {
                    _getCurrentTasksTask.Result.RemoveAll(i => i.Id == taskId);

                    while (true)
                    {
                        var curr = GetCurrentPageViewModel() as BaseScheduleEntryPageViewModel;

                        // Remove any pages that were for this item
                        if (curr != null && curr.ScheduleEntryId == taskId)
                            GoBack();
                        else
                            break;
                    }
                });
            }
        }

        private void DataStore_OnTaskAdded(object sender, DataItemTask task)
        {
            if (_getCurrentTasksTask.IsCompleted)
            {
                var dontWait = ExecuteOnUI(delegate
                {
                    AddTask(task);
                });
            }
        }

        /// <summary>
        /// Assumes already on UI thread
        /// </summary>
        /// <param name="task"></param>
        private void AddTask(DataItemTask task)
        {
            if (!_getCurrentTasksTask.IsCompleted)
                return;

            // Insert sorted
            int indexToInsertAt = IEnumerableExtensions.FindIndexForSortedInsert(_getCurrentTasksTask.Result, task, _currentTaskComparer);

            _getCurrentTasksTask.Result.Insert(indexToInsertAt, task);
        }

        private IAsyncAction ExecuteOnUI(DispatchedHandler agileCallback)
        {
            return _dispatcher.RunAsync(CoreDispatcherPriority.Normal, agileCallback);
        }

        /// <summary>
        /// Navigates to the first page
        /// </summary>
        public void Initialize()
        {
            // Push the default page to start
            Navigate(new MainPageViewModel(this));
        }

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }
            set { SetProperty(ref _canGoBack, value); }
        }

        private void UpdateCanGoBack()
        {
            CanGoBack = _pages.Count > 1;
        }


        public void GoBack()
        {
            if (!CanGoBack)
                throw new InvalidOperationException("Cannot go back, CanGoBack is false.");

            if (!_frame.CanGoBack)
                throw new InvalidOperationException("Frame cannot go back. ViewModel is in an invalid state, this means there was a programming error.");

            _pages.RemoveAt(_pages.Count - 1);
            _frame.GoBack();
            UpdateCanGoBack();
        }

        public void Navigate(BasePageViewModel page)
        {
            _pages.Add(page);

            _frame.Navigate(page.GetPageType());

            UpdateCanGoBack();
        }

        public BasePageViewModel GetCurrentPageViewModel()
        {
            return _pages.Last();
        }
        
        private Task<ObservableCollection<DataItemTask>> _getCurrentTasksTask;

        public Task<ObservableCollection<DataItemTask>> GetCurrentTasksAsync()
        {
            if (_getCurrentTasksTask == null)
                _getCurrentTasksTask = CreateCurrentGetTasksTask();

            return _getCurrentTasksTask;
        }

        private static readonly CurrentTaskComparer _currentTaskComparer = new CurrentTaskComparer();
        private class CurrentTaskComparer : IComparer<DataItemTask>
        {
            public int Compare(DataItemTask x, DataItemTask y)
            {
                return x.StartTime.CompareTo(y.StartTime);
            }
        }

        private async Task<ObservableCollection<DataItemTask>> CreateCurrentGetTasksTask()
        {
            var entries = (await DataStore.GetCurrentTasksAsync()).ToList();
            entries.Sort(_currentTaskComparer);

            ObservableCollection<DataItemTask> answer = new ObservableCollection<DataItemTask>(entries);

            DispatcherTimer cleanUpCompletedPastDueTasksTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            cleanUpCompletedPastDueTasksTimer.Tick += delegate
            {
                RemoveAllTasksThatAreNoLongerCurrent();
            };

            cleanUpCompletedPastDueTasksTimer.Start();

            // TODO: Sort

            return answer;
        }

        private void RemoveAllTasksThatAreNoLongerCurrent()
        {
            if (_getCurrentTasksTask.IsCompleted)
                _getCurrentTasksTask.Result.RemoveAll(i => !DataStore.IsTaskCurrentFunction(i));
        }

        public void ViewScheduleEntry(int scheduleEntryId)
        {
            Navigate(new ViewScheduleEntryPageViewModel(this, scheduleEntryId));
        }

        public void MarkComplete(DataItemTask task)
        {
            SetCompletionStatus(task, true);
        }

        public void MarkIncomplete(DataItemTask task)
        {
            SetCompletionStatus(task, false);
        }

        private void SetCompletionStatus(DataItemTask task, bool isComplete)
        {
            if (task.IsComplete == isComplete)
                return;

            task = task.CreateDeepCopy();
            task.IsComplete = isComplete;

            DataStore.UpdateTaskAsync(task);
        }
    }
}
