using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Toolit.Interfaces;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AddTaskLocationViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task OpenModal();
        }

        private readonly ICallback view;
        private readonly IThumbnailService _thumbnailService;

        public CraftLayoutModel CraftMdl { get; }
        public IList<MediaFile> Media { get; }
        public Task NewTask { get; }

        public ValidatableGroup HouseForm { get; } = new ValidatableGroup();
        public ValidatableGroup CondominiumForm { get; } = new ValidatableGroup();
        public ValidatableObject<string> Address { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Postcode { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> City { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> HousePropertyDesignation { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> CondominiumHousingName { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> CondominiumApartmentNumber { get; } = new ValidatableObject<string>();

        public ICommand MoveToShellCommand { get; private set; }
        public ICommand SubmitTaskCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ToggleRotRutYesBtnCommand { get; set; }
        public ICommand ToggleRotRutNoBtnCommand { get; set; }
        public ICommand ToggleHouseBtnCommand { get; set; }
        public ICommand ToggleCondominiumBtnCommand { get; set; }
        public ICommand ToggleSaveBtnCommand { get; set; }

        public bool saved;

        public bool Saved
        {
            get => saved;
            set
            {
                saved = value;
                OnPropertyChanged();
            }
        }

        private bool _isRotRut;
        public bool IsRotRut
        {
            get => _isRotRut;
            set
            {
                _isRotRut = value;
                OnPropertyChanged();
            }
        }
        
        private bool _isCondominium;
        public bool IsCondominium
        {
            get => _isCondominium;
            set
            {
                _isCondominium = value;
                OnPropertyChanged();
            }
        }

        public bool _isPublic;

        public bool IsPublic
        {
            get => _isPublic;
            set
            {
                _isPublic = value;
                OnPropertyChanged();
            }
        }

        public AddTaskLocationViewModel(ICallback view, CraftLayoutModel craftMdl,
            IList<MediaFile> media, Task newTask)
        {
            this.view = view;

            _thumbnailService = DependencyService.Get<IThumbnailService>();
            
            CraftMdl = craftMdl;
            Media = media;
            NewTask = newTask;

            BackCommand = new Command(Back);
            SubmitTaskCommand = new AsyncCommand(SubmitTask);
            ToggleRotRutYesBtnCommand = new Command(ToggleRotRutYesBtn);
            ToggleRotRutNoBtnCommand = new Command(ToggleRotRutNoBtn);
            ToggleHouseBtnCommand = new Command(ToggleHouseBtn);
            ToggleCondominiumBtnCommand = new Command(ToggleCondominiumBtn);
            ToggleSaveBtnCommand = new Command(ToggleSaveBtn);

            Address.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "YourAddressCannotBeEmpty"});
            Postcode.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "PostNumberCannotBeEmpty"});
            City.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = "CityCannotBeEmpty"});
            HousePropertyDesignation.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
            CondominiumHousingName.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
            CondominiumApartmentNumber.Validations.Add(new IsNotNullOrEmptyRule<string> {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
            
            HouseForm.Add(new IIsValid[] {Address, Postcode, City, HousePropertyDesignation});
            CondominiumForm.Add(new IIsValid[] {Address, Postcode, City, CondominiumHousingName, CondominiumApartmentNumber});
        }

        public override void Navigated()
        {
            base.Navigated();

            var savedAddress = Settings.SavedAddress;
            if (savedAddress != null)
            {
                Address.Value = savedAddress.Address;
                Postcode.Value = savedAddress.Postcode;
                City.Value = savedAddress.City;

                Saved = true;
            }

            IsRotRut = true;
        }

        public async void Back()
        {
            await view.Back();
        }

        public async System.Threading.Tasks.Task SubmitTask()
        {
            if ((!IsCondominium && HouseForm.IsValid) || (IsCondominium && CondominiumForm.IsValid))
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    NewTask.Address = Address.Value;
                    NewTask.Postcode = Postcode.Value;
                    NewTask.City = City.Value;

                    NewTask.PropertyDesignation = HousePropertyDesignation.Value ?? string.Empty;
                    NewTask.RealestateUnion = CondominiumHousingName.Value ?? string.Empty;
                    NewTask.ApartmentNumber = CondominiumApartmentNumber.Value ?? string.Empty;

                    NewTask.UseRotRut = IsRotRut;
                    NewTask.ShowPublicly = IsPublic;

                    NewTask.Crafts = new[] {CraftMdl.ServerId};
                    NewTask.Tags = CraftMdl.Tags
                        .Where(tg => tg.IsSelected)
                        .Select(tg => tg.ServerId).ToArray();
                    NewTask.UserId = dao.ActiveUser.Id;
                    NewTask.OfficeId = Settings.ActiveOffice;

                    NewTask.Modified = DateTime.Now.Normalize();
                    NewTask.DateDone = DateTime.MinValue.ToRFC3339();

                    NewTask.Images = new string[] { };
                    NewTask.Videos = new string[] { };

                    NewTask.Finished = false;
                    NewTask.Rated = false;
                    NewTask.AcceptedBid = null;
                    NewTask.PaymentId = string.Empty;

                    NewTask.PublishStatus = PublishStatus.Published;

                   var imageStreams = new List<(Stream Stream, string Extension)>();
                   var videoStreams = new List<(Stream Stream, string Extension)>();
                    
                    foreach (var mediaFile in Media)
                    {
                        if (mediaFile.IsVideo)
                        {
                            videoStreams.Add((
                                await mediaFile.Source.Stream(new CancellationToken()),
                                mediaFile.Extension.TrimStart('.')
                            ));
                        }
                        else
                        {
                            imageStreams.Add((
                                await mediaFile.Source.Stream(new CancellationToken()),
                                mediaFile.Extension.TrimStart('.')
                            ));
                        }
                    }

                    if (videoStreams.Any())
                    {
                        // generate video thumbnail
                        if (_thumbnailService.GenerateVideoThumbnail(Media
                            .First(mf => mf.IsVideo).Path) is StreamImageSource thumbStream)
                        {
                            imageStreams.Insert(0, (await thumbStream.Stream(new CancellationToken()), ".png"));
                        }
                    }

                    var newApiTask = await dao.Add(NewTask, imageStreams.ToArray(), videoStreams.ToArray());

                    foreach (var imageStream in imageStreams)
                    {
                        imageStream.Stream.Dispose();
                    }
                    foreach (var videoStream in videoStreams)
                    {
                        videoStream.Stream.Dispose();
                    }

                    if (Saved)
                    {
                        Settings.SavedAddress = new AddressSettingsModel()
                        {
                            Address = Address.Value,
                            Postcode = Postcode.Value,
                            City = City.Value
                        };
                    }
                    
                    userDialogs.HideLoading();
                    await view.OpenModal();
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
            else
            {
                userDialogs.Toast(AppResources.PleaseFillAllFieldsErrorString);
            }
        }

        public void ToggleRotRutYesBtn()
        {
            IsRotRut = true;
        }

        public void ToggleRotRutNoBtn()
        {
            IsRotRut = false;
        }

        public void ToggleHouseBtn()
        {
            IsCondominium = false;
        }

        public void ToggleCondominiumBtn()
        {
            IsCondominium = true;
        }

        public void ToggleSaveBtn()
        {
            Saved = !Saved;
        }
    }
}