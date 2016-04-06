using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilesAndRemindersApp.ViewModel
{
    public abstract class BaseScheduleEntryPageViewModel : BasePageViewModel
    {
        public BaseScheduleEntryPageViewModel(CoreViewModel coreViewModel, int scheduleEntryId) : base(coreViewModel)
        {
            ScheduleEntryId = scheduleEntryId;
        }

        public int ScheduleEntryId { get; private set; }
    }
}
