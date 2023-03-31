using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using DentMeApp.Shared.Contracts.Services;
using Toolit.Interfaces;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class EmailSignInViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToMain();
        }
        
        private readonly ICallback view;
        
        private INotificationRegistrationService _notificationRegistrationService;
        private IAppTrackingService _appTrackingService;
        
        private static bool _isNotificationRegistrationCompleted;

        #region Bindable properties
        
        private IUserDialogs _userDialogs;

        private ValidatableObject<string> _email;
        public ValidatableObject<string> Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private ValidatableObject<string> _password;

        public ValidatableObject<string> Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        #endregion
        
        public ICommand LoginCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public EmailSignInViewModel(ICallback view)
        {
            this.view = view;
            _notificationRegistrationService = DependencyService.Get<INotificationRegistrationService>();
            _appTrackingService = DependencyService.Get<IAppTrackingService>();
            
            BackCommand = new Command(Back);
            LoginCommand = new Command(Login);

            Email = new ValidatableObject<string>();
            Password = new ValidatableObject<string>();

            Email.Validations.Add(new IsValidEmailRule<string>()
                {
                    ValidationMessage = AppResources.PleaseFillAllFieldsErrorString
                });

            Password.Validations.Add(new IsNotNullOrEmptyRule<string>()
                {
                    ValidationMessage = AppResources.PleaseFillAllFieldsErrorString
                });
        }

        public async void Back()
        {
            await view.Back();
        }

        public async void Login()
        {
            Email?.Validate();

            _userDialogs = UserDialogs.Instance;
            if (!Email.IsValid)
            {
                _userDialogs.Toast(Email.Validations.First()
                    .ValidationMessage);

                return;
            }

            Password?.Validate();

            if (!Password.IsValid)
            {
                _userDialogs.Toast(Password.Validations.First()
                    .ValidationMessage);

                return;
            }
            
            try
            {
                _userDialogs.ShowLoading(AppResources.LoadingString);
                
                await dao.SigninEmail(Email.Value, Password.Value);
                
                // wait for user poll to complete
                await dao.IsSignedIn();
                Settings.IsNotFirstLaunch = true;
                MessagingCenter.Send(new AuthMessage(), AppConstants.UserLoggedInEventMessage);
                
                // register for notifications
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
                
                // register for ad tracking
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await _appTrackingService.RequestTrackingPermission();
                }
                
                _userDialogs.HideLoading();

                Email.Value = string.Empty;
                Password.Value = string.Empty;
                await view.MoveToMain();
            }
            catch (WrongCredentialsException wcEx)
            {
                Password.Value = string.Empty;

                _userDialogs.HideLoading();
                _userDialogs.Toast(AppResources.InvalidCredentialsErrorString);
            }
            catch (Exception ex)
            {
                Password.Value = string.Empty;
                
                _userDialogs.HideLoading();
                _userDialogs.Toast(AppResources.NetworkErrorString);
            }
        }
    }
}
