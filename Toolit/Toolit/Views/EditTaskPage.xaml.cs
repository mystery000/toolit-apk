using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditTaskPage : ContentPage, EditTaskViewModel.ICallback
    {
        public EditTaskPage(string taskId)
        {
            BindingContext = new EditTaskViewModel(this, taskId);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
        
        public async System.Threading.Tasks.Task DeleteTask(string taskId)
        {
            await PopupNavigation.Instance.PushAsync(new DeleteTaskPopup(taskId));
        }
    }
}