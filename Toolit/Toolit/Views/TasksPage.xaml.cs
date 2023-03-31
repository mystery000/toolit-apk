using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolit.Controls;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : DarkStatusBarPage, TasksViewModel.ICallback
    {
        public TasksPage()
        {
            BindingContext = new TasksViewModel(this);
            InitializeComponent();
        }
        
        public async System.Threading.Tasks.Task MoveToMyTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new MyTaskPage(taskId));
        }

        public async System.Threading.Tasks.Task MoveToOtherTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new OtherTaskPage(taskId));
        }
        
        public async System.Threading.Tasks.Task MoveToAd(AdUiModel ad)
        {
            await Shell.Current.Navigation.PushAsync(new AdPage(ad));
        }
    }
}