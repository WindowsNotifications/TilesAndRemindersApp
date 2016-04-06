using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TilesAndRemindersApp.ViewModel;
using TilesAndRemindersLibrary.Model;
using TilesAndRemindersLibrary.Model.DataItems;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TilesAndRemindersApp.View
{
    /// <summary>
    /// Since UWP doesn't support x:TypeArguments, we can't have generics in XAML and need this extra layer of indirection.
    /// </summary>
    public abstract class MainPageBase : BasePageWithViewModel<MainPageViewModel> { }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : MainPageBase
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void TextBoxAddNew_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ViewModel.SaveNewTask(TextBoxAddNew.Text, DueInTimeSpanPicker.TimeSpan);

                TextBoxAddNew.Text = "";
            }
        }

        private void ListViewSchedules_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataItemTask item = e.ClickedItem as DataItemTask;

            if (item.IsComplete)
                ViewModel.CoreViewModel.MarkIncomplete(item);
            else
                ViewModel.CoreViewModel.MarkComplete(item);
        }
    }
}
