using System;
using System.Globalization;
using Xamarin.Forms;

namespace Toolit.Converters
{
    public class IntegerToNaturalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double intg)
            {
                return intg > 0 ? intg : 1;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}