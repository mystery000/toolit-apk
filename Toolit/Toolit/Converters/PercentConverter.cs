using System;
using System.Globalization;
using Xamarin.Forms;

namespace Toolit
{
    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round((decimal)(double)value * 100, GetParameter(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new UnsupportedOperationException();
        }

        private int GetParameter(object parameter)
        {
            if (parameter is double)
            {
                return (int)parameter;
            }
            else if (parameter is int)
            {
                return (int)parameter;
            }
            else if (parameter is string)
            {
                return int.Parse((string)parameter);
            }
            else 
            {
                return 1;
            }
        }
    }
}
