using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersLibrary;

namespace TilesAndRemindersApp.ViewModel
{
    public abstract class BasePageViewModel : BindableBase
    {
        public BasePageViewModel(CoreViewModel coreViewModel)
        {
            CoreViewModel = coreViewModel;
        }

        public CoreViewModel CoreViewModel { get; private set; }

        public abstract Type GetPageType();
    }
}
