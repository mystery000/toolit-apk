using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanEditBioPage : ContentPage, CraftsmanEditBioViewModel.ICallback
    {
        public CraftsmanEditBioPage(string officeId, string craftsmanId)
        {
            BindingContext = new CraftsmanEditBioViewModel(this, officeId, craftsmanId);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}