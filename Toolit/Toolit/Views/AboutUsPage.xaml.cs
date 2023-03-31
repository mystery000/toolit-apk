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
    public partial class AboutUsPage : ContentPage, AboutUsViewModel.ICallback
    {
        public AboutUsPage()
        {
            BindingContext = new AboutUsViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}