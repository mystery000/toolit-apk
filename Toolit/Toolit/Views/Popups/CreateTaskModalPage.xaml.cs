using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Layouts;
using Toolit.ViewModels.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views.ContentViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateTaskModalPage : PopupPage, CreateTaskModalViewModel.ICallback
    {
        public CreateTaskModalPage(CraftLayoutModel craftMdl)
        {
            BindingContext = new CreateTaskModalViewModel(this, craftMdl);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Close()
        {
            await PopupNavigation.Instance.PopAsync();
        }
        
        public async System.Threading.Tasks.Task MoveToTips(CraftLayoutModel craftMdl)
        {
            await PopupNavigation.Instance.PopAsync();
            await Shell.Current.Navigation.PushAsync(new AddTaskTipsPage(craftMdl));
        }
    }
}