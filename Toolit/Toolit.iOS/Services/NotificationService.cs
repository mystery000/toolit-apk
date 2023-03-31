using System.Threading.Tasks;
using Foundation;
using Toolit.Interfaces;
using Toolit.iOS.Services;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(NotificationService))]
namespace Toolit.iOS.Services
{
    public class NotificationService : INotificationService
    {
        public bool HasNotificationPermission()
        {
            return UIApplication.SharedApplication.CurrentUserNotificationSettings.Types !=
                   UIUserNotificationType.None;
        }
        
        public async Task<(bool isSuccess, string error)> RequestNotificationPermissionAndRegister()
        {
            var result = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(
                UNAuthorizationOptions.Alert |
                UNAuthorizationOptions.Badge |
                UNAuthorizationOptions.Sound);

            if (result.Item1 && // approved
                result.Item2 == null) // no error
            {
                await RegisterForRemoteNotifications();
            }

            return (result.Item1, result.Item2?.Description);
        }
        
        private async System.Threading.Tasks.Task RegisterForRemoteNotifications()
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert |
                    UIUserNotificationType.Badge |
                    UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            });
        }
    }
}