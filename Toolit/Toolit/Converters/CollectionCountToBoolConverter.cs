using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace Toolit.Converters
{
    public class CollectionCountToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter?.ToString().ToLower() == "i")
            {
                if(value is IEnumerable<object> collectionWithParameter)
                {
                    return !(collectionWithParameter?.Count() > 0);
                }

                return true;
            }
            
            if(value is IEnumerable<object> collection)
            {
                return collection?.Count() > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
