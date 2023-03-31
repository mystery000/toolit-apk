using System;
using System.Globalization;
using Xamarin.Forms;

namespace Toolit
{
    public class MatchesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == (string)parameter ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new UnsupportedOperationException();
        }
    }
}
