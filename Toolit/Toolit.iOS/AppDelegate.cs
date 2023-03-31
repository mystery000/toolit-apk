using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Acr.UserDialogs;
using DentMeApp.Shared.Contracts.Services;
using Foundation;
using Toolit.Interfaces;
using Toolit.iOS.Extensions;
using Toolit.iOS.Helpers;
using UIKit;
using UserNotifications;
using Xam.Shell.Badge.iOS;
using Xamarin.Forms;

namespace Toolit.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // Sender Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
                
        private IDeviceInstallationService _deviceInstallationService;
        private INotificationRegistrationService _notificationRegistrationService;
        
        IDeviceInstallationService DeviceInstallationService
            => _deviceInstallationService ??
               (_deviceInstallationService =
                   DependencyService.Get<IDeviceInstallationService>());
        
        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??
               (_notificationRegistrationService =
                   DependencyService.Get<INotificationRegistrationService>());


        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(); 
            Rg.Plugins.Popup.Popup.Init();
            BottomBar.Init();
            
            if (app is TouchEventUIApplication teuiAppl)
            {
                teuiAppl.InitTimer();
            }
            
            global::Xamarin.Forms.Forms.Init();
            
            LoadApplication(new App());
            
            // add notification delegate for foreground notifications
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

            return base.FinishedLaunching(app, options);
        }
        
        #region Remote notifications

        public override void RegisteredForRemoteNotifications(
            UIApplication application,
            NSData deviceToken)
        {
            CompleteRegistrationAsync(deviceToken).ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    NotificationRegistrationService.HandleFailedRegistration(task.Exception?.Message);
                }
            });
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            NotificationRegistrationService.HandleRemoteNotification();     
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            NotificationRegistrationService.HandleRemoteNotification();
        }

        public override void FailedToRegisterForRemoteNotifications(
            UIApplication application,
            NSError error)
        {
            Debug.WriteLine(error.Description);
            NotificationRegistrationService.HandleFailedRegistration(error.Description);
        }

        private System.Threading.Tasks.Task CompleteRegistrationAsync(NSData deviceToken)
        {
            DeviceInstallationService.Token = deviceToken?.ToHexString();
            return NotificationRegistrationService.RegisterDeviceAsync();
        }

        #endregion
    }
}
