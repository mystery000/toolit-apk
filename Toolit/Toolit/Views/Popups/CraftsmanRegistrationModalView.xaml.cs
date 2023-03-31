using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Toolit.ViewModels.Popups;
using Toolit.Views.Craftsman;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanRegistrationModalView : PopupPage, CraftsmanRegistrationModalViewModel.ICallback
    {
        public CraftsmanRegistrationModalView(bool isACraftsman)
        {
            BindingContext = new CraftsmanRegistrationModalViewModel(this, isACraftsman);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Close()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async System.Threading.Tasks.Task MoveToApply()
        {
            await PopupNavigation.Instance.PopAsync();
            await Shell.Current.Navigation.PushAsync(new CraftsmanApplyTypePage());
        }

        public async System.Threading.Tasks.Task MoveToApplyNewCraft()
        {
            await PopupNavigation.Instance.PopAsync();
            await Shell.Current.Navigation.PushAsync(new CraftsmanApplyNewCraftPage());
        }
    }
}