
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentsPage : ContentPage, PaymentsViewModel.ICallback
    {
        public PaymentsPage()
        {
            BindingContext = new PaymentsViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
        
        public async System.Threading.Tasks.Task MoveToMyTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new MyTaskPage(taskId));
        }

        public async System.Threading.Tasks.Task MoveToOtherTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new OtherTaskPage(taskId));
        }
    }
}