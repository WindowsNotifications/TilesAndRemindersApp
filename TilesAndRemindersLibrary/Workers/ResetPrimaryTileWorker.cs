using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TilesAndRemindersLibrary.Helpers;
using TilesAndRemindersLibrary.Model;
using TilesAndRemindersLibrary.Model.DataItems;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace TilesAndRemindersLibrary.Workers
{
    public class ResetPrimaryTileWorker : SingleWorker
    {
        public static readonly ResetPrimaryTileWorker Instance = new ResetPrimaryTileWorker();

        protected override Task Execute(CancellationToken cancellationToken)
        {
            return Task.Run(async delegate
            {
                TileUpdater updater = TileUpdateManager.CreateTileUpdaterForApplication();
                cancellationToken.ThrowIfCancellationRequested();

                TileHelper.ClearScheduledNotifications(updater);
                cancellationToken.ThrowIfCancellationRequested();

                var desiredTileNotifications = await GenerateDesiredTileNotifications(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (desiredTileNotifications.Count == 0)
                {
                    // Only call clear if we're not updating with anything else.
                    // Because there's a good chance the tile is actually already up-to-date, and the notification that we
                    // place there is just going to be a duplicate of the existing. If you called clear, then the UI would clear
                    // the tile and then animate to display the new (same) content. Skipping the call to clear, the UI notices that the
                    // new notification is the same and doesn't display it, leading to a better experience.
                    updater.Clear();
                }

                else
                {
                    foreach (var desired in desiredTileNotifications)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // If it's set to be displayed in the past, or now (or super soon), we'll update immediately, since scheduling something
                        // for a time that has already passed throws an exception
                        if (desired.DeliveryTime < DateTimeOffset.Now.AddSeconds(5))
                            updater.Update(new TileNotification(desired.Content));

                        else
                            updater.AddToSchedule(new ScheduledTileNotification(desired.Content, desired.DeliveryTime));
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<List<DesiredNotification>> GenerateDesiredTileNotifications(CancellationToken cancellationToken)
        {
            var tasks = await DataStore.GetIncompleteTasksAsync();
            cancellationToken.ThrowIfCancellationRequested();

            List<DesiredNotification> answer = new List<DesiredNotification>();

            // If there's no tasks, there's no tile notifications to display
            if (tasks.Length == 0)
                return answer;

            // Order the tasks by time
            tasks = tasks.OrderBy(i => i.StartTime).ToArray();
            cancellationToken.ThrowIfCancellationRequested();

            DateTimeOffset relativeNow = DateTimeOffset.Now;

            DateTimeOffset firstDueTime = tasks.First().StartTime;
            var normalOnDay = tasks.Where(t => t.StartTime.UtcDateTime.Date == firstDueTime.UtcDateTime.Date).ToArray();



            // Schedule up to 3 days in advance
            for (int i = 0; i < 3; i++, relativeNow = relativeNow.UtcDateTime.Date.AddDays(1))
            {
                // If in normal mode, display one notification that's an aggregate of all on that day
                if (firstDueTime >= relativeNow)
                {
                    answer.Add(CreateScheduledTileNotification(normalOnDay, relativeNow));
                }

                // If in overdue mode
                if (firstDueTime.UtcDateTime.Date <= relativeNow.UtcDateTime.Date)
                {
                    // Move relative now to the first due date
                    relativeNow = firstDueTime.AddTicks(1);

                    while (true)
                    {
                        var overdue = tasks.Where(t => t.StartTime < relativeNow).ToArray();

                        answer.Add(CreateScheduledTileNotification(overdue, relativeNow));

                        // Otherwise change now to the next task
                        var nextOverdueTask = tasks.FirstOrDefault(t => t.StartTime >= relativeNow);

                        // If there was no next task, we're done
                        if (nextOverdueTask == null)
                            break;

                        // Change the relative now time
                        relativeNow = nextOverdueTask.StartTime.AddTicks(1);

                        // Schedule up to 3 days in advance
                        if (relativeNow.UtcDateTime.Date > DateTime.Today.AddDays(3))
                            break;
                    }

                    break;
                }
            }

            return answer;
        }


        /// <summary>
        /// Uses yield return
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<DateTimeOffset> GetPointsInTimeWhereTileChanges(DataItemTask[] tasks)
        {
            DateTimeOffset now = DateTimeOffset.Now;

            foreach (var t in tasks)
            {
                if (t.StartTime.UtcDateTime.Date > now.UtcDateTime.Date)
                    yield return t.StartTime.UtcDateTime.Date;
            }
        }

        private class DesiredNotification
        {
            public XmlDocument Content { get; set; }
            public DateTimeOffset DeliveryTime { get; set; }
        }

        private static DesiredNotification CreateScheduledTileNotification(DataItemTask[] tasks, DateTimeOffset now)
        {
            XmlDocument content = CreateTileNotificationContent(tasks, now);

            return new DesiredNotification()
            {
                Content = content,
                DeliveryTime = now
            };
        }

        private static XmlDocument CreateTileNotificationContent(DataItemTask[] tasks, DateTimeOffset now)
        {
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = CreateTileNotificationMediumBinding(tasks, now)
                }
            };

            return content.GetXml();
        }

        private static TileBinding CreateTileNotificationMediumBinding(DataItemTask[] tasks, DateTimeOffset now)
        {
            var content = new TileBindingContentAdaptive()
            {
                Children =
                {
                    new TileGroup()
                    {
                        Children =
                        {
                            new TileSubgroup()
                            {
                                Weight = 22,
                                Children =
                                {
                                    new TileText()
                                    {
                                        Text = tasks.Length.ToString(),
                                        Style = TileTextStyle.SubheaderNumeral
                                    }
                                }
                            },

                            new TileSubgroup()
                            {
                                Weight = 78,
                                Children =
                                {
                                    new TileText()
                                    {
                                        Text = RelativeTimeFromNow(tasks[0].StartTime, now),
                                        Wrap = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            foreach (var t in tasks)
            {
                content.Children.Add(new TileText()
                {
                    Text = t.Title,
                    Style = TileTextStyle.CaptionSubtle
                });
            }

            return new TileBinding()
            {
                Content = content
            };
        }

        private static string RelativeTimeFromNow(DateTimeOffset time, DateTimeOffset now)
        {
            if (time < now)
                return "overdue";

            int days = (int)(time.UtcDateTime.Date - now.UtcDateTime.Date).TotalDays;

            if (days == 0)
                return "today";

            if (days == 1)
                return "tomorrow";
            
            return $"in {days}\ndays";
        }
    }
}
