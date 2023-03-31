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
    public partial class DeleteTaskPopup : PopupPage, DeleteTaskPopupViewModel.ICallback
    {
        public DeleteTaskPopup(string taskId)
        {
            BindingContext = new DeleteTaskPopupViewModel(this, taskId);
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

        public async System.Threading.Tasks.Task FullyClose()
        {
            await Shell.Current.GoToAsync("../../");
            await PopupNavigation.Instance.PopAsync();
        }

        public async System.Threading.Tasks.Task Close()
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}