using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Toolit.Helpers;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class BidViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId);
            System.Threading.Tasks.Task OpenSwish(BidUiModel bid);
        }

        private readonly ICallback view;
        private readonly string _taskId;
        private readonly string _bidId;
        private ChatUiModel _chat;

        private bool _isShowingInfo;
        public bool IsShowingInfo
        {
            get => _isShowingInfo;
            set
            {
                _isShowingInfo = value;
                OnPropertyChanged();
            }
        }

        private BidUiModel _displayedBid;
        public BidUiModel DisplayedBid
        {
            get => _displayedBid;
            set
            {
                _displayedBid = value;
                OnPropertyChanged();
            }
        }

        // for content view bindings
        public CraftsmanUiModel Craftsman => DisplayedBid?.Craftsman;

        private ObservableCollection<RatingUiModel> ratings;

        public ObservableCollection<RatingUiModel> Ratings
        {
            get => ratings;
            set
            {
                ratings = value;
                OnPropertyChanged();
            }
        }
        
        public ICommand ShowBidInfoCommand { get; private set; }
        public ICommand ShowAboutCommand { get; private set; }
        public ICommand AcceptBidCommand { get; private set; }
        public ICommand MoveToChatCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand MoveToToSCommand { get; private set; }

        
        public BidViewModel(ICallback view, string taskId, string bidId)
        {
            this.view = view;
            _taskId = taskId;
            _bidId = bidId;
            
            ShowBidInfoCommand = new AsyncCommand(ShowBidInfo);
            ShowAboutCommand = new AsyncCommand(ShowAbout);
            AcceptBidCommand = new AsyncCommand(AcceptBid);
            MoveToChatCommand = new AsyncCommand(MoveToChat);
            MoveToToSCommand = new AsyncCommand(MoveToToS);
            BackCommand = new AsyncCommand(Back);
        }


        public override async void Navigated()
        {
            base.Navigated();
            
            try
            {
                DisplayedBid = (await dao.GetBid(Settings.ActiveOffice, _taskId, _bidId)).ToBidUiModel();
                DisplayedBid.Task = (await dao.GetTask(Settings.ActiveOffice, _taskId)).ToTaskUiModel();
                DisplayedBid.Craftsman =
                    (await dao.GetCraftsman(Settings.ActiveOffice, DisplayedBid.CraftsmanId))
                    .ToCraftsmanUiModel();
                DisplayedBid.Craftsman.User = (await dao.GetUser(DisplayedBid.Craftsman.UserId)).ToUserUiModel();

                Ratings = new ObservableCollection<RatingUiModel>(DisplayedBid.Craftsman.Ratings);

                if (!IsSubscribedToDao)
                {
                    dao.Subscribe(HandleChatSuccess, HandleError);
                    IsSubscribedToDao = true;
                }
                
                OnPropertyChanged(nameof(Craftsman));
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
                dao.Unsubscribe(HandleChatSuccess, HandleError);
                IsSubscribedToDao = false;
            }
        }

        private void HandleChatSuccess(Chat[] data, string nonce, DateTime updated)
        {
            try
            {
                _chat = data.FirstOrDefault(cht => cht.BidId.Equals(DisplayedBid.Id))?.ToChatUiModel();
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
        
        public async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }
        
        private async System.Threading.Tasks.Task ShowBidInfo()
        {
            IsShowingInfo = false;
        }

        private async System.Threading.Tasks.Task ShowAbout()
        {
            IsShowingInfo = true;
        }
        
        public async System.Threading.Tasks.Task AcceptBid()
        {
            await view.OpenSwish(DisplayedBid);
        }
        
        public async System.Threading.Tasks.Task MoveToToS()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitTosUrl);
        }

        public async System.Threading.Tasks.Task MoveToChat()
        {
            if (!ReferenceEquals(_chat, null))
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                await view.MoveToChat(_chat, DisplayedBid.CraftsmanId);
            }
        }
    }
}
