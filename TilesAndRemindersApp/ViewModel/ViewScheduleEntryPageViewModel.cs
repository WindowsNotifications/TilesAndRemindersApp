using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TilesAndRemindersApp.View;
using TilesAndRemindersLibrary.Model;
using TilesAndRemindersLibrary.Model.DataItems;

namespace TilesAndRemindersApp.ViewModel
{
    public class ViewScheduleEntryPageViewModel : BaseScheduleEntryPageViewModel
    {
        public ViewScheduleEntryPageViewModel(CoreViewModel coreViewModel, int scheduleEntryId) : base(coreViewModel, scheduleEntryId)
        {
            DeleteCommand = new BasicCommand(Delete, defaultCanExecute: false);

            Initialize(scheduleEntryId);
        }

        private async void Initialize(int scheduleEntryId)
        {
            var schedules = await CoreViewModel.GetCurrentTasksAsync();

            ScheduleEntry = schedules.First(i => i.Id == scheduleEntryId);

            DeleteCommand.SetCanExecute(true);
        }

        private DataItemTask _scheduleEntry;

        public DataItemTask ScheduleEntry
        {
            get { return _scheduleEntry; }
            set { SetProperty(ref _scheduleEntry, value); }
        }

        public BasicCommand DeleteCommand { get; private set; }

        private void Delete()
        {
            DataStore.RemoveTaskAsync(ScheduleEntry.Id);
        }

        public override Type GetPageType()
        {
            return typeof(ViewScheduleEntryPage);
        }
    }
}
