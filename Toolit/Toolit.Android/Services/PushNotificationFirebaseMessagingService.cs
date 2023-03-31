using System.Diagnostics;
using Android.App;
using DentMeApp.Shared.Contracts.Services;
using Firebase.Messaging;
using Toolit.Droid.Helpers;
using Toolit.Interfaces;
using Xamarin.Forms;

namespace Toolit.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class PushNotificationFirebaseMessagingService : FirebaseMessagingService
    {
        private IDeviceInstallationService _deviceInstallationService;
        private INotificationRegistrationService _notificationRegistrationService;
        
        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??= DependencyService.Get<IDeviceInstallationService>();
        
        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??= DependencyService.Get<INotificationRegistrationService>();

        public override async void OnNewToken(string deviceToken)
        {
            DeviceInstallationService.Token = deviceToken;
            await NotificationRegistrationService.RegisterDeviceAsync();
        }

        public override async void OnMessageReceived(RemoteMessage msg)
        {
            Debug.WriteLine("From: " + msg.From);

            var notification = msg.GetNotification();
            if (notification != null)
            {
                Debug.WriteLine($"Notification body: {notification.Body}");
                
                NotificationHelper.SendNotification(this, notification.Body, msg.Data);
            }
            
            await NotificationRegistrationService.HandleRemoteNotification();
        }
    }
}