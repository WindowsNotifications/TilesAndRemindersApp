using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersApp.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TilesAndRemindersApp.View
{
    public abstract class BasePageWithViewModel<T> : Page where T : BasePageViewModel
    {


        public T ViewModel
        {
            get { return (T)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(T), typeof(BasePageWithViewModel<T>), new PropertyMetadata(null));



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = (T)App.ViewModel.GetCurrentPageViewModel();
        }
    }
}
