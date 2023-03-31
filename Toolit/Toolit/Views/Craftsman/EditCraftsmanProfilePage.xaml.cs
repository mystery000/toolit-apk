using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCraftsmanProfilePage : ContentPage, EditCraftsmanProfileViewModel.ICallback
    {
        public EditCraftsmanProfilePage(string officeId, string craftsmanId)
        {
            BindingContext = new EditCraftsmanProfileViewModel(this, officeId, craftsmanId);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}