using Toolit.Models.Layouts;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTaskTipsPage : ContentPage, AddTaskTipsViewModel.ICallback
    {
        public AddTaskTipsPage(CraftLayoutModel craftMdl)
        {
            BindingContext = new AddTaskTipsViewModel(this, craftMdl);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task Skip()
        {
            Application.Current.MainPage = new AppShell();
        }

        public async System.Threading.Tasks.Task MoveToMedia(CraftLayoutModel craftMdl)
        {
            await Shell.Current.Navigation.PushAsync(new AddTaskMediaPage(craftMdl));
        }
    }
}