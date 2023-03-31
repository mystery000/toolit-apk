using Android.Gms.Common;
using Java.Lang;
using Toolit.Droid.Services;
using Toolit.Helpers;
using Toolit.Interfaces;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(DeviceInstallationService))]
namespace Toolit.Droid.Services
{
    public class DeviceInstallationService : IDeviceInstallationService
    {
        public string Token { get; set; }

        public bool AreNotificationsSupported => GoogleApiAvailability.Instance
            .IsGooglePlayServicesAvailable(Application.Context) == ConnectionResult.Success;
        
        public string GetDeviceId()
        {
            return Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver,
                Android.Provider.Settings.Secure.AndroidId);
        }

        public DeviceInstallation GetDeviceInstallation()
        {
            if (!AreNotificationsSupported)
                throw new Exception(GetPlayServicesError());

            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Unable to resolve token for FCM");
            
            var installation = new DeviceInstallation
            {
                InstallId = GetDeviceId(),
                Platform = "fcm",
                Token = Token
            };

            return installation;
        }

        private string GetPlayServicesError()
        {
            var resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(Application.Context);

            if (resultCode != ConnectionResult.Success)
            {
                return GoogleApiAvailability.Instance.IsUserResolvableError(resultCode)
                    ? GoogleApiAvailability.Instance.GetErrorString(resultCode)
                    : "This device is not supported";
            }

            return "An error occurred preventing the use of push notifications";
        }
    }
}