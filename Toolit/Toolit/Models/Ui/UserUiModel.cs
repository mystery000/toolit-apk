namespace Toolit.Models.Ui
{
    public class UserUiModel : INotifyPropertyChangedBase
    {
        private string _preferredName;
        private string _firstName;
        private string _lastName;
        private string _imageUrl;
        private string _email;
        private string _phone;
        private string _address;

        public string Id { get; set; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string PreferredName
        {
            get => _preferredName;
            set
            {
                _preferredName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
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

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public string FullName => $"{PreferredName} {LastName}";
    }
}