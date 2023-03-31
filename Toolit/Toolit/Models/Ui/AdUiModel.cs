using System;

namespace Toolit.Models.Ui
{
    public class AdUiModel : INotifyPropertyChangedBase
    {
        private string _company;
        private string _title;
        private string _text;
        private string _url;
        private string _imageUrl;
        private DateTime _modified;

        public string Id { get; set; }

        public string Company
        {
            get => _company;
            set
            {
                _company = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
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

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
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

        public DateTime Modified
        {
            get => _modified;
            set
            {
                _modified = value;
                OnPropertyChanged();
            }
        }
    }
}