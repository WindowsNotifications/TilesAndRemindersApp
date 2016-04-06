using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TilesAndRemindersApp.ViewModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TilesAndRemindersApp.View
{
    public class BackButtonHandler : FrameworkElement
    {
        private SystemNavigationManager _manager;
        private CoreViewModel _coreViewModel;

        public BackButtonHandler(CoreViewModel coreViewModel)
        {
            _manager = SystemNavigationManager.GetForCurrentView();
            _coreViewModel = coreViewModel;
            _manager.BackRequested += _manager_BackRequested;

            SetBinding(CanGoBackProperty, new Binding()
            {
                Path = new PropertyPath("CanGoBack"),
                Source = _coreViewModel
            });
        }

        private void _manager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            _coreViewModel.GoBack();
        }

        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanGoBack.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof(bool), typeof(BackButtonHandler), new PropertyMetadata(false, OnCanGoBackChanged));

        private static void OnCanGoBackChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as BackButtonHandler).OnCanGoBackChanged(e);
        }

        private void OnCanGoBackChanged(DependencyPropertyChangedEventArgs e)
        {
            _manager.AppViewBackButtonVisibility = CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }
    }
}
