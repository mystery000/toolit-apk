using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Toolit.Helpers;
using Toolit.Mappers;
using Toolit.Models.Misc;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;

namespace Toolit.ViewModels
{
    public class MyTaskViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToBid(string taskId, string bidId);
            System.Threading.Tasks.Task MoveToEditTask(string taskId);
            System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId);
            System.Threading.Tasks.Task OpenRating(string taskId, string craftsmanId);
            System.Threading.Tasks.Task OpenFullImage(string url);
            System.Threading.Tasks.Task OpenFullVideo(string url);
        }

        private readonly ICallback view;
        
        private readonly string _taskId;
        private ChatUiModel _chat;

        private TaskUiModel _displayedTask;
        private ObservableCollection<MediaFile> _displayedTaskMediaFiles;
        private bool _isShowingInfo;
        private ObservableCollection<BidUiModel> _taskBidList;
        private BidUiModel _acceptedBid;

        public TaskUiModel DisplayedTask
        {
            get => _displayedTask;
            set
            {
                _displayedTask = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasAcceptedBid));
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

        public bool IsShowingInfo
        {
            get => _isShowingInfo;
            set
            {
                _isShowingInfo = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BidUiModel> TaskBidList
        {
            get => _taskBidList;
            set
            {
                _taskBidList = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(NumberOfTaskBidsFormattedString));
            }
        }

        public string NumberOfTaskBidsFormattedString => HasAcceptedBid
            ? AppResources.SubmittedBidsString
            : string.Format(AppResources.SubmittedBidsFormatString, TaskBidList?.Count ?? 0);

        public BidUiModel AcceptedBid
        {
            get => _acceptedBid;
            set
            {
                _acceptedBid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasAcceptedBid));
            }
        }

        public bool HasAcceptedBid => AcceptedBid != null;
        public bool CanFinish => DisplayedTask != null && !DisplayedTask.Finished;

        public ICommand BackCommand { get; private set; }
        public ICommand ShowBidsCommand { get; private set; }
        public ICommand ShowInfoCommand { get; private set; }
        public ICommand MoveToBidDetailCommand { get; private set; }
        public ICommand MoveToToSCommand { get; private set; }
        public ICommand EditTaskCommand { get; private set; }

        public ICommand OpenChatCommand { get; private set; }
        public ICommand FinishTaskCommand { get; private set; }
        
        public ICommand OpenFullImageCommand { get; private set; }
        public ICommand OpenFullVideoCommand { get; private set; }

        public MyTaskViewModel(ICallback view, string taskId)
        {
            this.view = view;
            _taskId = taskId;
            
            BackCommand = new AsyncCommand(Back);
            ShowBidsCommand = new AsyncCommand(ShowBids);
            ShowInfoCommand = new AsyncCommand(ShowInfo);
            MoveToBidDetailCommand = new AsyncCommand<BidUiModel>(MoveToBidDetail);
            MoveToToSCommand = new AsyncCommand(MoveToToS);
            EditTaskCommand = new AsyncCommand(EditTask);
            OpenChatCommand = new AsyncCommand(OpenChat);
            FinishTaskCommand = new AsyncCommand(FinishTask);
            OpenFullImageCommand = new AsyncCommand<MediaFile>(OpenFullImage);
            OpenFullVideoCommand = new AsyncCommand<MediaFile>(OpenFullVideo);

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

                foreach (var imageUrl in DisplayedTask.ImageUrls)
                {
                    DisplayedTaskMediaFiles.Add(new MediaFile() {Url = imageUrl});
                }

                if (!IsSubscribedToDao)
                {
                    dao.Subscribe(HandleBidsSuccess, HandleError);
                    dao.Subscribe(HandleChatSuccess, HandleError);
                    IsSubscribedToDao = true;
                }
                
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
                IsSubscribedToDao = false;
            }
        }

        private async void HandleBidsSuccess(Bid[] data, string nonce, DateTime updated)
        {
            try
            {
                if (string.IsNullOrEmpty(DisplayedTask.AcceptedBid))
                {
                    TaskBidList =
                        new ObservableCollection<BidUiModel>(await System.Threading.Tasks.Task.WhenAll(data
                            .Where(b => !b.Deleted && b.TaskId.Equals(_taskId))
                            .Select(async b =>
                            {
                                try
                                {
                                    var uiMdl = b.ToBidUiModel();
                                    uiMdl.Craftsman =
                                        (await dao.GetCraftsman(Settings.ActiveOffice, b.CraftsmanId))
                                        .ToCraftsmanUiModel();
                                    uiMdl.Craftsman.User = (await dao.GetUser(uiMdl.Craftsman.UserId)).ToUserUiModel();

                                    return uiMdl;
                                }
                                catch (Exception ex)
                                {
                                    HandleError(ex, ex.Message);
                                    
                                    return new BidUiModel();
                                }
                            }))
                        );
                }
                else
                {
                    AcceptedBid = (await dao.GetBid(Settings.ActiveOffice, DisplayedTask.Id,
                        DisplayedTask.AcceptedBid)).ToBidUiModel();
                    AcceptedBid.Craftsman =
                        (await dao.GetCraftsman(Settings.ActiveOffice, AcceptedBid.CraftsmanId))
                        .ToCraftsmanUiModel();
                    AcceptedBid.Craftsman.User = (await dao.GetUser(AcceptedBid.Craftsman.UserId)).ToUserUiModel();
                }
                OnPropertyChanged(nameof(NumberOfTaskBidsFormattedString));
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

        private async System.Threading.Tasks.Task MoveToBidDetail(BidUiModel bid)
        {
            userDialogs.ShowLoading(AppResources.LoadingString);
            await view.MoveToBid(_taskId, bid.Id);
        }
        
        private async System.Threading.Tasks.Task MoveToToS()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitTosUrl);
        }

        private async System.Threading.Tasks.Task EditTask()
        {
            userDialogs.ShowLoading(AppResources.LoadingString);
            await view.MoveToEditTask(_taskId);
        }

        private async System.Threading.Tasks.Task OpenChat()
        {
            if (AcceptedBid != null && !ReferenceEquals(_chat, null))
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                await view.MoveToChat(_chat, AcceptedBid.CraftsmanId);
            }
        }
        
        private async System.Threading.Tasks.Task FinishTask()
        {
            if (AcceptedBid != null)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    var craftsmanId = AcceptedBid.CraftsmanId;
                    await dao.FinishTask(DisplayedTask.OfficeId, DisplayedTask.Id);

                    userDialogs.HideLoading();
                    await view.OpenRating(DisplayedTask.Id, craftsmanId);
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
        }
        
        private async System.Threading.Tasks.Task OpenFullImage(MediaFile image)
        {
            await view.OpenFullImage(image.Url);
        }
        
        private async System.Threading.Tasks.Task OpenFullVideo(MediaFile video)
        {
            await view.OpenFullVideo(video?.Url ?? DisplayedTask.TitleVideoUrl);
        }

    }
}