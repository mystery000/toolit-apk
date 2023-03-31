using System;

namespace Toolit.Models.Ui
{
    public class RatingUiModel : INotifyPropertyChangedBase
    {
        private string _header;
        private string _text;
        private DateTime _created;
        private int _amount;
        private CraftsmanUiModel _craftsman;
        private UserUiModel _user;

        public string Id { get; set; }
        public string OfficeId { get; set; }
        public string CraftsmanId { get; set; }
        public string TaskId { get; set; }
        public string UserId { get; set; }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
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

        public DateTime Created
        {
            get => _created;
            set
            {
                _created = value;
                OnPropertyChanged();
            }
        }

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
                OnPropertyChanged();
            }
        }

        public UserUiModel User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
    }
}