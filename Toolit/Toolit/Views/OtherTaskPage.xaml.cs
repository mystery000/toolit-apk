using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherTaskPage : ContentPage, OtherTaskViewModel.ICallback
    {
        public OtherTaskPage(string taskId)
        {
            BindingContext = new OtherTaskViewModel(this, taskId);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task EditMessage(string taskId, string bidId)
        {
            await Shell.Current.Navigation.PushAsync(new CraftsmanEditMessagePage(taskId, bidId));
        }

        public async System.Threading.Tasks.Task EditOffer(string taskId, string bidId, decimal brokerageFee)
        {
            await Shell.Current.Navigation.PushAsync(new CraftsmanEditOfferPage(taskId, bidId, brokerageFee));
        }

        public async System.Threading.Tasks.Task DeleteBid(string taskId, string bidId)
        {
            await PopupNavigation.Instance.PushAsync(new DeleteBidPopup(taskId, bidId));
        }

        public async System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId)
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(chat, recipientId));
        }

        public async System.Threading.Tasks.Task ShowAddedBidConfirmation()
        {
            await PopupNavigation.Instance.PushAsync(new NewBidAddedConfirmationPopup());
        }

        public async System.Threading.Tasks.Task OpenFullImage(string url)
        {
            await PopupNavigation.Instance.PushAsync(new FullImagePopup(url));
        }
        
        public async System.Threading.Tasks.Task OpenFullVideo(string url)
        {
            await PopupNavigation.Instance.PushAsync(new FullVideoPopup(url));
        }
    }
}