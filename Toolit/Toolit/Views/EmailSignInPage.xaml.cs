using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmailSignInPage : ContentPage, EmailSignInViewModel.ICallback
    {
        public EmailSignInPage()
        {
            BindingContext = new EmailSignInViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task MoveToMain()
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync(state: "//main");
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}