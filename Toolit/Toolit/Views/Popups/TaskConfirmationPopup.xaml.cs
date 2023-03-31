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

namespace Toolit.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskConfirmationPopup : PopupPage, TaskConfirmationPopupViewModel.ICallback
    {
        public TaskConfirmationPopup()
        {
            BindingContext = new TaskConfirmationPopupViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task MoveToShell()
        {
            await PopupNavigation.Instance.PopAsync();
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}