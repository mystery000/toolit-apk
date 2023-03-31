using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatsPage : ContentPage, ChatsViewModel.ICallback
    {
        public ChatsPage()
        {
            BindingContext = new ChatsViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId)
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(chat, recipientId));
        }
    }
}