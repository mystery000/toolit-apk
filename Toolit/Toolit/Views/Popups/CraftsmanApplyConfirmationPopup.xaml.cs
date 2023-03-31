using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Toolit.ViewModels.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanApplyConfirmationPopup : PopupPage, CraftsmanApplyConfirmationPopupViewModel.ICallback
    {
        private readonly bool _isNewCraft;

        public CraftsmanApplyConfirmationPopup(bool isNewCraft = false)
        {
            _isNewCraft = isNewCraft;
            
            BindingContext = new CraftsmanApplyConfirmationPopupViewModel(this);
            InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            (BindingContext as BaseViewModel)?.Navigated();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            (BindingContext as BaseViewModel)?.NavigatingFrom();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async System.Threading.Tasks.Task MoveToMain()
        {
            await PopupNavigation.Instance.PopAsync();
            if (_isNewCraft)
            {
                await Shell.Current.GoToAsync("../");
            }
            else
            {
                await Shell.Current.GoToAsync("../../../../");
            }
        }
    }
}