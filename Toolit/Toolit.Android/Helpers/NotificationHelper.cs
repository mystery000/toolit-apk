using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Toolit.Resourses;

namespace Toolit.Droid.Helpers
{
    public static class NotificationHelper
    {
        public static void SendNotification(Context applicationContext, string messageBody, IDictionary<string, string> data)
        {
            var intent = new Intent(applicationContext, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(applicationContext, AppConstants.NotificationId, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(applicationContext, AppConstants.GeneralChannelId)
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetContentTitle("Toolit")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(messageBody))
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(applicationContext);
            notificationManager.Notify(AppConstants.NotificationId, notificationBuilder.Build());
        }
    }
}