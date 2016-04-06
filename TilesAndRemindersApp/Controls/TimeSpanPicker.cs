using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace TilesAndRemindersApp.Controls
{
    [TemplatePart(Name = TimeSpanPicker.ElementFiveSecondsRadioBox, Type = typeof(TimeSpanPicker))]
    [TemplatePart(Name = TimeSpanPicker.ElementTenSecondsRadioBox, Type = typeof(TimeSpanPicker))]
    [TemplatePart(Name = TimeSpanPicker.ElementTwentySecondsRadioBox, Type = typeof(TimeSpanPicker))]
    public sealed class TimeSpanPicker : Control
    {
        private const string ElementFiveSecondsRadioBox = "PART_FiveSecondsRadioBox";
        private const string ElementTenSecondsRadioBox = "PART_TenSecondsRadioBox";
        private const string ElementTwentySecondsRadioBox = "PART_TwentySecondsRadioBox";

        private RadioButton _fiveSecondsRadioBox;
        private RadioButton _tenSecondsRadioBox;
        private RadioButton _twentySecondsRadioBox;

        public TimeSpanPicker()
        {
            this.DefaultStyleKey = typeof(TimeSpanPicker);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //_fiveSecondsRadioBox = GetTemplateChild(ElementFiveSecondsRadioBox) as RadioButton;
            //_tenSecondsRadioBox = GetTemplateChild(ElementTenSecondsRadioBox) as RadioButton;
            //_twentySecondsRadioBox = GetTemplateChild(ElementTwentySecondsRadioBox) as RadioButton;

            //_fiveSecondsRadioBox.Checked += _fiveSecondsRadioBox_Checked;
            //_tenSecondsRadioBox.Checked += _tenSecondsRadioBox_Checked;
            //_twentySecondsRadioBox.Checked += _twentySecondsRadioBox_Checked;
        }

        private void _twentySecondsRadioBox_Checked(object sender, RoutedEventArgs e)
        {
            TimeSpan = TimeSpan.FromSeconds(20);
        }

        private void _tenSecondsRadioBox_Checked(object sender, RoutedEventArgs e)
        {
            TimeSpan = TimeSpan.FromSeconds(10);
        }

        private void _fiveSecondsRadioBox_Checked(object sender, RoutedEventArgs e)
        {
            TimeSpan = TimeSpan.FromSeconds(5);
        }





        public TimeSpan[] AvailableTimeSpans
        {
            get { return (TimeSpan[])GetValue(AvailableTimeSpansProperty); }
            set { SetValue(AvailableTimeSpansProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AvailableTimeSpans.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AvailableTimeSpansProperty =
            DependencyProperty.Register("AvailableTimeSpans", typeof(TimeSpan[]), typeof(TimeSpanPicker), new PropertyMetadata(new TimeSpan[]
                {
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20),
                    TimeSpan.FromDays(1),
                    TimeSpan.FromDays(2),
                    TimeSpan.FromDays(4)
                }));






        public TimeSpan TimeSpan
        {
            get { return (TimeSpan)GetValue(TimeSpanProperty); }
            set { SetValue(TimeSpanProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeSpanProperty =
            DependencyProperty.Register("TimeSpan", typeof(TimeSpan), typeof(TimeSpanPicker), new PropertyMetadata(TimeSpan.FromSeconds(5)));



        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(TimeSpanPicker), new PropertyMetadata(null));



        //private bool IsFiveSecondsChecked
        //{
        //    get { return (bool)GetValue(IsFiveSecondsCheckedProperty); }
        //    set { SetValue(IsFiveSecondsCheckedProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsFiveSecondsChecked.  This enables animation, styling, binding, etc...
        //private static readonly DependencyProperty IsFiveSecondsCheckedProperty =
        //    DependencyProperty.Register("IsFiveSecondsChecked", typeof(bool), typeof(TimeSpanPicker), new PropertyMetadata(true));



        //private bool IsTenSecondsChecked
        //{
        //    get { return (bool)GetValue(IsTenSecondsCheckedProperty); }
        //    set { SetValue(IsTenSecondsCheckedProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsTenSecondsChecked.  This enables animation, styling, binding, etc...
        //private static readonly DependencyProperty IsTenSecondsCheckedProperty =
        //    DependencyProperty.Register("IsTenSecondsChecked", typeof(bool), typeof(TimeSpanPicker), new PropertyMetadata(false));



        //private bool IsTwentySecondsChecked
        //{
        //    get { return (bool)GetValue(IsTwentySecondsCheckedProperty); }
        //    set { SetValue(IsTwentySecondsCheckedProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsTwentySecondsChecked.  This enables animation, styling, binding, etc...
        //private static readonly DependencyProperty IsTwentySecondsCheckedProperty =
        //    DependencyProperty.Register("IsTwentySecondsChecked", typeof(bool), typeof(TimeSpanPicker), new PropertyMetadata(false));


    }
}
