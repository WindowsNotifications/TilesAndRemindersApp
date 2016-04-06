using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersApp.View;
using TilesAndRemindersLibrary.Model;
using TilesAndRemindersLibrary.Model.DataItems;

namespace TilesAndRemindersApp.ViewModel
{
    public class MainPageViewModel : BasePageViewModel
    {
        public MainPageViewModel(CoreViewModel coreViewModel) : base(coreViewModel)
        {
            Initialize();
        }

        private ObservableCollection<DataItemTask> _schedules;

        public ObservableCollection<DataItemTask> Schedules
        {
            get { return _schedules; }
            set { SetProperty(ref _schedules, value); }
        }

        private async void Initialize()
        {
            Schedules = await CoreViewModel.GetCurrentTasksAsync();
        }

        public override Type GetPageType()
        {
            return typeof(MainPage);
        }

        public async void SaveNewTask(string name, TimeSpan dueIn)
        {
            await DataStore.AddTaskAsync(new DataItemTask()
            {
                Title = name,
                StartTime = DateTimeOffset.Now.Add(dueIn)
            });
        }
    }
}
