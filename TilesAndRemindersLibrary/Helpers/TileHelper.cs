using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace TilesAndRemindersLibrary.Helpers
{
    public class TileHelper
    {
        public static void ClearScheduledNotifications(TileUpdater updater)
        {
            foreach (var notif in updater.GetScheduledTileNotifications())
                updater.RemoveFromSchedule(notif);
        }
    }
}
