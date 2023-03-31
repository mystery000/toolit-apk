using System;

namespace Toolit.Models.Ui
{
    public class ChatUiModel : INotifyPropertyChangedBase
    {
        private DateTime _modified;
        private MessageUiModel _lastMessage;
        private TaskUiModel _task;
        private BidUiModel _bid;
        private UserUiModel _chatRecipient;

        public string Id { get; set; }
        public string OfficeId { get; set; }
        public string TaskId { get; set; }
        public string BidId { get; set; }

        public DateTime Modified
        {
            get => _modified;
            set
            {
                _modified = value;
                OnPropertyChanged();
            }
        }

        public MessageUiModel LastMessage
        {
            get => _lastMessage;
            set
            {
                _lastMessage = value;
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

        public BidUiModel Bid
        {
            get => _bid;
            set
            {
                _bid = value;
                OnPropertyChanged();
            }
        }

        public UserUiModel ChatRecipient
        {
            get => _chatRecipient;
            set
            {
                _chatRecipient = value;
                OnPropertyChanged();
            }
        }
    }
}