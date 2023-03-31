using System;
using Toolit.Helpers;
using Toolit.Interfaces;
using Toolit.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInstallationService))]
namespace Toolit.iOS.Services
{
    public class DeviceInstallationService : IDeviceInstallationService
    {
        private const int SupportedVersionMajor = 11;
        private const int SupportedVersionMinor = 1;
        
        public string Token { get; set; }

        public bool AreNotificationsSupported => UIDevice.CurrentDevice
            .CheckSystemVersion(SupportedVersionMajor, SupportedVersionMinor);
        
        public string GetDeviceId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        }

        public DeviceInstallation GetDeviceInstallation()
        {
            if (!AreNotificationsSupported)
                throw new Exception(GetNotificationsSupportError());

            if (string.IsNullOrWhiteSpace(Token))
                throw new Exception("Unable to resolve token for APNS");

            var installation = new DeviceInstallation
            {
                InstallId = GetDeviceId(),
                Platform = "apns",
                Token = Token
            };

            return installation;
        }
        
        private string GetNotificationsSupportError()
        {
            if (!AreNotificationsSupported)
                return $"This app only supports notifications on iOS {SupportedVersionMajor}.{SupportedVersionMinor} and above. You are running {UIDevice.CurrentDevice.SystemVersion}.";

            if (Token == null)
                return $"This app can support notifications but you must enable this in your settings.";


            return "An error occurred preventing the use of push notifications";
        }
    }
}