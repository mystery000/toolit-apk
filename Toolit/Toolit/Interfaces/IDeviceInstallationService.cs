using DentMeApp;
using Toolit.Helpers;

namespace Toolit.Interfaces
{
    public interface IDeviceInstallationService
    {
        string Token { get; set; }
        bool AreNotificationsSupported { get; }

        string GetDeviceId();
        DeviceInstallation GetDeviceInstallation();
    }
}