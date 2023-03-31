using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTaskPage : ContentPage, MyTaskViewModel.ICallback
    {
        public MyTaskPage(string taskId)
        {
            BindingContext = new MyTaskViewModel(this, taskId);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task MoveToBid(string taskId, string bidId)
        {
            await Shell.Current.Navigation.PushAsync(new BidPage(taskId, bidId));
        }
        
        public async System.Threading.Tasks.Task MoveToEditTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new EditTaskPage(taskId));
        }
        
        public async System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId)
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage(chat, recipientId));
        }

        public async System.Threading.Tasks.Task OpenRating(string taskId, string craftsmanId)
        {
            await PopupNavigation.Instance.PushAsync(new UserTaskCompletedPopup(taskId, craftsmanId));
        }

        public async System.Threading.Tasks.Task OpenFullImage(string url)
        {
            await PopupNavigation.Instance.PushAsync(new FullImagePopup(url));
        }

        public async System.Threading.Tasks.Task OpenFullVideo(string url)
        {
            await PopupNavigation.Instance.PushAsync(new FullVideoPopup(url));
        }
    }
}