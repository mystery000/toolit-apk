using System.Collections.Generic;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTaskWorkTypesPage : ContentPage, AddTaskWorkTypesViewModel.ICallback
    {
        public AddTaskWorkTypesPage(CraftLayoutModel craftMdl, IList<MediaFile> media)
        {
            BindingContext = new AddTaskWorkTypesViewModel(this, craftMdl, media);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task Skip()
        {
            Application.Current.MainPage = new AppShell();
        }

        public async System.Threading.Tasks.Task MoveToDescription(CraftLayoutModel craftMdl, IList<MediaFile> media)
        {
            await Shell.Current.Navigation.PushAsync(new AddTaskDescription(craftMdl, media));
        }
    }
}