using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersLibrary.Helpers;
using TilesAndRemindersLibrary.Model;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace TilesAndRemindersBackgroundProject
{
    public sealed class MyToastNotificationActionBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            ToastNotificationActionTriggerDetail details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

            var args = ArgumentsHelper.ParseArguments(details.Argument);

            if (args is MarkCompleteArguments)
            {
                MarkCompleteArguments markCompleteArgs = args as MarkCompleteArguments;

                var additionalTasks = await DataStore.SetTaskCompletionStatusAsync(markCompleteArgs.TaskId, true);

                // Signal that changes to the data have been made before waiting for tiles and other tasks
                // The foreground app instance (if running) listens to progress change and then re-loads data
                taskInstance.Progress = 1;

                await additionalTasks.AwaitAll();
            }

            deferral.Complete();
        }
    }
}
