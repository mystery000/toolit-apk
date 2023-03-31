using System;
using System.Globalization;
using System.Linq;
using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.Converters
{
    public class UserToInitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserUiModel userUiMdl)
            {
                return
                    $"{char.ToUpper((userUiMdl.PreferredName ?? userUiMdl.FirstName).FirstOrDefault())}{char.ToUpper(userUiMdl.LastName.FirstOrDefault())}";
            }
            if (value is User userMdl)
            {
                return
                    $"{char.ToUpper((userMdl.PreferredName ?? userMdl.FirstName).FirstOrDefault())}{char.ToUpper(userMdl.LastName.FirstOrDefault())}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}