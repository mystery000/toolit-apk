using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Toolit.ViewModels.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeleteAccountModalView : PopupPage, DeleteAccountModalViewModel.ICallback
    {
        public DeleteAccountModalView()
        {
            BindingContext = new DeleteAccountModalViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Close()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async System.Threading.Tasks.Task ResetToWelcome()
        {
            await PopupNavigation.Instance.PopAsync();
            await Shell.Current.GoToAsync(state: "//login");
        }
    }
}