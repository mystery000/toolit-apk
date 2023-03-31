using System;
using System.Globalization;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Toolit.Converters
{
    public class BoolToCaptureModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVideo)
            {
                return isVideo ? CameraCaptureMode.Video : CameraCaptureMode.Photo;
            }

            return CameraCaptureMode.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}