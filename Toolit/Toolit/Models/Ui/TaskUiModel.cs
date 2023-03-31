using System;
using System.Collections.Generic;
using System.Linq;
using Toolit.Resourses;
using Xamarin.Forms;

namespace Toolit.Models.Ui
{
    public class TaskUiModel : INotifyPropertyChangedBase
    {
        private string _title;
        private string _description;
        private DateTime _dateDone;
        private DateTime _modified;
        private string _address;
        private string _postcode;
        private string _city;
        private decimal _price;
        private IList<string> _imageUrls;
        public string _titleImageUrl;
        private IList<string> _videoUrls;
        private IList<string> _crafts;
        private IList<string> _tags;
        private string _acceptedBid;
        private bool _finished;
        private bool? _rated;
        private string _propertyDesignation;
        private string _realEstateUnion;
        private string _apartmentNumber;
        private bool _useRotRut;
        private bool _showPublicly;
        private bool _isBiddable;
        private bool _hasUsersBid;

        public string Id { get; set; }
        public string OfficeId { get; set; }
        public string UserId { get; set; }
        public string PaymentId { get; set; }
        
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateDone
        {
            get => _dateDone;
            set
            {
                _dateDone = value;
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

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        public string Postcode
        {
            get => _postcode;
            set
            {
                _postcode = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
                OnPropertyChanged(FormattedBubbleText);
            }
        }

        public string FormattedBubbleText => Price > Decimal.Zero
            ? string.Concat(Price.ToString("N0"), AppResources.CurrencyFormatString)
            : City;

        public IList<string> ImageUrls
        {
            get => _imageUrls;
            set
            {
                _imageUrls = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitleImageUrl));
            }
        }
        
        public string TitleImageUrl
        {
            get => _titleImageUrl;
            set
            {
                _titleImageUrl = value;
                OnPropertyChanged();
            }
        }

        public IList<string> VideoUrls
        {
            get => _videoUrls;
            set
            {
                _videoUrls = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitleVideoUrl));
                OnPropertyChanged(nameof(HasVideo));
            }
        }
        
        public string TitleVideoUrl => VideoUrls?.FirstOrDefault();

        public bool HasVideo => !string.IsNullOrEmpty(TitleVideoUrl);

        public IList<string> Crafts
        {
            get => _crafts;
            set
            {
                _crafts = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Craft));
            }
        }
        
        public string Craft => AppConstants.CraftModels.FirstOrDefault(crft => 
            crft.ServerId.Equals(Crafts?.FirstOrDefault() ?? string.Empty))?.LocalName ?? string.Empty;
        
        public IList<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        public string AcceptedBid
        {
            get => _acceptedBid;
            set
            {
                _acceptedBid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasAcceptedBid));
                OnPropertyChanged(nameof(BiddingStatus));
            }
        }

        public bool HasAcceptedBid => !string.IsNullOrWhiteSpace(AcceptedBid);

        public string BiddingStatus
        {
            get
            {
                if (HasAcceptedBid)
                {
                    return Finished
                        ? AppResources.TaskListItemCompletedStatus
                        : AppResources.TaskListItemInProgressStatus;
                }

                return AppResources.TaskListItemBiddingStatus;
            }
        }

        public bool Finished
        {
            get => _finished;
            set
            {
                _finished = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BiddingStatus));
            }
        }

        public bool? Rated
        {
            get => _rated;
            set
            {
                _rated = value;
                OnPropertyChanged();
            }
        }

        public string PropertyDesignation
        {
            get => _propertyDesignation;
            set
            {
                _propertyDesignation = value;
                OnPropertyChanged();
            }
        }

        public string RealEstateUnion
        {
            get => _realEstateUnion;
            set
            {
                _realEstateUnion = value;
                OnPropertyChanged();
            }
        }

        public string ApartmentNumber
        {
            get => _apartmentNumber;
            set
            {
                _apartmentNumber = value;
                OnPropertyChanged();
            }
        }

        public bool UseRotRut
        {
            get => _useRotRut;
            set
            {
                _useRotRut = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPublicly
        {
            get => _showPublicly;
            set
            {
                _showPublicly = value;
                OnPropertyChanged();
            }
        }

        public bool IsBiddable
        {
            get => _isBiddable;
            set
            {
                _isBiddable = value;
                OnPropertyChanged();
            }
        }

        public bool HasUsersBid
        {
            get => _hasUsersBid;
            set
            {
                _hasUsersBid = value;
                OnPropertyChanged();
            }
        }
    }
}