using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace TilesAndRemindersApp.Converters
{
    public class TimeSpanToFriendlyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan span = (TimeSpan)value;

            if (span.TotalDays >= 1)
            {
                if (span.TotalDays == 1)
                    return "1 day";

                return (int)span.TotalDays + " days";
            }

            return (int)span.TotalSeconds + " seconds";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
