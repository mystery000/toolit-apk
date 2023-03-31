using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Toolit.ViewModels;
using Toolit.ViewModels.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserPaymentConfirmationPopup : PopupPage, UserPaymentConfirmationPopupViewModel.ICallback
    {
        public UserPaymentConfirmationPopup()
        {
            BindingContext = new UserPaymentConfirmationPopupViewModel(this);
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
            await Shell.Current.GoToAsync("../");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}