using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class ChatsViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToChat(ChatUiModel chat, string recipientId);
        }

        private readonly ICallback view;
        
        private List<Chat> _rawChatCache;
        private List<ChatUiModel> _chatCache;
        private List<MessageUiModel> _messageCache;

        private readonly object _chatLock = new object();

        private ObservableCollection<ChatUiModel> chats;

        public ObservableCollection<ChatUiModel> Chats
        {
            get { return chats; }
            set
            {
                chats = value;
                OnPropertyChanged();
            }
        }

        public User User { get; set; }


        public ICommand MoveToChatCommand { get; private set; }

        public ChatsViewModel(ICallback view)
        {
            IsATab = true;
            this.view = view;

            MoveToChatCommand = new AsyncCommand<ChatUiModel>(MoveToChat);

            Chats = new ObservableCollection<ChatUiModel>();
        }
        
        
        public override async void Navigated()
        {
            base.Navigated();

            userDialogs.ShowLoading(AppResources.LoadingString);
            
            if (!IsSubscribedToDao)
            {
                dao.Subscribe(HandleChatSuccess, HandleError);
                dao.Subscribe(HandleMessageSuccess, HandleError);
                IsSubscribedToDao = true;
            }
        }


        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
            
            if (IsSubscribedToDao)
            {
                dao.Unsubscribe(HandleChatSuccess, HandleError);
                dao.Unsubscribe(HandleMessageSuccess, HandleError);
                IsSubscribedToDao = false;
            }
        }

        private async void HandleChatSuccess(Chat[] data, string nonce, DateTime updated)
        {
            try
            {
                _rawChatCache = new List<Chat>(data);
                _chatCache = data.Select(cht => cht.ToChatUiModel()).ToList();

                foreach (var chat in _chatCache)
                {
                    chat.Task = (await dao.GetTask(chat.OfficeId, chat.TaskId)).ToTaskUiModel();
                    chat.Bid = (await dao.GetBid(chat.OfficeId, chat.TaskId, chat.BidId)).ToBidUiModel();
                }
                
                await UpdateChat();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }

        private async void HandleMessageSuccess(Message[] data, string nonce, DateTime updated)
        {
            try
            {
                _messageCache = data.Select(msg => msg.ToMessageUiModel()).ToList();

                foreach (var message in _messageCache)
                {
                    message.Sender = (await dao.GetUser(message.UserId)).ToUserUiModel();
                }
                
                await UpdateChat();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }        
        }

        public async System.Threading.Tasks.Task MoveToChat(ChatUiModel chat)
        {
            userDialogs.ShowLoading(AppResources.LoadingString);
            // determine recipient: if the chat is for user's task, then bidder, otherwise the task owner
            await view.MoveToChat(chat, chat.ChatRecipient.Id);
        }
        
        private async System.Threading.Tasks.Task Delete(ChatUiModel chat)
        {
            try
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                var chatMdl = _rawChatCache.FirstOrDefault(ch => ch.Id.Equals(chat.Id));
                await dao.Delete(chatMdl);

                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }

        private async System.Threading.Tasks.Task UpdateChat()
        {
            if (_chatCache == null || _chatCache.Count == 0 || _messageCache == null || _messageCache.Count == 0)
            {
                userDialogs.HideLoading();
                return;
            }

            try
            {
                var updatedChatList = new List<ChatUiModel>(_chatCache);
                foreach (var chat in updatedChatList)
                {
                    var lastMsg = _messageCache
                        .Where(msg => msg.ChatId.Equals(chat.Id))
                        .OrderByDescending(msg => msg.Sent)
                        .FirstOrDefault();

                    if (lastMsg != null)
                    {
                        var recipientId = chat.Task.UserId.Equals(dao.ActiveUser.Id)
                            ? chat.Bid.CraftsmanId
                            : chat.Task.UserId;
                        chat.ChatRecipient = (await dao.GetUser(recipientId)).ToUserUiModel();
                        chat.LastMessage = lastMsg;
                    }
                }

                updatedChatList = updatedChatList
                    .Where(cht => cht.LastMessage != null)
                    .OrderByDescending(cht => cht.LastMessage.Sent)
                    .ToList();

                lock (_chatLock)
                {
                    Chats = new ObservableCollection<ChatUiModel>(updatedChatList);
                }
                userDialogs.HideLoading();
            }
            catch (NullReferenceException ex) // NREs sometimes happen here, no idea why
            {
                Debug.WriteLine(ex);
            }
        }
    }
}