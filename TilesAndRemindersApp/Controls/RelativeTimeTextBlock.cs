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
    public sealed class RelativeTimeTextBlock : Control
    {
        public RelativeTimeTextBlock()
        {
            this.DefaultStyleKey = typeof(RelativeTimeTextBlock);

            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(0.5)
            };

            timer.Tick += Timer_Tick;

            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            UpdateText();
        }

        private string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RelativeTimeTextBlock), new PropertyMetadata(""));




        public DateTimeOffset DateTime
        {
            get { return (DateTimeOffset)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTimeOffset), typeof(RelativeTimeTextBlock), new PropertyMetadata(DateTimeOffset.MinValue));


        private static void OnDateTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as RelativeTimeTextBlock).UpdateText();
        }

        private void OnDateTimeChanged()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            Text = GetRelativeText();
        }

        private string GetRelativeText()
        {
            TimeSpan span = DateTime - DateTimeOffset.Now;
            int days = (int)(DateTime.UtcDateTime.Date - DateTimeOffset.Now.UtcDateTime.Date).TotalDays;

            if (days >= 1)
            {
                if (days == 1)
                    return "tomorrow";

                return days + " days";
            }

            if (span.TotalHours >= 1)
            {
                if ((int)span.TotalHours == 1)
                    return "1 hour";

                return (int)span.TotalHours + " hours";
            }

            if (span.TotalMinutes >= 1)
            {
                if ((int)span.TotalMinutes == 1)
                    return "1 minute";

                return (int)span.TotalMinutes + " minutes";
            }


            return (int)span.TotalSeconds + " seconds";
        }
    }
}
