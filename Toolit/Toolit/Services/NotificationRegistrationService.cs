using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using DentMeApp.Shared.Contracts.Services;
using Toolit.Interfaces;
using Toolit.Resourses;
using Xamarin.Forms;

namespace Toolit.Services
{
    public class NotificationRegistrationService : INotificationRegistrationService
    {
        private readonly IDeviceInstallationService _deviceInstallationService;
        private readonly IUserDialogs _userDialogs;
        private readonly DAO _dao;
        
        private readonly INotificationService _iosNotificationService;

        public NotificationRegistrationService()
        {
            _deviceInstallationService = DependencyService.Get<IDeviceInstallationService>();
            _userDialogs = UserDialogs.Instance;
            _dao = DAO.Instance;

            if (Device.RuntimePlatform == Device.iOS)
            {
                _iosNotificationService = DependencyService.Get<INotificationService>();
            }
        }

        private async System.Threading.Tasks.Task RefreshDeviceRegistrationAsync()
        {
            var cachedToken = await Settings.GetNotificationTokenAsync();
            
            if (string.IsNullOrWhiteSpace(cachedToken))
            {
                //throw new Exception("No saved PNS data found");
                return; // skip if no initial registration was made
            }

            _deviceInstallationService.Token = cachedToken;
            await RegisterDeviceAsync(cachedToken);
        }
        
        public async System.Threading.Tasks.Task RegisterDeviceAsync(string cachedToken = null)
        {
            Debug.WriteLine("Registering for notifications");

            var deviceInstallation = _deviceInstallationService.GetDeviceInstallation();

            if (string.IsNullOrEmpty(cachedToken)) // means that a new PNS token has been received
            {
                await Settings.SetNotificationTokenAsync(deviceInstallation.Token);
            }

            if (await _dao.IsSignedIn())
            {
                await _dao.RegisterDevice(deviceInstallation.Token);
            }
            
            Debug.WriteLine("Notification registration successful");
        }

        public async Task<(bool isSuccess, string errorMsg)> RegisterForNotificationsAsync()
        {
            try
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var hasPermission = await Device.InvokeOnMainThreadAsync(() => 
                        _iosNotificationService.HasNotificationPermission());

                    // request permissions if first login
                    if (!hasPermission)
                    {
                        var result = await _iosNotificationService.RequestNotificationPermissionAndRegister();
                        return (result.isSuccess, result.error ?? AppResources.NotificationNoPermissionErrorString);
                    }
                    else
                    {
                        // register with DAO if permission was already given
                        await RefreshDeviceRegistrationAsync();

                        return (true, null);
                    }
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    if (await _dao.IsSignedIn())
                    {
                        await RefreshDeviceRegistrationAsync();
                    }

                    return (true, null);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            return (false, null);
        }

        
        public async System.Threading.Tasks.Task HandleRemoteNotification()
        {
            _dao.Poll();
        }

        public async System.Threading.Tasks.Task HandleFailedRegistration(string errorMsg)
        {
            try
            {
                await _userDialogs.AlertAsync(string
                    .Format(AppResources.NotificationRegistrationErrorString, errorMsg));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}