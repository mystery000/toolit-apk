using System;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using Toolit.Extensions;
using Toolit.Resourses;
using Xamarin.Essentials;

namespace Toolit.Helpers
{
    public static class EssentialsHelper
    {
        private static readonly IUserDialogs _userDialogs = UserDialogs.Instance;

        public static void TryPhoneCall(string phoneNumber)
        {
            // avoids emulator crash on XamEssentials 1.6 
            if (DeviceInfo.Platform == DevicePlatform.iOS &&
                (DeviceInfo.Idiom != DeviceIdiom.Phone || DeviceInfo.DeviceType != DeviceType.Physical))
            {
                _userDialogs.Toast(AppResources.InvalidPhoneNumberErrorMessage);
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(phoneNumber))
            {
                // remove everything but digits
                var hasLeadingPlus = phoneNumber.First().Equals('+');
                var msisdnNumber = new string(phoneNumber.ToCharArray().Where(char.IsDigit).ToArray());

                if (ValidationHelper.IsPhoneNumberValid(msisdnNumber))
                {
                    try
                    {
                        var leadingChars = hasLeadingPlus ? "+" : string.Empty;
                        PhoneDialer.Open($"{leadingChars}{msisdnNumber}");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"In EssentialsHelper: PhoneDialer call failed: {ex.Message}");
                    }
                }
            }
            
            _userDialogs.Toast(AppResources.InvalidPhoneNumberErrorMessage);
        }

        public static void TryOpenMapClient(Placemark location)
        {
            try
            {
                Map.OpenAsync(location);
            }
            catch (Exception)
            {
                _userDialogs.Toast(AppResources.InvalidPhoneNumberErrorMessage);
            }
        }

        public static async System.Threading.Tasks.Task TryOpenEmailClient(string email)
        {
            try
            {
                var uri = new Uri($"mailto:{email}");
                if (!(await Launcher.TryOpenAsync(uri)))
                {
                    _userDialogs.Toast(AppResources.NoEmailClientFoundErrorMessage);
                }
            }
            catch (Exception)
            {
                _userDialogs.Toast(AppResources.InvalidEmailAddressErrorMessage);
            }
        }

        public static async System.Threading.Tasks.Task TryOpenWebBrowser(string url)
        {
            try
            {
                var uri = new Uri(url.ParseUrlForUri());
                if (!(await Launcher.TryOpenAsync(uri)))
                {
                    _userDialogs.Toast(AppResources.NoWebBrowserFoundErrorMessage);
                }
            }
            catch (Exception)
            {
                _userDialogs.Toast(AppResources.InvalidWebAddressErrorMessage);
            }
        }
    }
}