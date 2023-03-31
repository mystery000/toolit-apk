using Rg.Plugins.Popup.Services;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage, WelcomeViewModel.ICallback
    {
        public WelcomePage()
        {
            BindingContext = new WelcomeViewModel(this);
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            // disable back button
            return true;
        }

        public async System.Threading.Tasks.Task MoveToMain()
        {
            await Shell.Current.GoToAsync(state: "//main");
        }
        
        public async System.Threading.Tasks.Task MoveToEmailSignIn()
        {
            await Shell.Current.Navigation.PushAsync(new EmailSignInPage());
        }

        public async System.Threading.Tasks.Task OpenBankIdSignUp(string firstName, string lastName, string opaquePnum)
        {
            await PopupNavigation.Instance.PushAsync(new BankIdSignUpPopup(firstName, lastName, opaquePnum));
        }
    }
}