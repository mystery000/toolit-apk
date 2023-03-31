using System;
using DentMeApp.Shared.Contracts.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Toolit.Resourses;
using Toolit.Services;

namespace Toolit
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            DependencyService.Register<INotificationRegistrationService, NotificationRegistrationService>();
            
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            AppCenter.Start("ios=222b767e-3ccd-404f-bc44-c441aef74cb2;" + 
                "android=2ffc0884-320e-4216-940a-77c54f40e136;" +
                typeof(Analytics), typeof(Crashes));
        }

        protected override async void OnSleep()
        {
            await Settings.SetSessionTimeoutTime(DateTime.UtcNow
                .AddMilliseconds(AppConstants.SessionTimeoutDuration));
        }

        protected override async void OnResume()
        {
            var timeoutDt = await Settings.GetSessionTimeoutTime();
            if (timeoutDt < DateTime.UtcNow) // check if the session had timed out while the app was inactive
            {
                // TODO: uncomment if bankid's auto sign out rule will be followed
                //MessagingCenter.Send<object>(this, AppConstants.SessionTimeoutMessage);
            }
            else
            {
                if (await DAO.Instance.IsSignedIn())
                {
                    DAO.Instance.Poll();
                }
            }
        }
    }
}
