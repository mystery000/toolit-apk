using System.Collections.Generic;
using System.IO;
using Rg.Plugins.Popup.Services;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.ViewModels;
using Toolit.Views.Popups;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddTaskMediaPage : ContentPage, AddTaskMediaViewModel.ICallback
    {
        public AddTaskMediaPage(CraftLayoutModel craftMdl)
        {
            BindingContext = new AddTaskMediaViewModel(this, craftMdl);
            InitializeComponent();
        }

        public void CameraViewShutter()
        {
            CameraView.Shutter();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
        
        public async System.Threading.Tasks.Task Skip()
        {
             Application.Current.MainPage = new AppShell();
        }

        public async System.Threading.Tasks.Task MoveToWorkTypes(CraftLayoutModel craftMdl, IList<MediaFile> media)
        {
            await Shell.Current.Navigation.PushAsync(new AddTaskWorkTypesPage(craftMdl, media));
        }

        public async System.Threading.Tasks.Task OpenFullImage(ImageSource source)
        {
            await PopupNavigation.Instance.PushAsync(new FullImagePopup(source));
        }

        public async System.Threading.Tasks.Task OpenFullVideo(MediaSource video)
        {
            await PopupNavigation.Instance.PushAsync(new FullVideoPopup(video));
        }

        // EventToCommandBehavior can't handle non-EventArgs event params, so we must use a handler in code-behind
        private void CameraView_OnOnAvailable(object sender, bool e)
        {
            (BindingContext as AddTaskMediaViewModel)?.CameraViewAvailableCommand.Execute(e);
        }

        private async void CameraView_OnMediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            if (e.Video != null)
            {
                (BindingContext as AddTaskMediaViewModel)?.CameraViewMediaCapturedCommand.Execute(
                    new MediaFile()
                    {
                        Source = ImageSource.FromStream(() =>
                            new FileStream(e.Video.File, FileMode.Open, FileAccess.Read)) as StreamImageSource,
                        MediaSource = e.Video,
                        Extension = Path.GetExtension(e.Video.File), 
                        Path = e.Video.File,
                        IsVideo = true
                    });
            }
            else
            {
                (BindingContext as AddTaskMediaViewModel)?.CameraViewMediaCapturedCommand.Execute(
                    new MediaFile()
                        {Source = e.Image as StreamImageSource, Extension = ".jpeg"});
            }
        }
    }
}