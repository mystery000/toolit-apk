using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using DentMeApp.Shared.Contracts.Services;
using Toolit.Interfaces;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class LoadUserViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToMain();
            System.Threading.Tasks.Task MoveToWelcome();
        }

        private readonly ICallback _view;
        private bool _hasLoggedOut;

        private INotificationRegistrationService _notificationRegistrationService;
        private IAppTrackingService _appTrackingService;
        
        private static bool _isNotificationRegistrationCompleted;
        
        private static bool _isFirstLoad;


        public bool IsFirstLoad
        {
            get => _isFirstLoad;
            set => _isFirstLoad = value;
        }

        static LoadUserViewModel()
        {
            _isFirstLoad = true;
        }
        
        public LoadUserViewModel(ICallback view)
        {
            _view = view;
            _notificationRegistrationService = DependencyService.Get<INotificationRegistrationService>();
            _appTrackingService = DependencyService.Get<IAppTrackingService>();
        }

        public override async void Navigated()
        {
            var timeoutDt = await Settings.GetSessionTimeoutTime();
            // TODO: uncomment if bankid's auto sign out rule will be followed
//#if DEBUG
            var isSessionTimedOut = false;
// #else
//             var isSessionTimedOut = (timeoutDt < DateTime.UtcNow);
// #endif

            if (string.IsNullOrEmpty(Settings.RefreshToken) || isSessionTimedOut)
            {
                if (timeoutDt > DateTime.MinValue && isSessionTimedOut)
                {
                    dao.SignOut();
                    if (timeoutDt != DateTime.MinValue)
                    {
                        userDialogs.Toast(AppResources.SessionEndedMessage);
                        await Settings.SetSessionTimeoutTime(DateTime.MinValue);
                    }
                }

                // if session not timed out, it's likely a first login, so show onboarding
                IsFirstLoad = false;
                await _view.MoveToWelcome();
            }
            else
            {
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    dao.ImplicitSignIn(); // NOTE: Kickstarts poll.
                    System.Diagnostics.Debug.WriteLine("Implicit signin complete");

                    var ok = await dao.IsSignedIn() && !isSessionTimedOut;
                    Settings.IsNotFirstLaunch = true;
                    System.Diagnostics.Debug.WriteLine("IsSignedIn complete");
                    
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        MessagingCenter.Send(new AuthMessage(), AppConstants.UserLoggedInEventMessage);
                        if (ok)
                        {
                            var u = dao.ActiveUser;
                            
                            // Register for notifications.
                            if (!_isNotificationRegistrationCompleted)
                            {
                                var notificationRegistrationResult =
                                    await _notificationRegistrationService.RegisterForNotificationsAsync();
                
                                if (!notificationRegistrationResult.isSuccess)
                                {
                                    await userDialogs.AlertAsync(string.Format(AppResources.NotificationRegistrationErrorString,
                                        notificationRegistrationResult.errorMsg ?? string.Empty));
                                }
 
                                _isNotificationRegistrationCompleted = true;
                            }
                            
                            // Register for ad tracking.
                            if (DeviceInfo.Platform == DevicePlatform.iOS)
                            {
                                await _appTrackingService.RequestTrackingPermission();
                            }
                            
                            IsFirstLoad = false;
                            await _view.MoveToMain();
                        }
                        else // Should never happen. TODO: Report?
                        {
                            IsFirstLoad = false;
                            await _view.MoveToWelcome();
                        }
                    });
                });
            }
        }
    }
}