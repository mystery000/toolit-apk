using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Misc;
using Toolit.Models.Ui;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task OpenFullImage(ImageSource source);
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;

        private readonly Queue<(Message Message, Stream ImageStream, string extension)> _messageQueue = 
            new Queue<(Message Message, Stream ImageStream, string extension)>();
        private Timer _messageQueueTimer;

        private bool _hasNoUnreadMessages;
        
        private readonly string _recipientId;
        private List<MessageUiModel> _messages;
        private string _messageText;
        private ChatUiModel _displayedChat;
        private UserUiModel _recipient;
        private bool _isSending;

        public ChatUiModel DisplayedChat
        {
            get => _displayedChat;
            set
            {
                _displayedChat = value;
                OnPropertyChanged();
            }
        }

        public UserUiModel Recipient
        {
            get => _recipient;
            set
            {
                _recipient = value;
                OnPropertyChanged();
            }
        }

        public List<MessageUiModel> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }

        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged();
            }
        }

        public bool IsSending
        {
            get => _isSending;
            set
            {
                _isSending = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; private set; }
        public ICommand AttachImageCommand { get; private set; }
        public ICommand SendMessageCommand { get; private set; }
        public ICommand OpenFullImageCommand { get; set; }

        public ChatViewModel(ICallback view, ChatUiModel chat, string recipientId)
        {
            this.view = view;
            _recipientId = recipientId;
            
            DisplayedChat = chat;
            
            BackCommand = new Command(Back);
            AttachImageCommand = new AsyncCommand(AttachImage);
            SendMessageCommand = new AsyncCommand(SendMessage);
            OpenFullImageCommand = new AsyncCommand<MessageUiModel>(OpenFullImage);
        }

        public override async void Navigated()
        {
            base.Navigated();
            
            try
            {
                Recipient = (await dao.GetUser(_recipientId)).ToUserUiModel();
                if (!IsSubscribedToDao)
                {
                    _messageQueueTimer = new Timer(CheckMessageQueue, null, 0, 1000);
                    
                    dao.Subscribe(HandleMessageSuccess, HandleError);
                    
                    IsSubscribedToDao = true;
                }
                
                OnPropertyChanged(nameof(Craftsman));
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
                userDialogs.HideLoading();
            }
        }

        
        public override async void NavigatingFrom()
        {
            base.NavigatingFrom();
            
            // wait for the vm to work through the queue before destroying it
            while (IsSending || _messageQueue.Any())
            {
                await System.Threading.Tasks.Task.Delay(1000);
            }
            
            if (IsSubscribedToDao)
            {
                dao.Unsubscribe(HandleMessageSuccess, HandleError);
                _messageQueueTimer.Dispose();
                _messageQueueTimer = null;
                
                IsSubscribedToDao = false;
            }
        }

        private async void HandleMessageSuccess(Message[] data, string nonce, DateTime updated)
        {
            try
            {
                Debug.WriteLine("In HandleMessageSuccess");
                var messages = data
                    .Where(msg => msg.ChatId.Equals(DisplayedChat.Id)).ToList();

                // boldly assume that if the chat page was opened, the user has read all unread messages in it
                var unreadMessagesCount = messages.Count(msg => !msg.UserId.Equals(dao.ActiveUser?.Id) && !msg.IsRead);
                Debug.WriteLine($"unreadMessagesCount: {unreadMessagesCount}");
                if (unreadMessagesCount > 0 && !_hasNoUnreadMessages)
                {
                    var markReadResult = await dao.MarkRead(DisplayedChat.ToChatApiModel());
                    // we need this to exit the loop bc the cache might not have updated for older chat messages
                    _hasNoUnreadMessages = !markReadResult.Any();
                    //this will trigger another update event, so no need to continue this method
                    return;
                }
                
                // chat layout is reversed  
                var uiMessages = messages.Select(msg => msg.ToMessageUiModel()).ToList(); 

                foreach (var message in uiMessages)
                {
                    message.Sender = (await dao.GetUser(message.UserId)).ToUserUiModel();
                    message.ChatRecipientId = _recipientId;
                }

                if (Messages == null)
                {
                    Messages = new List<MessageUiModel>(uiMessages.OrderByDescending(msg => msg.Sent));
                }
                else
                {
                    foreach (var message in uiMessages)
                    {
                        // since new messages from the Messages collection don't have ids, this is an alternative id check
                        var listMsg = Messages.FirstOrDefault(msg => 
                            msg.Text.Equals(message.Text) &&
                            Math.Abs((msg.Sent - message.Sent).TotalMilliseconds) < 100);
                        
                        if (listMsg == null)
                        {
                            Messages.Insert(0, message);
                        }
                        else if (string.IsNullOrEmpty(listMsg.Id))
                        {
                            // update status
                            listMsg.Id = message.Id;
                            listMsg.Status = message.Status;
                        }
                        else if (listMsg.ImageUrl != message.ImageUrl)
                        {
                            listMsg.ImageUrl = message.ImageUrl;
                        }
                    }

                    Messages = new List<MessageUiModel>(Messages.OrderByDescending(msg => msg.Sent));
                }

                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }        
        }
        
        public async void Back()
        {
            await view.Back();
        }

        private async System.Threading.Tasks.Task AttachImage()
        {
            var imageFile = await MediaPicker.PickPhotoAsync();

            if (imageFile != null)
            {
                try
                {
                    var dt = DateTime.UtcNow;
                    
                    var newMsg = new Message()
                    {
                        ChatId = DisplayedChat.Id,
                        OfficeId = DisplayedChat.OfficeId,
                        TaskId = DisplayedChat.TaskId,
                        UserId = dao.ActiveUser.Id,
                        BidId = DisplayedChat.BidId,
                        PublishStatus = PublishStatus.Unpublished,

                        Text = MessageText ?? string.Empty,
                        Sent = dt,
                        Modified = dt
                    };

                    var imageStream = await imageFile.OpenReadAsync();
                    
                    var uiMdl = newMsg.ToMessageUiModel();
                    uiMdl.Status = MessageStatus.Sending;
                    
                    Messages.Insert(0, uiMdl);
                    Messages = new List<MessageUiModel>(Messages.OrderByDescending(msg => msg.Sent));
                    _messageQueue.Enqueue((newMsg, imageStream, Path.GetExtension(imageFile.FullPath).TrimStart('.')));
                    
                    MessageText = string.Empty;
                }
                catch (Exception ex)
                {
                    HandleError(ex, ex.Message);
                }
            }
        }

        private async System.Threading.Tasks.Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(MessageText))
            {
                try
                {
                    var newMsg = new Message()
                    {
                        ChatId = DisplayedChat.Id,
                        OfficeId = DisplayedChat.OfficeId,
                        TaskId = DisplayedChat.TaskId,
                        UserId = dao.ActiveUser.Id,
                        BidId = DisplayedChat.BidId,
                        PublishStatus = PublishStatus.Published,
                        
                        Text = MessageText,
                        Sent = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    };

                    var uiMdl = newMsg.ToMessageUiModel();
                    uiMdl.Status = MessageStatus.Sending;
                    
                    Messages.Insert(0, uiMdl);
                    Messages = new List<MessageUiModel>(Messages.OrderByDescending(msg => msg.Sent));
                    _messageQueue.Enqueue((newMsg, null, null));

                    MessageText = string.Empty;
                }
                catch (Exception ex)
                {
                    HandleError(ex, ex.Message);
                }
            }
        }
        
        private async System.Threading.Tasks.Task OpenFullImage(MessageUiModel message)
        {
            await view.OpenFullImage(message.ImageUrl);
        }
        
        private async void CheckMessageQueue(object state)
        {
            if (!IsSending && _messageQueue.Any())
            {
                IsSending = true;
                
                (Message Message, Stream ImageStream, string Extension) dequeuedMessageTuple = _messageQueue.Dequeue();
                Debug.WriteLine($"Dequeued message id {dequeuedMessageTuple.Message.Id}");
                try
                {
                    await dao.Add(dequeuedMessageTuple.Message, dequeuedMessageTuple.ImageStream, dequeuedMessageTuple.Extension);
                    Debug.WriteLine($"Sent message id {dequeuedMessageTuple.Message.Id}");
                }
                catch (Exception ex)
                {
                    HandleError(ex, ex.Message);
                }

                IsSending = false;
            }
            else
            {
                userDialogs.HideLoading();
            }
        }
    }
}
