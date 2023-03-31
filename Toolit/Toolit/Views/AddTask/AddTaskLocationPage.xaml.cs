using System.Collections.Generic;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTaskLocationPage : ContentPage, AddTaskLocationViewModel.ICallback
    {
        public AddTaskLocationPage(CraftLayoutModel craftMdl, IList<MediaFile> media, Task newTask)
        {
            BindingContext = new AddTaskLocationViewModel(this, craftMdl, media, newTask);
            InitializeComponent();
        }
        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task OpenModal()
        {
            await PopupNavigation.Instance.PushAsync(new TaskConfirmationPopup());
        }
    }
}