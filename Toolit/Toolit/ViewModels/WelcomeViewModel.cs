using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using DentMeApp.Shared.Contracts.Services;
using Microsoft.AppCenter.Crashes;
using Toolit.Helpers;
using Toolit.Interfaces;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class WelcomeViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToMain();
            System.Threading.Tasks.Task MoveToEmailSignIn();
            System.Threading.Tasks.Task OpenBankIdSignUp(string firstName, string lastName, string opaquePnum);
        }

        public enum WelcomeItemType
        {
            OnBoarding,
            BankId,
            Login
        }

        public class CarouselItem : INotifyPropertyChangedBase
        {
            public string BackgroundImage { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            public WelcomeItemType Type { get; set; }
        }

        private ObservableCollection<CarouselItem> _carouselItems;

        public ObservableCollection<CarouselItem> CarouselItems
        {
            get { return _carouselItems; }
            set
            {
                _carouselItems = value;
                OnPropertyChanged();
            }
        }

        private readonly ICallback view;

        private INotificationRegistrationService _notificationRegistrationService;
        private IAppTrackingService _appTrackingService;

        private static bool _isNotificationRegistrationCompleted;

        private int _selectedPageIndex;
        private int _emailSignInNumberOfTaps;

        private CarouselItem _selectedPage;

        public CarouselItem SelectedPage
        {
            get => _selectedPage;
            set
            {
                _selectedPage = value;
                OnPropertyChanged();
            }
        }

        public ICommand NextCommand { get; private set; }
        public ICommand OpenBankIdCommand { get; private set; }
        public ICommand MoveToEmailSignInCommand { get; private set; }

        public WelcomeViewModel(ICallback view)
        {
            this.view = view;

            _notificationRegistrationService = DependencyService.Get<INotificationRegistrationService>();
            _appTrackingService = DependencyService.Get<IAppTrackingService>();

            NextCommand = new AsyncCommand(Next);
            OpenBankIdCommand = new AsyncCommand(OpenBankId);
            MoveToEmailSignInCommand = new AsyncCommand(MoveToEmailSignIn);

            CarouselItems = new ObservableCollection<CarouselItem>
            {
                new CarouselItem()
                {
                    BackgroundImage = "bg_one",
                    Title = AppResources.WelcomeIntroTitle1,
                    Text = AppResources.WelcomeIntroDescription1,
                    Type = WelcomeItemType.OnBoarding
                },
                new CarouselItem()
                {
                    BackgroundImage = "bg_two",
                    Title = AppResources.WelcomeIntroTitle2,
                    Text = AppResources.WelcomeIntroDescription2,
                    Type = WelcomeItemType.OnBoarding
                },
                new CarouselItem()
                {
                    BackgroundImage = "bg_three",
                    Title = AppResources.WelcomeIntroTitle3,
                    Text = AppResources.WelcomeIntroDescription3,
                    Type = WelcomeItemType.OnBoarding
                },
                new CarouselItem()
                {
                    Type = WelcomeItemType.BankId
                }
            };
        }

        public override void Navigated()
        {
            base.Navigated();

            if (Settings.IsNotFirstLaunch)
            {
                CarouselItems = new ObservableCollection<CarouselItem>()
                {
                    new CarouselItem()
                    {
                        Type = WelcomeItemType.Login
                    }
                };
            }
        }

        private async System.Threading.Tasks.Task Next()
        {
            if (_selectedPageIndex < CarouselItems.Count - 1)
            {
                SelectedPage = CarouselItems[++_selectedPageIndex];
                _emailSignInNumberOfTaps = 0;
            }
        }

        private async System.Threading.Tasks.Task OpenBankId()
        {
            userDialogs.ShowLoading(AppResources.LoadingString);

            bool ok;
            string opaquePnum, firstName, lastName;
            try
            {
                (var t, var r) = await dao.RequestBankIdAutostartToken();

                ok = DependencyService.Get<IBankId>().StartBankIdWithoutPNum(t);
                //ok = true;
                if (ok)
                {
                    // NOTE: TaskCanceledException may happen on iOS when app comes back after backgrounding, cf. https://github.com/xamarin/xamarin-macios/issues/6443
                    for (var i = 0; true; i++)
                    {
                        try
                        {
                            (_, opaquePnum, firstName, lastName) = await dao.BankIdSignin(r);
                        }
                        catch (Exception) when (i < 4) // TaskCanceledException
                        {
                            await System.Threading.Tasks.Task.Delay(1000);
                            continue;
                        }

                        break;
                    }
                }
                else
                {
                    ok = false;
                    opaquePnum = null;
                    firstName = null;
                    lastName = null;

                    // TODO: Use pnum.
                    userDialogs.Toast(AppResources.BankIdNotInstalledErrorString);
                }
            }
            catch (Exception ex)
            {
                // TODO: Show error.
                ok = false;
                opaquePnum = null;
                firstName = null;
                lastName = null;

                userDialogs.HideLoading();

                Crashes.TrackError(ex);
                await userDialogs.AlertAsync("BankId sign in failed");
            }

            if (ok)
            {
                if (opaquePnum != null)
                {
                    userDialogs.HideLoading();
                    await view.OpenBankIdSignUp(firstName, lastName, opaquePnum);
                }
                else
                {
                    // Signed in. Now wait for first user poll to complete.
                    await dao.IsSignedIn();
                    Settings.IsNotFirstLaunch = true;
                    MessagingCenter.Send(new AuthMessage(), AppConstants.UserLoggedInEventMessage);

                    var u = dao.ActiveUser;

                    await Settings.SetSessionTimeoutTime(
                        DateTime.UtcNow.AddMilliseconds(
                            AppConstants.SessionTimeoutDuration));
                    
                    // register for notifications
                    await RegisterForNotifications();

                    // register for ad tracking
                    if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        await _appTrackingService.RequestTrackingPermission();
                    }

                    userDialogs.HideLoading();
                    await view.MoveToMain();
                }
            }
            else
            {
                userDialogs.HideLoading();
            }
        }


        public async System.Threading.Tasks.Task MoveToEmailSignIn()
        {
            if (++_emailSignInNumberOfTaps >= 3)
            {
                _emailSignInNumberOfTaps = 0;
                await view.MoveToEmailSignIn();
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