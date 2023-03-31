using System;
using System.Collections.Generic;
using System.Windows.Input;
using DentMeApp.Shared.Contracts.Services;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;
using Microsoft.AppCenter.Crashes;
using Toolit.Helpers;
using Toolit.Interfaces;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels.Popups
{
    public class BankIdSignUpPopupViewModel : BaseViewModel
    {
        private readonly ICallback _view;
        
        private INotificationRegistrationService _notificationRegistrationService;
        private IAppTrackingService _appTrackingService;
        
        private static bool _isNotificationRegistrationCompleted;
        
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _opaquePnum;

        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToMain();
        }

        public ValidatableObject<string> Email { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Address { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Phone { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> PostCode { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> City { get; } = new ValidatableObject<string>();
        public ValidatableObject<bool> TermsAndCondition { get; } = new ValidatableObject<bool>();
        public ValidatableObject<bool> DataProtectionPolicy { get; } = new ValidatableObject<bool>();
        public ValidatableGroup Form { get; } = new ValidatableGroup();
        
        public ICommand MoveToToSCommand { get; private set; }
        public ICommand MoveToDataPolicyCommand { get; private set; }
        public ICommand SignUpCommand { get; private set; }

        public BankIdSignUpPopupViewModel(ICallback view, string firstName, string lastName, string opaquePnum)
        {
            _view = view;
            _firstName = firstName;
            _lastName = lastName;
            _opaquePnum = opaquePnum;
            
            _notificationRegistrationService = DependencyService.Get<INotificationRegistrationService>();
            _appTrackingService = DependencyService.Get<IAppTrackingService>();

            MoveToToSCommand = new AsyncCommand(MoveToToS);
            MoveToDataPolicyCommand = new AsyncCommand(MoveToDataPolicy);
            SignUpCommand = new AsyncCommand(SignUp);
            
            Email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "EmailCannotBeEmpty" });
            Email.Validations.Add(new IsValidEmailRule<string> { ValidationMessage = "EmailNotValid" });
            Phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "PhoneCannotBeEmpty" });
            Address.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "AddressCannotBeEmpty" });
            PostCode.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "PostCodeCannotBeEmpty" });
            City.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "PostOfficeCannotBeEmpty" });
            TermsAndCondition.Validations.Add(new IsValueTrueRule<bool> { ValidationMessage = "AcceptUserTerms" });
            DataProtectionPolicy.Validations.Add(new IsValueTrueRule<bool> { ValidationMessage = "AcceptDataPolicy" });
                
            Form.Add(new IIsValid[] { Email, Phone, Address, PostCode, City, TermsAndCondition, DataProtectionPolicy });
            
        }

        private async System.Threading.Tasks.Task MoveToToS()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitTosUrl);
        }

        private async System.Threading.Tasks.Task MoveToDataPolicy()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitDataPolicyUrl);
        }

        private async System.Threading.Tasks.Task SignUp()
        {
            if (await IsSignUpSuccessful())
            {
                // register for notifications
                await RegisterForNotifications();
                
                 
                // register for ad tracking
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await _appTrackingService.RequestTrackingPermission();
                }
                
                await _view.MoveToMain();
            }
        }
        
        private async System.Threading.Tasks.Task<bool> IsSignUpSuccessful()
        {
            userDialogs.ShowLoading(AppResources.LoadingString);

            try
            {
                var dt = DateTime.UtcNow;

                var newUserMdl = new User()
                {
                    Id = string.Empty,
                    FirstName = _firstName,
                    MiddleNames = string.Empty,
                    LastName = _lastName,
                    Description = string.Empty,
                    Email = Email.Value,
                    Phone = $"+46{Phone.Value}",
                    Address = Address.Value,
                    Postcode = PostCode.Value,
                    City = City.Value,
                    Devices = new List<User.Device>(),
                    Message = string.Empty,
                    PreferredName = _firstName,
                    Modified = dt,
                    Country = "SE",
                    Nid = string.Empty,
                    Started = dt,
                };

                // get location
                var geometryCol = LocationHelper.BuildGeometryCollection(new Location(59.33, 18.0));
                try
                {
                    var location = await Geolocation.GetLocationAsync();
                    geometryCol = LocationHelper.BuildGeometryCollection(location);
                }
                catch (PermissionException pEx)
                {
                    
                }

                await dao.Signup(newUserMdl, _opaquePnum, location: geometryCol);

                userDialogs.HideLoading();

                return true;
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                Crashes.TrackError(ex, new Dictionary<string, string>());
                
                userDialogs.Alert(string.Format(AppResources.NetworkErrorString, ex.Message));

                return false;
            }
        }
        
        
        private async System.Threading.Tasks.Task RegisterForNotifications()
        {
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
        }
    }
}