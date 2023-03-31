using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Toolit.ViewModels;
using Toolit.ViewModels.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullImagePopup : PopupPage, FullImagePopupViewModel.ICallback
    {
        public FullImagePopup(ImageSource source)
        {
            BindingContext = new FullImagePopupViewModel(this, source);
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


        public async System.Threading.Tasks.Task Close()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}