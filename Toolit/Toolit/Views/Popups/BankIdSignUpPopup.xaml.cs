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
    public partial class BankIdSignUpPopup : PopupPage, BankIdSignUpPopupViewModel.ICallback
    {
        public BankIdSignUpPopup(string firstName, string lastName, string opaquePnum)
        {
            BindingContext = new BankIdSignUpPopupViewModel(this, firstName, lastName, opaquePnum);
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

        protected override bool OnBackButtonPressed()
        {
            // disable back button
            return true;
        }

        public async System.Threading.Tasks.Task MoveToMain()
        {
            await Shell.Current.GoToAsync(state: "//main");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}