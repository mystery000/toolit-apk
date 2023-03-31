using System.Collections.Generic;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTaskDescription : ContentPage, AddTaskDescriptionViewModel.ICallback
    {
        public AddTaskDescription(CraftLayoutModel craftMdl, IList<MediaFile> media)
        {
            BindingContext = new AddTaskDescriptionViewModel(this, craftMdl, media);
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

        public async System.Threading.Tasks.Task MoveToLocation(CraftLayoutModel craftMdl, IList<MediaFile> media, Task newTask)
        {
            await Shell.Current.Navigation.PushAsync(new AddTaskLocationPage(craftMdl, media, newTask));
        }
    }
}