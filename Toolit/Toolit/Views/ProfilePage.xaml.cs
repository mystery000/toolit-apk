using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Toolit.ViewModels;
using Toolit.Views.ContentViews;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage, ProfileViewModel.ICallback
    {
        public ProfilePage()
        {
            BindingContext = new ProfileViewModel(this);
            InitializeComponent();
        }
        
        public async System.Threading.Tasks.Task MoveToEditProfile()
        {
            await Shell.Current.Navigation.PushAsync(new EditProfilePage());
        }
        
        public async System.Threading.Tasks.Task MoveToEditCraftsmanProfile(string officeId, string craftsmanId)
        {
            await Shell.Current.Navigation.PushAsync(new EditCraftsmanProfilePage(officeId, craftsmanId));
        }
        
        public async System.Threading.Tasks.Task MoveToEditCraftsmanBio(string officeId, string craftsmanId)
        {
            await Shell.Current.Navigation.PushAsync(new CraftsmanEditBioPage(officeId, craftsmanId));
        }

        public async System.Threading.Tasks.Task OpenDeleteAccountPopup()
        {
            await PopupNavigation.Instance.PushAsync(new DeleteAccountModalView());
        }
        
        public async System.Threading.Tasks.Task OpenCraftsmanRegistrationPopup(bool isACraftsman)
        {
            await PopupNavigation.Instance.PushAsync(new CraftsmanRegistrationModalView(isACraftsman));
        }

        public async System.Threading.Tasks.Task MoveToMyTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new MyTaskPage(taskId));
        }

        public async System.Threading.Tasks.Task MoveToOtherTask(string taskId)
        {
            await Shell.Current.Navigation.PushAsync(new OtherTaskPage(taskId));
        }
        
        public async System.Threading.Tasks.Task ResetToWelcome()
        {
            await Shell.Current.GoToAsync(state: "//login");
        }
        public async System.Threading.Tasks.Task MoveToPayments()
        {
            await Shell.Current.Navigation.PushAsync(new PaymentsPage());
        }

        public async System.Threading.Tasks.Task MoveToAboutUs()
        {
            await Shell.Current.Navigation.PushAsync(new AboutUsPage());
        }
        
        public async System.Threading.Tasks.Task MoveToToS()
        {
            await Shell.Current.Navigation.PushAsync(new ToSPage());
        }
        
        public async System.Threading.Tasks.Task MoveToFAQ()
        {
            await Shell.Current.Navigation.PushAsync(new FAQPage());
        }
        public async System.Threading.Tasks.Task MoveToDataPolicy()
        {
            await Shell.Current.Navigation.PushAsync(new DataPolicyPage());
        }
    }
}
