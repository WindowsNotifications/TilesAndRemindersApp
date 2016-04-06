using NotificationsExtensions.Toasts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersLibrary.Model;
using TilesAndRemindersLibrary.Model.DataItems;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace TilesAndRemindersLibrary.Helpers
{
    public class ToastHelper
    {
        public const string SELECTION_BOX_SNOOZE_TIME_ID = "snoozeTime";

        public static void HandleOnTaskAdded(DataItemTask task)
        {
            // If task is incomplete or alredy past the start time (we add padding to the Now time since scheduling a toast to appear exactly now doesn't work, it has to be in the future)
            if (task.IsComplete || task.StartTime < DateTimeOffset.Now.AddSeconds(1))
                return;

            ScheduleToast(task);
        }

        private static void ScheduleToast(DataItemTask task)
        {
            var toastId = GenerateToastTag(task.Id);

            ScheduledToastNotification notif = new ScheduledToastNotification(GenerateToastContent(task), task.StartTime)
            {
                Tag = toastId
            };

            ToastNotificationManager.CreateToastNotifier().AddToSchedule(notif);
        }

        public static void HandleOnTaskUpdated(DataItemTask task)
        {
            // First remove from schedule
            RemoveScheduledTaskToast(task.Id);

            // If it's complete, also remove the displayed, no need to reschedule or anything else
            if (task.IsComplete)
                RemoveDisplayedTaskToast(task.Id);

            // If we need to update the existing displayed toast
            else if (task.StartTime < DateTimeOffset.Now.AddSeconds(1))
            {
                ToastNotification notif = new ToastNotification(GenerateToastContent(task))
                {
                    Tag = GenerateToastTag(task.Id),

                    // Suppress the popup, since we're just silently updating
                    SuppressPopup = true
                };

                ToastNotificationManager.CreateToastNotifier().Show(notif);
            }

            // Otherwise we need to remove currently displayed and then re-schedule
            else
            {
                RemoveDisplayedTaskToast(task.Id);

                ScheduleToast(task);
            }
        }

        public static void HandleOnTaskRemoved(int taskId)
        {
            RemoveScheduledTaskToast(taskId);
            RemoveDisplayedTaskToast(taskId);
        }

        private static void RemoveDisplayedTaskToast(int taskId)
        {
            var toastTag = GenerateToastTag(taskId);

            ToastNotificationManager.History.Remove(toastTag);
        }

        private static void RemoveScheduledTaskToast(int taskId)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            var toastTag = GenerateToastTag(taskId);

            ScheduledToastNotification existing = notifier.GetScheduledToastNotifications().FirstOrDefault(i => i.Tag.Equals(toastTag));

            // If it exists, remove it
            if (existing != null)
                notifier.RemoveFromSchedule(existing);
        }

        private static string GenerateToastTag(int taskId)
        {
            return "Task" + taskId;
        }

        private static XmlDocument GenerateToastContent(DataItemTask task)
        {
            ToastContent content = new ToastContent()
            {
                Scenario = ToastScenario.Reminder,

                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = task.Title
                    },

                    BodyTextLine1 = new ToastText()
                    {
                        Text = "Due " + task.StartTime.LocalDateTime.ToString()
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastSelectionBox(SELECTION_BOX_SNOOZE_TIME_ID)
                        {
                            DefaultSelectionBoxItemId = "1",

                            Items =
                            {
                                new ToastSelectionBoxItem("1", "1 minute"),
                                new ToastSelectionBoxItem("2", "2 minutes"),
                                new ToastSelectionBoxItem("5", "5 minutes")
                            }
                        }
                    },

                    Buttons =
                    {
                        new ToastButtonSnooze()
                        {
                            SelectionBoxId = SELECTION_BOX_SNOOZE_TIME_ID
                        },

                        new ToastButton("Mark Complete", new MarkCompleteArguments()
                        {
                            TaskId = task.Id
                        }.ToString())
                        {
                            ActivationType = ToastActivationType.Background
                        }
                    }
                }
            };

            return content.GetXml();
        }
    }
}
