using System;
using System.Timers;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DentMeApp.Shared.Contracts.Services;
using Toolit.Resourses;
using Xam.Shell.Badge.Droid;
using Xamarin.Forms;

namespace Toolit.Droid
{
    [Activity(
        Label = "Toolit", 
        Icon = "@mipmap/ic_launcher", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        private static Timer _inactivityTimer;
        private static bool _isTimerConnected;
        
        private INotificationRegistrationService _notificationRegistrationService;

        INotificationRegistrationService NotificationRegistrationService
            => _notificationRegistrationService ??= DependencyService.Get<INotificationRegistrationService>();


        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            ResetNotificationChannels();
            
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true); 
            Rg.Plugins.Popup.Popup.Init(this);
            UserDialogs.Init(this);
            BottomBar.Init();
            
            InitTimer();
            
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
        // inactivity timer
        
        public override void OnUserInteraction()
        {
            base.OnUserInteraction();

            ResetTimer();
        }
        
        
        private void InitTimer()
        {
            _inactivityTimer = new Timer(AppConstants.SessionTimeoutDuration);
            if (!_isTimerConnected) // disallow multiple subscriptions
            {
                _inactivityTimer.Elapsed += InactivityTimer_Elapsed;
                _isTimerConnected = true;
            }

            _inactivityTimer.Start();
            
            System.Diagnostics.Debug.WriteLine("Inactivity timer created");
        }

        private void InactivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // generic parameter passed to the method doesn't have to be the sender class
            // TODO: uncomment if bankid's auto sign out rule will be followed
            //MessagingCenter.Send<object>(this, AppConstants.SessionTimeoutMessage);
            System.Diagnostics.Debug.WriteLine("Inactivity timer elapsed\n");
        }

        private void ResetTimer()
        {
            // this effectively resets the timer and starts it again
            _inactivityTimer.Stop();
            _inactivityTimer.Start();

            System.Diagnostics.Debug.WriteLine("Inactivity timer reset\n");
        }


        // notification handler
        protected override async void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            
            await NotificationRegistrationService.HandleRemoteNotification();
        }
        
        
        private void ResetNotificationChannels()
        {
            // general notifications
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification 
                // channel on older versions of Android.
                return;
            }

            var generalChannel = new NotificationChannel(AppConstants.GeneralChannelId, 
                "General Notifications", NotificationImportance.Max)
            {
                Description = "Firebase Cloud Messages appear in this channel"
            };
            
            generalChannel.EnableVibration(true);
            generalChannel.LockscreenVisibility = NotificationVisibility.Public;

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            notificationManager?.CreateNotificationChannel(generalChannel);
        }
    }
}