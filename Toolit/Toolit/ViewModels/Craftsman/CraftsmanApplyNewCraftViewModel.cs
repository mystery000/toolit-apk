using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Layouts;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Toolit.ViewModels
{
    public class CraftsmanApplyNewCraftViewModel : BaseViewModel
    {
        public class PreselectableCraftlayoutModel : INotifyPropertyChangedBase
        {
            private bool _isPreselected;
            private CraftLayoutModel _craft;

            public bool IsPreselected
            {
                get => _isPreselected;
                set
                {
                    _isPreselected = value;
                    if (value)
                    {
                        Craft.IsSelected = true;
                    }
                    
                    OnPropertyChanged();
                }
            }

            public CraftLayoutModel Craft
            {
                get => _craft;
                set
                {
                    _craft = value;
                    if (_isPreselected)
                    {
                        value.IsSelected = true;
                    }
                }
            }
        }
        
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task OpenApplyConfirmation();
        }
        
        private readonly ICallback view;
        
        private FileResult _certificateFile;
        
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
        
        private ObservableCollection<PreselectableCraftlayoutModel> _craftList;
        public ObservableCollection<PreselectableCraftlayoutModel> CraftList
        {
            get => _craftList;
            set
            {
                _craftList = value;
                OnPropertyChanged();
            }
        }

        public bool CanContinue => 
            (CraftList?.Any(crft => crft.Craft.IsSelected && !crft.IsPreselected) ?? false) &&
            CertificateFile != null;
        
        public ICommand CraftTappedCommand { get; private set; }
        public ICommand UploadCertificateCommand { get; private set; }
        public ICommand MoveToApplyConfirmationCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        
        public CraftsmanApplyNewCraftViewModel(ICallback view)
        {
            this.view = view;
            
            BackCommand = new AsyncCommand(Back);
            CraftTappedCommand = new AsyncCommand<PreselectableCraftlayoutModel>(CraftTapped);
            UploadCertificateCommand = new AsyncCommand(UploadCertificate);
            MoveToApplyConfirmationCommand = new AsyncCommand(MoveToApplyConfirmation);

            CraftList = new ObservableCollection<PreselectableCraftlayoutModel>(AppConstants.CraftModels
                .Select(crft => new PreselectableCraftlayoutModel()
                {
                    Craft = crft
                }));
        }

        public override async void Navigated()
        {
            base.Navigated();
            
            Craftsman = (await dao.GetCraftsman(Settings.ActiveOffice, dao.ActiveUser.Id)).ToCraftsmanUiModel();
            foreach (var craft in CraftList)
            {
                if (Craftsman.Crafts.Any(crft => crft.CraftType.Equals(craft.Craft.ServerId)))
                {
                    // preselect skill that craftsman already has
                    craft.IsPreselected = true;
                }
            }
        }

        public async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }
        
        private async System.Threading.Tasks.Task CraftTapped(PreselectableCraftlayoutModel craft)
        {
            if (craft.IsPreselected)
            {
                return;
            }
            
            if (!craft.Craft.IsSelected)
            {
                foreach (var craftLayoutModel in CraftList)
                {
                    if (!craftLayoutModel.IsPreselected)
                    {
                        craftLayoutModel.Craft.IsSelected = false;
                    }
                }

                craft.Craft.IsSelected = true;
            }
            else
            {
                craft.Craft.IsSelected = false;
            }

            OnPropertyChanged(nameof(CanContinue));
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
        
        public async System.Threading.Tasks.Task MoveToApplyConfirmation()
        {
            if (Craftsman != null)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);
                    
                    // add a new craft
                    var craftMdl = new Craft()
                    {
                        Id = string.Empty,
                        CertificateId = string.Empty,
                        
                        CraftType = CraftList.First(crft => !crft.IsPreselected && crft.Craft.IsSelected).Craft.ServerId,
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
                    HandleError(ex, ex.Message);
                }
            }
        }
    }
}