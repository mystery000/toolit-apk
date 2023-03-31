using Rg.Plugins.Popup.Services;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanApplyInfoPage : ContentPage, CraftsmanApplyInfoViewModel.ICallback
    {
        public CraftsmanApplyInfoPage(CraftsmanUiModel newCraftsmanModel)
        {
            BindingContext = new CraftsmanApplyInfoViewModel(this, newCraftsmanModel);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task OpenApplyConfirmation()
        {
            await PopupNavigation.Instance.PushAsync(new CraftsmanApplyConfirmationPopup());
        }
    }
}