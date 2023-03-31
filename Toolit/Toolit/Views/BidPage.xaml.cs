using Rg.Plugins.Popup.Services;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BidPage : ContentPage, BidViewModel.ICallback
    {
        public BidPage(string taskId, string bidId)
        {
            BindingContext = new BidViewModel(this, taskId, bidId);
            InitializeComponent();
        }
        

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId)
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(chat, recipientId));
        }

        public async System.Threading.Tasks.Task OpenSwish(BidUiModel bid)
        {
            await PopupNavigation.Instance.PushAsync(new SwishPaymentRequestPopup(bid));
        }

        public async System.Threading.Tasks.Task MoveToToS()
        {
            await Shell.Current.Navigation.PushAsync(new ToSPage());
        }
    }
}