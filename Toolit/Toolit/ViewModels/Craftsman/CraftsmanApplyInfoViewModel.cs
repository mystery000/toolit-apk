using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class CraftsmanApplyInfoViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task OpenApplyConfirmation();
        }

        private readonly ICallback view;

        private readonly CraftsmanUiModel _newCraftsmanModel;
        private FileResult _certificateFile;

        public ValidatableGroup Form { get; } = new ValidatableGroup();
        public ValidatableObject<string> AboutText { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> AboutHeader { get; } = new ValidatableObject<string>();


        public FileResult CertificateFile
        {
            get => _certificateFile;
            set
            {
                _certificateFile = value;
                OnPropertyChanged();
            }
        }

        private CraftsmanUiModel _craftsman;

        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
                OnPropertyChanged();
            }
        }

        public bool CanContinue => Form.IsValid && CertificateFile != null;

        public ICommand UploadCertificateCommand { get; private set; }
        public ICommand MoveToApplyConfirmationCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public CraftsmanApplyInfoViewModel(ICallback view, CraftsmanUiModel newCraftsmanModel)
        {
            this.view = view;
            _newCraftsmanModel = newCraftsmanModel;

            BackCommand = new Command(Back);
            UploadCertificateCommand = new AsyncCommand(UploadCertificate);
            MoveToApplyConfirmationCommand = new Command(MoveToApplyConfirmation);

            AboutHeader.Validations.Add(new IsNotNullOrEmptyRule<string>
                {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
            AboutText.Validations.Add(new IsNotNullOrEmptyRule<string>
                {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
            Form.Add(new IIsValid[] {AboutHeader, AboutText});
        }

        public async void Back()
        {
            await view.Back();
        }


        private async System.Threading.Tasks.Task UploadCertificate()
        {
            try
            {              
                CertificateFile = await MediaPicker.PickPhotoAsync();

                OnPropertyChanged(nameof(CanContinue));
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }
        }

        public async void MoveToApplyConfirmation()
        {
            if (Form.IsValid && CertificateFile != null)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    var newCraftsmanMdl = new Craftsman()
                    {
                        OfficeId = Settings.ActiveOffice,
                        UserId = dao.ActiveUser.Id,

                        AboutHeader = AboutHeader.Value,
                        AboutText = AboutText.Value,
                        Ratings = new Rating[] { },
                        CompanyName = _newCraftsmanModel.CompanyName,
                        CompanyAddress = _newCraftsmanModel.CompanyAddress,
                        OrgNumber = _newCraftsmanModel.OrgNumber,
                        CraftsmanName = dao.ActiveUser.FullName,
                        WorkArea = dao.ActiveUser.City ?? string.Empty,
                        CompletedJobs = 0,
                        FTax = _newCraftsmanModel.FTax,
                        AccountNumber = _newCraftsmanModel.AccountNumber,
                        Modified = DateTime.Now.Normalize(),
                        MemberSince = DateTime.Now.Normalize().ToRFC3339()
                    };

                    // user is not a craftsman, create one
                    Craftsman = (await dao.Add(newCraftsmanMdl)).ToCraftsmanUiModel();

                    // add a new craft
                    var craftMdl = new Craft()
                    {
                        Id = string.Empty,
                        CertificateId = string.Empty,

                        CraftType = _newCraftsmanModel.Crafts.First().CraftType,
                        Status = CraftStatus.Applied
                    };
                    (Craftsman CraftsmanApiMdl, string CraftId) craftResponse =
                        await dao.CraftApply(Settings.ActiveOffice, Craftsman.Id, craftMdl);

                    var certificateStream = await CertificateFile.OpenReadAsync();
                    var certificateMdl = await dao.AddCertificate(
                        craftResponse.CraftsmanApiMdl,
                        craftResponse.CraftId,
                        certificateStream,
                        Path.GetExtension(CertificateFile.FullPath).TrimStart('.'));

                    certificateStream.Dispose();
                    
                    userDialogs.HideLoading();
                    await view.OpenApplyConfirmation();
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    if (ex.Message.Contains("received: 409") || ex.Message.Contains("id already exists"))
                    {
                        userDialogs.Toast(AppResources.CraftsmanAlreadyExistsErrorMessage);
                        Crashes.TrackError(ex);
                    }
                    else
                    {
                        HandleError(ex, ex.Message);
                    }
                }
            }
        }
    }
}