using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Rg.Plugins.Popup.Services;
using Toolit.Extensions;
using Toolit.Helpers;
using Toolit.Mappers;
using Toolit.Models.Misc;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Toolit.ViewModels
{
    public class OtherTaskViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();           
            System.Threading.Tasks.Task EditMessage(string taskId, string bidId);
            System.Threading.Tasks.Task EditOffer(string taskId, string bidId, decimal brokerageFee);
            System.Threading.Tasks.Task DeleteBid(string taskId, string bidId);
            System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId);
            System.Threading.Tasks.Task ShowAddedBidConfirmation();
            System.Threading.Tasks.Task OpenFullImage(string url);
            System.Threading.Tasks.Task OpenFullVideo(string url);
        }

        private readonly ICallback view;
        private readonly string _taskId;
        private ChatUiModel _chat;
        private decimal _brokerageFee;
        
        private TaskUiModel _displayedTask;
        private ObservableCollection<MediaFile> _displayedTaskMediaFiles;
        private bool _isShowingInfo;
        private BidUiModel _taskBid;
        
        private bool _isACraftsman;
        private BidUiModel _newBid;

        public TaskUiModel DisplayedTask
        {
            get => _displayedTask;
            set
            {
                _displayedTask = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanFinish));
            }
        }

        public ObservableCollection<MediaFile> DisplayedTaskMediaFiles
        {
            get => _displayedTaskMediaFiles;
            set
            {
                _displayedTaskMediaFiles = value;
                OnPropertyChanged();
            }
        }

        public bool IsACraftsman
        {
            get => _isACraftsman;
            set
            {
                _isACraftsman = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowingInfo
        {
            get => _isShowingInfo;
            set
            {
                _isShowingInfo = value;
                OnPropertyChanged();
            }
        }

        public BidUiModel TaskBid
        {
            get => _taskBid;
            set
            {
                _taskBid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasAddedBid));
                OnPropertyChanged(nameof(HasUserAcceptedBid));
            }
        }

        public BidUiModel NewBid
        {
            get => _newBid;
            set
            {
                _newBid = value;
                OnPropertyChanged();
            }
        }

        public bool HasAddedBid => TaskBid != null;
        public bool HasUserAcceptedBid => HasAddedBid && (DisplayedTask?.AcceptedBid?.Equals(TaskBid.Id) ?? false);
        public bool CanFinish => DisplayedTask != null && !DisplayedTask.Finished;

        public bool IsNewBidCostInfoVisible => (AddNewBidForm?.IsValid ?? false) &&
                                               !string.IsNullOrWhiteSpace(LaborCost.Value) &&
                                               !string.IsNullOrWhiteSpace(MaterialCost.Value);


        public ValidatableGroup AddNewBidForm { get; } = new ValidatableGroup();
        
        public ValidatableObject<string> BidMessage { get; } = new ValidatableObject<string>()
        {
            ValidateOnChange = true
        };
        public ValidatableObject<string> LaborCost { get; } = new ValidatableObject<string>()
        {
            IsValid = true,
            ValidateOnChange = true
        };
        public ValidatableObject<string> MaterialCost { get; } = new ValidatableObject<string>()
        {
            IsValid = true,
            ValidateOnChange = true
        };

        public ICommand BackCommand { get; private set; }
        public ICommand ShowBidsCommand { get; private set; }
        public ICommand ShowInfoCommand { get; private set; }        
        public ICommand ValidateCommand { get; private set; }        
        public ICommand AddBidCommand { get; private set; }
        public ICommand EditMessageCommand { get; private set; }
        public ICommand EditOfferCommand { get; private set; }
        public ICommand OpenChatCommand { get; private set; }
        public ICommand CompleteJobCommand { get; private set; }
        public ICommand DeleteBidCommand { get; private set; }
        public ICommand MoveToToSCommand { get; private set; }
        public ICommand OpenFullImageCommand { get; private set; }
        public ICommand OpenFullVideoCommand { get; private set; }

        public OtherTaskViewModel(ICallback view, string taskId) : base()
        {
            this.view = view;
            _taskId = taskId;
            
            BackCommand = new AsyncCommand(Back);
            ShowBidsCommand = new AsyncCommand(ShowBids);
            ShowInfoCommand = new AsyncCommand(ShowInfo);
            ValidateCommand = new AsyncCommand(Validate);
            AddBidCommand = new AsyncCommand(AddBid);
            EditMessageCommand = new AsyncCommand(EditMessage);
            EditOfferCommand = new AsyncCommand(EditOffer);
            OpenChatCommand = new AsyncCommand(OpenChat);
            CompleteJobCommand = new AsyncCommand(CompleteJob);
            DeleteBidCommand = new AsyncCommand(DeleteBid);
            MoveToToSCommand = new AsyncCommand(MoveToToS);
            OpenFullImageCommand = new AsyncCommand<MediaFile>(OpenFullImage);
            OpenFullVideoCommand = new AsyncCommand<MediaFile>(OpenFullVideo);
            
            BidMessage.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            LaborCost.Validations.Add(new IsValuePositiveRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            MaterialCost.Validations.Add(new IsValuePositiveRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            
            AddNewBidForm.Add(new IIsValid[] { BidMessage, LaborCost, MaterialCost });

            IsShowingInfo = true;
        }

        public override async void Navigated()
        {
            base.Navigated();

            userDialogs.ShowLoading();
            
            try
            {
                DisplayedTask = (await dao.GetTask(Settings.ActiveOffice, _taskId)).ToTaskUiModel();
                DisplayedTaskMediaFiles = new ObservableCollection<MediaFile>();

                if (DisplayedTask.HasVideo)
                {
                    foreach (var videoUrl in DisplayedTask.VideoUrls)
                    {
                        DisplayedTaskMediaFiles.Add(new MediaFile() {Url = videoUrl, IsVideo = true});
                    }
                }

                foreach (var url in DisplayedTask.ImageUrls)
                {
                    DisplayedTaskMediaFiles.Add(new MediaFile() {Url = url});
                }
                
                if (!IsSubscribedToDao)
                {
                    AddNewBidForm.Validated += AddNewBidFormOnValidated;
                    
                    _brokerageFee = (await dao.GetOffice("", Settings.ActiveOffice)).BrokeragePercentage;
                    LaborCost.IsChanged = false;
                    MaterialCost.IsChanged = false;
                    
                    dao.Subscribe(HandleBidsSuccess, HandleError);
                    dao.Subscribe(HandleChatSuccess, HandleError);
                    
                    IsSubscribedToDao = true;
                }

                try
                {
                    var craftsmanMdl = await dao.GetCraftsman(Settings.ActiveOffice, dao.ActiveUser.Id);
                
                    // if no exception, then can move on
                    IsACraftsman = !craftsmanMdl.Deleted && craftsmanMdl.Crafts.Any(crft => crft.Status != CraftStatus.Rejected);
                }
                catch (Exception ex)
                {
                    IsACraftsman = false;
                }
                
                OnPropertyChanged(nameof(HasUserAcceptedBid));
                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
                userDialogs.HideLoading();
            }
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
            
            if (IsSubscribedToDao)
            {
                dao.Unsubscribe(HandleBidsSuccess, HandleError);
                dao.Unsubscribe(HandleChatSuccess, HandleError);
                
                AddNewBidForm.Validated -= AddNewBidFormOnValidated;
                
                IsSubscribedToDao = false;
            }
        }
        
        private void AddNewBidFormOnValidated(object sender, EventArgs e)
        {
            if (AddNewBidForm.IsValid)
            {
                decimal.TryParse(LaborCost.Value.Replace(',', '.'), NumberStyles.Any, 
                    CultureInfo.InvariantCulture, out var laborCost);
                decimal.TryParse(MaterialCost.Value.Replace(',', '.'), NumberStyles.Any, 
                    CultureInfo.InvariantCulture, out var materialCost);
                
                NewBid = BidCostHelper.ConstructNewBidModel(laborCost, materialCost, DisplayedTask.UseRotRut, _brokerageFee);
                OnPropertyChanged(nameof(IsNewBidCostInfoVisible));
            }
        }

        private async void HandleBidsSuccess(Bid[] data, string nonce, DateTime updated)
        {
            try
            {
                TaskBid = data.Where(b => !b.Deleted)
                    .FirstOrDefault(b => b.CraftsmanId.Equals(dao.ActiveUser.Id) && b.TaskId.Equals(_taskId))?
                    .ToBidUiModel();

                if (TaskBid != null)
                {
                    TaskBid.Craftsman =
                        (await dao.GetCraftsman(Settings.ActiveOffice, dao.ActiveUser.Id))
                        .ToCraftsmanUiModel();
                    TaskBid.Craftsman.User = dao.ActiveUser.ToUserUiModel();
                    TaskBid.BrokerageFee = _brokerageFee;

                    OnPropertyChanged(nameof(HasUserAcceptedBid));
                }
                
                OnPropertyChanged(nameof(HasAddedBid));
                OnPropertyChanged(nameof(HasUserAcceptedBid));
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
        
        
        private void HandleChatSuccess(Chat[] data, string nonce, DateTime updated)
        {
            try
            {
                _chat = data.FirstOrDefault(cht => cht.TaskId.Equals(DisplayedTask.Id))?.ToChatUiModel();
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }

        
        private async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }
        
        private async System.Threading.Tasks.Task ShowBids()
        {
            IsShowingInfo = false;
        }

        private async System.Threading.Tasks.Task ShowInfo()
        {
            IsShowingInfo = true;
        }
        
        private async System.Threading.Tasks.Task Validate()
        {
            AddNewBidFormOnValidated(this, EventArgs.Empty);
        }

        private async System.Threading.Tasks.Task AddBid()
        {
            if (AddNewBidForm.IsValid)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    decimal.TryParse(LaborCost.Value.Replace(',', '.'), NumberStyles.Any, 
                        CultureInfo.InvariantCulture, out var laborCost);
                    decimal.TryParse(MaterialCost.Value.Replace(',', '.'), NumberStyles.Any, 
                        CultureInfo.InvariantCulture, out var materialCost);

                    var newBidUiMdl = BidCostHelper.ConstructNewBidModel(laborCost, materialCost, DisplayedTask.UseRotRut, _brokerageFee);

                    var newBidMdl = new Bid()
                    {
                        OfficeId = Settings.ActiveOffice,
                        CraftsmanId = dao.ActiveUser.Id,
                        TaskId = DisplayedTask.Id,
                        
                        BidMessage = BidMessage.Value,
                        
                        LabourCost = newBidUiMdl.LabourCost,
                        MaterialCost = newBidUiMdl.MaterialCost,
                        RootDeduction = newBidUiMdl.RootDeduction,
                        Vat = newBidUiMdl.Vat,
                        FinalBid = newBidUiMdl.FinalBid,

                        Modified = DateTime.Now.Normalize()
                    };
                    
                    await dao.Add(newBidMdl);

                    userDialogs.HideLoading();
                    await view.ShowAddedBidConfirmation();
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

        private async System.Threading.Tasks.Task EditMessage()
        {
            await view.EditMessage(DisplayedTask.Id, TaskBid.Id);
        }

        private async System.Threading.Tasks.Task EditOffer()
        {
            await view.EditOffer(DisplayedTask.Id, TaskBid.Id, _brokerageFee);
        }

        private async System.Threading.Tasks.Task OpenChat()
        {
            if (HasUserAcceptedBid && !ReferenceEquals(_chat, null))
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                await view.MoveToChat(_chat, DisplayedTask.UserId);
            }
        }
        
        private async System.Threading.Tasks.Task CompleteJob()
        {
            if (CanFinish)
            {
                try
                {
                    userDialogs.ShowLoading();

                    await dao.CraftsmanFinishTask(DisplayedTask.OfficeId, DisplayedTask.Id);
                    
                    userDialogs.HideLoading();
                    userDialogs.Toast(AppResources.CompleteJobAction);
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
        }
        
        private async System.Threading.Tasks.Task DeleteBid()
        {
            await view.DeleteBid(DisplayedTask.Id, TaskBid.Id);
        }
        
        private async System.Threading.Tasks.Task MoveToToS()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitTosUrl);
        }
        
        private async System.Threading.Tasks.Task OpenFullImage(MediaFile image)
        {
            await view.OpenFullImage(image.Url);
        }
        
        private async System.Threading.Tasks.Task OpenFullVideo(MediaFile video = null)
        {
            await view.OpenFullVideo(video?.Url ?? DisplayedTask.TitleVideoUrl);
        }
    }
}