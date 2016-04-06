using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace TilesAndRemindersApp.Helpers
{
    public static class BackgroundTasksHelper
    {
        public const string MyToastNotificationActionBackgroundTaskName = "MyToastNotificationActionBackgroundTask";

        public static async Task ConfigureBackgroundTasks()
        {
            BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            if (status == BackgroundAccessStatus.Denied || status == BackgroundAccessStatus.Unspecified)
                return;

            ConfigureToastBackgroundTask();
        }

        private static void ConfigureToastBackgroundTask()
        {
            var taskRegistration = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(i => i.Name.Equals(MyToastNotificationActionBackgroundTaskName));

            if (taskRegistration == null)
            {
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
                {
                    Name = MyToastNotificationActionBackgroundTaskName,
                    TaskEntryPoint = "TilesAndRemindersBackgroundProject.MyToastNotificationActionBackgroundTask"
                };

                builder.SetTrigger(new ToastNotificationActionTrigger());

                taskRegistration = builder.Register();
            }

            taskRegistration.Progress += TaskRegistration_Progress;
        }

        private static void TaskRegistration_Progress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            var coreViewModel = App.ViewModel;

            if (coreViewModel != null)
            {
                coreViewModel.ResetDataFromBackgroundTaskChange();
            }
        }
    }
}
