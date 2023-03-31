using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Plugin.Media;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AddTaskMediaViewModel : BaseViewModel
    {

        public interface ICallback
        {
            void CameraViewShutter();
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToWorkTypes(CraftLayoutModel craftMdl, 
                IList<MediaFile> media);
            System.Threading.Tasks.Task OpenFullImage(ImageSource source);
            System.Threading.Tasks.Task OpenFullVideo(MediaSource video);
        }

        private readonly ICallback view;
        
        private ObservableCollection<MediaFile> _selectedMedia;
        private bool _areCameraControlsEnabled;

        private int _photosAdded;
        private int _videosAdded;
        private bool _cameraViewCaptureMode;
        private bool _isCameraRolling;

        public CraftLayoutModel CraftMdl { get; }

        public bool AreCameraControlsEnabled
        {
            get => _areCameraControlsEnabled;
            set
            {
                _areCameraControlsEnabled = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MediaFile> SelectedMedia
        {
            get => _selectedMedia;
            set
            {
                _selectedMedia = value;
                OnPropertyChanged();
            }
        }

        public bool IsCameraViewCapturingVideo
        {
            get => _cameraViewCaptureMode;
            set
            {
                _cameraViewCaptureMode = value;
                OnPropertyChanged();
            }
        }

        public bool IsCameraRolling
        {
            get => _isCameraRolling;
            set
            {
                _isCameraRolling = value;
                OnPropertyChanged();
            }
        }

        public bool CanContinue => !IsCameraRolling && 
                                   SelectedMedia.Count >= AppConstants.MinTaskMedia &&
                                   _photosAdded <= AppConstants.MaxTaskMediaPhotos &&
                                   _videosAdded <= AppConstants.MaxTaskMediaVideos;
        
        public ICommand CameraViewAvailableCommand { get; private set; }
        public ICommand CameraViewMediaCapturedCommand { get; private set; }
        public ICommand OpenGalleryCommand { get; private set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand DeleteMediaCommand { get; private set; }
        public ICommand MoveToWorkTypesCommand { get; private set; }
        public ICommand ToggleCaptureModePhotoCommand { get; private set; }
        public ICommand ToggleCaptureModeVideoCommand { get; private set; }
        public ICommand OpenFullImageCommand { get; set; }
        public ICommand OpenFullVideoCommand { get; set; }
        public ICommand BackCommand { get; private set; }

        public AddTaskMediaViewModel(ICallback view, CraftLayoutModel craftMdl)
        {
            this.view = view;
            CraftMdl = craftMdl;
            
            CameraViewAvailableCommand = new AsyncCommand<bool>(CameraViewAvailable);
            CameraViewMediaCapturedCommand = new AsyncCommand<MediaFile>(CameraViewMediaCaptured);
            OpenGalleryCommand = new AsyncCommand(OpenGallery);
            DeleteMediaCommand = new AsyncCommand<MediaFile>(DeleteMedia);
            TakePhotoCommand = new AsyncCommand(TakePhoto);
            MoveToWorkTypesCommand = new Command(MoveToWorkTypes);
            ToggleCaptureModePhotoCommand = new AsyncCommand(ToggleCaptureModePhoto);
            ToggleCaptureModeVideoCommand = new AsyncCommand(ToggleCaptureModeVideo);
            OpenFullImageCommand = new AsyncCommand<MediaFile>(OpenFullImage);
            OpenFullVideoCommand = new AsyncCommand<MediaFile>(OpenFullVideo);
            BackCommand = new Command(Back);
            
            SelectedMedia = new ObservableCollection<MediaFile>();
        }

        private async System.Threading.Tasks.Task CameraViewAvailable(bool isAvailable)
        {
            if (isAvailable)
            {
                AreCameraControlsEnabled = true;
            }
            else
            {
                AreCameraControlsEnabled = false;
                userDialogs.Toast("Camera not available");
            }
        }

        private async System.Threading.Tasks.Task OpenGallery()
        {
            var mediaTypeDialogResult = await userDialogs.ActionSheetAsync(
                title: AppResources.AddTaskMediaTypeTitle,
                cancel: AppResources.CancelAction,
                destructive: null,
                buttons: new [] {AppResources.AddTaskMediaPhotoType, AppResources.AddTaskMediaVideoType});

            if (mediaTypeDialogResult.Equals(AppResources.AddTaskMediaPhotoType))
            {
                if (_photosAdded < AppConstants.MaxTaskMediaPhotos)
                {
                    var photo = await MediaPicker.PickPhotoAsync();
                    if (!string.IsNullOrEmpty(photo?.FullPath))
                    {
                        SelectedMedia.Add(new MediaFile{
                            Source = ImageSource.FromStream(() => photo.OpenReadAsync().GetAwaiter().GetResult()) as
                                StreamImageSource,
                            Extension = Path.GetExtension(photo.FullPath),
                            Path = photo.FullPath
                        });
                        
                        _photosAdded++;
                        OnPropertyChanged(nameof(CanContinue));
                    }
                }
            }
            else if (mediaTypeDialogResult.Equals(AppResources.AddTaskMediaVideoType))
            {
                if (_videosAdded < AppConstants.MaxTaskMediaVideos)
                {
                    var video = await CrossMedia.Current.PickVideoAsync();
                    if (!string.IsNullOrEmpty(video?.Path))
                    {
                        SelectedMedia.Add(new MediaFile
                        {
                            Source = ImageSource.FromStream(() => video.GetStream()) as
                                StreamImageSource,
                            Extension = Path.GetExtension(video.Path),
                            MediaSource = MediaSource.FromFile(video.Path),
                            Path = video.Path,
                            IsVideo = true
                        });
                        
                        _videosAdded++;
                        OnPropertyChanged(nameof(CanContinue));
                    }
                }
            }
        }
        
        private async System.Threading.Tasks.Task TakePhoto()
        {
            if (AreCameraControlsEnabled)
            {
                IsCameraRolling = true;
                view.CameraViewShutter();
            }
        }
        
        
        private async System.Threading.Tasks.Task DeleteMedia(MediaFile media)
        {
            SelectedMedia.Remove(media);
            if (media.IsVideo)
            {
                _videosAdded--;
            }
            else
            {
                _photosAdded--;
            }
            
            OnPropertyChanged(nameof(CanContinue));
        }

        private async System.Threading.Tasks.Task CameraViewMediaCaptured(MediaFile e)
        {
            if (!IsCameraViewCapturingVideo)
            {
                if (_photosAdded < AppConstants.MaxTaskMediaPhotos)
                {
                    SelectedMedia.Add(e);
                }
            }
            else
            {
                if (_videosAdded < AppConstants.MaxTaskMediaVideos)
                {
                    SelectedMedia.Add(e);
                }
            }

            IsCameraRolling = false;
            OnPropertyChanged(nameof(CanContinue));
        }

        public async void MoveToWorkTypes()
        {
            await view.MoveToWorkTypes(CraftMdl, SelectedMedia);
        }
        
        private async System.Threading.Tasks.Task ToggleCaptureModePhoto()
        {
            IsCameraViewCapturingVideo = false;
        }

        private async System.Threading.Tasks.Task ToggleCaptureModeVideo()
        {
            IsCameraViewCapturingVideo = true;
        }
        
        private async System.Threading.Tasks.Task OpenFullImage(MediaFile image)
        {
            await view.OpenFullImage(image.Source);
        }

        private async System.Threading.Tasks.Task OpenFullVideo(MediaFile video)
        {
            // TODO
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await view.OpenFullVideo(video.MediaSource);
            }
        }

        public async void Back()
        {
            await view.Back();
        }

    }
}
