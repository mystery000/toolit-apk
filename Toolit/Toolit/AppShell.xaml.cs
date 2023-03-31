using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolit.Extensions;
using Toolit.Interfaces;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            BindingContext = new AppShellViewModel();
            InitializeComponent();
        }
        
        
        void ShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            var currentViewModel = Current.GetCurrentViewModel();

            if(currentViewModel is INavigationHandler navHandler)
            {
                navHandler.NavigatingFrom();
            }
        }

        void ShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            var currentViewModel = Current.GetCurrentViewModel();

            if (currentViewModel is INavigationHandler navHandler)
            {
                navHandler.Navigated();
            }
        }
    }
}