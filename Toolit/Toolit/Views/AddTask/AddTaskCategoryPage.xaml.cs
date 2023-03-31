using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Layouts;
using Toolit.ViewModels;
using Toolit.Views.ContentViews;
using Toolit.Views.Craftsman;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTaskCategoryPage : ContentPage, AddTaskCategoryViewModel.ICallback
    {
        public AddTaskCategoryPage()
        {
            BindingContext = new AddTaskCategoryViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task MoveToToS()
        {
            await Shell.Current.Navigation.PushAsync(new ToSPage());
        }

        public async System.Threading.Tasks.Task OpenCreateTaskPopup(CraftLayoutModel craftMdl)
        {
            await PopupNavigation.Instance.PushAsync(new CreateTaskModalPage(craftMdl));
        }

        public async System.Threading.Tasks.Task OpenCraftsmanRegistrationPopup(bool isACraftsman)
        {
            await PopupNavigation.Instance.PushAsync(new CraftsmanRegistrationModalView(isACraftsman));
        }
    }
}

