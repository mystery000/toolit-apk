using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdPage : ContentPage, AdViewModel.ICallback
    {
        public AdPage(AdUiModel ad)
        {
            BindingContext = new AdViewModel(this, ad);
            InitializeComponent();
        }


        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}