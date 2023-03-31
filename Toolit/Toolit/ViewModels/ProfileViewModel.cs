using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Toolit.Helpers;
using Toolit.Mappers;
using Toolit.Models.Misc;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToEditProfile();
            System.Threading.Tasks.Task MoveToEditCraftsmanProfile(string officeId, string craftsmanId);
            System.Threading.Tasks.Task MoveToEditCraftsmanBio(string officeId, string craftsmanId);
            System.Threading.Tasks.Task OpenCraftsmanRegistrationPopup(bool isACraftsman);
            System.Threading.Tasks.Task OpenDeleteAccountPopup();
            System.Threading.Tasks.Task ResetToWelcome();
            System.Threading.Tasks.Task MoveToAboutUs();
            System.Threading.Tasks.Task MoveToPayments();
            System.Threading.Tasks.Task MoveToToS();
            System.Threading.Tasks.Task MoveToFAQ();
            System.Threading.Tasks.Task MoveToDataPolicy();
            System.Threading.Tasks.Task MoveToMyTask(string taskId);
            System.Threading.Tasks.Task MoveToOtherTask(string taskId);
        }

        private readonly ICallback view;

        private ObservableCollection<PaymentUiModel> _paymentsList;
        public ObservableCollection<PaymentUiModel> PaymentsList
        {
            get => _paymentsList;
            set
            {
                _paymentsList = value;
                OnPropertyChanged();
            }
        }
        
        private ObservableCollection<RatingUiModel> ratings;
        public ObservableCollection<RatingUiModel> Ratings
        {
            get => ratings;
            set
            {
                ratings = value;
                OnPropertyChanged();
            }
        }
        

        private bool _isACraftsman;
        public bool IsACraftsman
        {
            get => _isACraftsman;
            set
            {
                _isACraftsman = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isSettingsTabSelected;

        public bool IsSettingsTabSelected
        {
            get => _isSettingsTabSelected;
            set
            {
                _isSettingsTabSelected = value;
                OnPropertyChanged();
            }
        }

        private UserUiModel _user;
        public UserUiModel User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        private CraftsmanUiModel _craftsman;
        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand ShowInfoCommand { get; private set; }
        public ICommand ShowSettingsCommand { get; private set; }
        public ICommand DeleteAccountCommand { get; private set; }
        public ICommand OpenCraftsmanRegistrationPopupCommand { get; }
        public ICommand MoveToEditProfileCommand { get; private set; }
        public ICommand MoveToPaymentTaskCommand { get; private set; }
        public ICommand MoveToEditCraftsmanBioCommand { get; private set; }
        public ICommand LogOutCommand { get; private set; }
        public ICommand MoveToAboutUsCommand { get; private set; }
        public ICommand ContactUsCommand { get; private set; }
        public ICommand MoveToPaymentsCommand { get; private set; }
        public ICommand MoveToToSCommand { get; private set; }
        public ICommand MoveToDataPolicyCommand { get; private set; }
        public ICommand MoveToFAQCommand { get; private set; }
        public ICommand EditProfilePictureCommand { get; private set; }

        public ProfileViewModel(ICallback view)
        {
            IsATab = true;
            this.view = view;
            
            ShowInfoCommand = new Command(ShowInfo);
            ShowSettingsCommand = new Command(ShowSettings);
            DeleteAccountCommand = new AsyncCommand(DeleteAccount);
            OpenCraftsmanRegistrationPopupCommand = new AsyncCommand(OpenCraftsmanRegistrationPopup);
            MoveToEditProfileCommand = new Command(MoveToEditProfile);
            MoveToPaymentTaskCommand = new AsyncCommand<PaymentUiModel>(MoveToPaymentTask);
            MoveToEditCraftsmanBioCommand = new AsyncCommand(MoveToEditCraftsmanBio);
            LogOutCommand = new AsyncCommand(LogOut);
            MoveToAboutUsCommand = new Command(MoveToAboutUs);
            ContactUsCommand = new Command(ContactUs);
            MoveToPaymentsCommand = new Command(MoveToPayments);
            MoveToToSCommand = new Command(MoveToToS);
            MoveToFAQCommand = new Command(MoveToFAQ);
            MoveToDataPolicyCommand = new Command(MoveToDataPolicy);
            EditProfilePictureCommand = new Command(EditProfilePicture);
        }

        public override async void Navigated()
        {
            base.Navigated();
            
            IsACraftsman = false;
            IsSettingsTabSelected = true;
            
            User = dao.ActiveUser.ToUserUiModel();
            try
            {
                Craftsman = (await dao.GetCraftsman(Settings.ActiveOffice, dao.ActiveUser.Id)).ToCraftsmanUiModel();
                Ratings = new ObservableCollection<RatingUiModel>(Craftsman.Ratings);

                foreach (var rating in Ratings)
                {
                    rating.User = (await dao.GetUser(rating.UserId)).ToUserUiModel();
                    rating.Craftsman = Craftsman;
                }

                // if no exception, then can move on
                IsACraftsman = !Craftsman.IsDeleted && Craftsman.Crafts.Any(crft => crft.Status != CraftStatus.Rejected);
            }
            catch (Exception ex)
            {
                IsACraftsman = false;
                IsSettingsTabSelected = true;
            }
            
            if (!IsSubscribedToDao)
            {
                dao.Subscribe(HandlePaymentSuccess, HandleError);
                IsSubscribedToDao = true;
            }
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
            
            if (IsSubscribedToDao)
            {
                dao.Unsubscribe(HandlePaymentSuccess, HandleError);
                IsSubscribedToDao = false;
            }
        }
        

        private async void HandlePaymentSuccess(Payment[] data, string nonce, DateTime updated)
        {
            try
            {
                PaymentsList = new ObservableCollection<PaymentUiModel>((await System.Threading.Tasks.Task.WhenAll(
                    data.Select(async pmnt =>
                {
                    var uiMdl = pmnt.ToPaymentUiModel();

                    try
                    {
                        uiMdl.Task = (await dao.GetTask(pmnt.OfficeId, pmnt.TaskId)).ToTaskUiModel();
                        uiMdl.Bid = (await dao.GetBid(pmnt.OfficeId, pmnt.TaskId, pmnt.BidId)).ToBidUiModel();
                        uiMdl.Craftsman = (await dao.GetCraftsman(pmnt.OfficeId, pmnt.CraftsmanId)).ToCraftsmanUiModel();
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex, ex.Message);
                    }
                    
                    return uiMdl;
                }))).OrderByDescending(pmnt => pmnt.Modified));
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
        
        public void ShowInfo()
        {
            IsSettingsTabSelected = false;
        }

        public void ShowSettings()
        {
            IsSettingsTabSelected = true;
        }

        public async System.Threading.Tasks.Task DeleteAccount()
        {
            await view.OpenDeleteAccountPopup();
        }
        
        public async System.Threading.Tasks.Task OpenCraftsmanRegistrationPopup()
        {
            await view.OpenCraftsmanRegistrationPopup(IsACraftsman);
        }

        public async void MoveToEditProfile()
        {
            userDialogs.ShowLoading(AppResources.LoadingString);
            if (!IsACraftsman)
            {
                await view.MoveToEditProfile();
            }
            else
            {
                await view.MoveToEditCraftsmanProfile(Settings.ActiveOffice, Craftsman.Id);
            }
        }
        
        private async System.Threading.Tasks.Task MoveToPaymentTask(PaymentUiModel mdl)
        {
            if (mdl?.Task != null)
            {
                if (mdl.Task.UserId.Equals(dao.ActiveUser?.Id))
                {
                    await view.MoveToMyTask(mdl.Task.Id);
                }
                else
                {
                    await view.MoveToOtherTask(mdl.Task.Id);
                }
            }
        }
        
        public async System.Threading.Tasks.Task MoveToEditCraftsmanBio()
        {
            if (IsACraftsman)
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                await view.MoveToEditCraftsmanBio(Settings.ActiveOffice, Craftsman.Id);
            }
        }
        
        public async System.Threading.Tasks.Task LogOut()
        { 
            dao.SignOut();
            await Settings.SetSessionTimeoutTime(DateTime.MinValue);
            MessagingCenter.Send(new AuthMessage(), AppConstants.UserLoggedOutEventMessage);

            await view.ResetToWelcome();
        }
        
        public async void MoveToAboutUs()
        {
            await view.MoveToAboutUs();
        }
        
        public async void ContactUs()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitContactUrl);
        }

        public async void MoveToPayments()
        {
            userDialogs.ShowLoading(AppResources.LoadingString);
            await view.MoveToPayments();
        }

        public async void MoveToToS()
        {
            await view.MoveToToS();
        }
        
        public async void MoveToFAQ()
        {
            await view.MoveToFAQ();
        }
        
        public async void MoveToDataPolicy()
        {
            await view.MoveToDataPolicy();
        }
        
        public async void EditProfilePicture()
        {
            try
            {
                var newImage = await MediaPicker.PickPhotoAsync();

                if (newImage != null)
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);
                    User = (await dao.AddImage(dao.ActiveUser, await newImage.OpenReadAsync(),
                        Path.GetExtension(newImage.FullPath).TrimStart('.'))).ToUserUiModel();

                    userDialogs.HideLoading();
                }
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }
    }
}
