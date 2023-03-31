using System;
using Toolit.Models.Misc;

namespace Toolit.Models.Ui
{
    public class MessageUiModel : INotifyPropertyChangedBase
    {
        private DateTime _sent;
        private string _text;
        
        private TaskUiModel _task;
        private ChatUiModel _chat;
        private UserUiModel _sender;
        private string _imageUrl;
        private bool _isRead;
        private MessageStatus _status;

        public string Id { get; set; }
        public string OfficeId { get; set; }
        public string TaskId { get; set; }
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public string ChatRecipientId { get; set; }

        public DateTime Sent
        {
            get => _sent;
            set
            {
                _sent = value;
                OnPropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
            }
        }


        public TaskUiModel Task
        {
            get => _task;
            set
            {
                _task = value;
                OnPropertyChanged();
            }
        }

        public ChatUiModel Chat
        {
            get => _chat;
            set
            {
                _chat = value;
                OnPropertyChanged();
            }
        }

        public UserUiModel Sender
        {
            get => _sender;
            set
            {
                _sender = value;
                OnPropertyChanged();
            }
        }

        public bool IsRead
        {
            get => _isRead;
            set
            {
                _isRead = value;
                OnPropertyChanged();
            }
        }

        public MessageStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }
    }
}