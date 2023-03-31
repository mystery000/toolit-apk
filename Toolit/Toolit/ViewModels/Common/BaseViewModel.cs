using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using Microsoft.AppCenter.Crashes;
using Toolit.Interfaces;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChangedBase, INavigationHandler
    {
        private static bool _isEndSessionInProgress;
        
        protected static readonly DAO dao = DAO.Instance;
        protected static readonly IUserDialogs userDialogs = UserDialogs.Instance;

        private static bool _isUpdateRequired;
        private bool _isSubscribedToEndSession;
        
        protected bool IsSubscribedToDao;

        public bool IsATab { get; set; }

        private bool notInProgress;
        public bool NotInProgress
        {
            get { return notInProgress; }
            set
            {
                notInProgress = value;
                OnPropertyChanged();
            }
        }
        
        // determines whether the iOS device has a safe area (i.e. notch)
        private static double _statusBarHeight;
        public bool HasSafeArea => _statusBarHeight > 20;

        static BaseViewModel()
        {
            _isUpdateRequired = false;

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                var statusBarService = DependencyService.Get<IStatusBarSizeService>();
                _statusBarHeight = statusBarService.GetStatusBarHeight();
            }
        }

        protected BaseViewModel()
        {
            notInProgress = true;
        }

        public virtual void Navigated()
        {
            // TODO: uncomment if bankid's auto sign out rule will be followed
            // if (!_isSubscribedToEndSession)
            // {
            //     MessagingCenter.Subscribe<object>(this, AppConstants.SessionTimeoutMessage, EndUserSession);
            //     _isSubscribedToEndSession = true;
            // }
        }

        public virtual void NavigatingFrom()
        {
            // TODO: uncomment if bankid's auto sign out rule will be followed
            // if (_isSubscribedToEndSession && !IsATab)
            // {
            //     MessagingCenter.Unsubscribe<object>(this, AppConstants.SessionTimeoutMessage);
            //     _isSubscribedToEndSession = false;
            // }
        }
        
        protected virtual void HandleError(Exception ex, string error)
        {
            userDialogs.HideLoading();
            Crashes.TrackError(ex);
#if DEBUG
            userDialogs.Toast($"{AppResources.NetworkErrorString}; {ex.Message}");
#else
            userDialogs.Toast(AppResources.NetworkErrorString);
#endif
        }

        private static async void EndUserSession(object sender)
        {
            // TODO: remove if bankid's auto sign out rule will be followed
            return;
            
            if (!_isEndSessionInProgress && await dao.IsSignedIn())
            {
                _isEndSessionInProgress = true;

                dao.SignOut();
                MessagingCenter.Send(new AuthMessage(), AppConstants.UserLoggedOutEventMessage);
                
                await Device.InvokeOnMainThreadAsync(async () => {
                    // return to welcome page (not MVVM-compliant, sadly)
                    await Shell.Current.GoToAsync(state: "//login");
                });

                userDialogs.Toast(AppResources.SessionEndedMessage);

                _isEndSessionInProgress = false;
            }
        }
    }
}
