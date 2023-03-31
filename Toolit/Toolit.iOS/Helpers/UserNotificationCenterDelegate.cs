using System;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using UserNotifications;

namespace Toolit.iOS.Helpers
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        #region Constructors
        
        public UserNotificationCenterDelegate ()
        {
        }
        
        #endregion

        #region Override Methods
        
        public override void WillPresentNotification (UNUserNotificationCenter center, 
            UNNotification notification, 
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do something with the notification
            Console.WriteLine ("Active Notification: {0}", notification);

            // TODO
            DAO.Instance.Poll();

            completionHandler (UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Sound);
        }
        
        #endregion
    }
}