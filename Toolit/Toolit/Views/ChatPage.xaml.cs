using System.Linq;
using DentMeApp.Shared.Contracts.Services;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Misc;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage, ChatViewModel.ICallback
    {
        private readonly IKeyboardService _keyboardService;
        
        public ChatPage(ChatUiModel chat, string recipientId)
        {
            BindingContext = new ChatViewModel(this, chat, recipientId);
            InitializeComponent();

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                _keyboardService = DependencyService.Get<IKeyboardService>();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                _keyboardService.KeyboardWillShow += KeyboardServiceOnKeyboardWillShow;
                _keyboardService.KeyboardWillHide += KeyboardServiceOnKeyboardWillHide;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                _keyboardService.KeyboardWillShow -= KeyboardServiceOnKeyboardWillShow;
                _keyboardService.KeyboardWillHide -= KeyboardServiceOnKeyboardWillHide;
            }
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
        
        public async System.Threading.Tasks.Task OpenFullImage(ImageSource source)
        {
            await PopupNavigation.Instance.PushAsync(new FullImagePopup(source));
        }

        
        private async void KeyboardServiceOnKeyboardWillShow(object sender, KeyboardEventArgs e)
        {
            // raise the container to keep the entry in sight
            ChatContainer.Margin = new Thickness(0, 68, 0, e.KeyboardHeight);
        }

        private async void KeyboardServiceOnKeyboardWillHide(object sender, KeyboardEventArgs e)
        {
            ChatContainer.Margin = new Thickness(0, 68, 0, 96);
        }

    }
}