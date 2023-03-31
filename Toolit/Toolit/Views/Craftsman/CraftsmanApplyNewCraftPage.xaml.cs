using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.Craftsman
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanApplyNewCraftPage : ContentPage, CraftsmanApplyNewCraftViewModel.ICallback
    {
        public CraftsmanApplyNewCraftPage()
        {
            BindingContext = new CraftsmanApplyNewCraftViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task OpenApplyConfirmation()
        {
            await PopupNavigation.Instance.PushAsync(new CraftsmanApplyConfirmationPopup(true));
        }
    }
}