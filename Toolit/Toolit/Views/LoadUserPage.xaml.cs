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
    public partial class LoadUserPage : ContentPage, LoadUserViewModel.ICallback
    {
        public LoadUserPage()
        {
            BindingContext = new LoadUserViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task MoveToMain()
        {
            await Shell.Current.GoToAsync(state: "//main");
        }

        public async System.Threading.Tasks.Task MoveToWelcome()
        {
            await Shell.Current.Navigation.PushAsync(new WelcomePage());
        }
    }
}