using System;
using System.Collections.Generic;
using System.Linq;
using Toolit.Resourses;

namespace Toolit.Models.Ui
{
    public class CraftsmanUiModel : INotifyPropertyChangedBase
    {
        private List<CraftUiModel> _crafts;
        private string _aboutText;
        private string _aboutHeader;
        private string _companyName;
        private string _orgNumber;
        private string _companyAddress;
        private string _workArea;
        private int _completedJobs;
        private DateTime _memberSince;
        private string _craftsmanName;
        private string _accountNumber;
        private List<RatingUiModel> _ratings;
        private bool _fTax;
        private UserUiModel _user;
        private bool _isDeleted;
        private bool _isFrozen;

        public string Id { get; set; }
        public string OfficeId { get; set; }
        public string UserId { get; set; }

        public List<CraftUiModel> Crafts
        {
            get => _crafts;
            set
            {
                _crafts = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Craft));
                OnPropertyChanged(nameof(NumberOfWorkCertificates));
            }
        }
        
        public string Craft => AppConstants.CraftModels.FirstOrDefault(crft => 
            crft.ServerId.Equals(Crafts?.FirstOrDefault()?.CraftType ?? string.Empty))?.LocalName ?? string.Empty;

        public int NumberOfWorkCertificates => Crafts.Count(crft => crft.Status == CraftStatus.Approved);

        public string AboutText
        {
            get => _aboutText;
            set
            {
                _aboutText = value;
                OnPropertyChanged();
            }
        }

        public string AboutHeader
        {
            get => _aboutHeader;
            set
            {
                _aboutHeader = value;
                OnPropertyChanged();
            }
        }

        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                OnPropertyChanged();
            }
        }

        public string OrgNumber
        {
            get => _orgNumber;
            set
            {
                _orgNumber = value;
                OnPropertyChanged();
            }
        }

        public string CompanyAddress
        {
            get => _companyAddress;
            set
            {
                _companyAddress = value;
                OnPropertyChanged();
            }
        }

        public string WorkArea
        {
            get => _workArea;
            set
            {
                _workArea = value;
                OnPropertyChanged();
            }
        }

        public int CompletedJobs
        {
            get => _completedJobs;
            set
            {
                _completedJobs = value;
                OnPropertyChanged();
            }
        }

        public DateTime MemberSince
        {
            get => _memberSince;
            set
            {
                _memberSince = value;
                OnPropertyChanged();
            }
        }

        public string CraftsmanName
        {
            get => _craftsmanName;
            set
            {
                _craftsmanName = value;
                OnPropertyChanged();
            }
        }

        public string AccountNumber
        {
            get => _accountNumber;
            set
            {
                _accountNumber = value;
                OnPropertyChanged();
            }
        }

        public List<RatingUiModel> Ratings
        {
            get => _ratings;
            set
            {
                _ratings = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AverageRating));
            }
        }

        public bool FTax
        {
            get => _fTax;
            set
            {
                _fTax = value;
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

        public bool IsDeleted
        {
            get => _isDeleted;
            set
            {
                _isDeleted = value;
                OnPropertyChanged();
            }
        }

        public bool IsFrozen
        {
            get => _isFrozen;
            set
            {
                _isFrozen = value;
                OnPropertyChanged();
            }
        }

        public double AverageRating =>
            Ratings.Count > 0 ? (double) Ratings.Sum(rtng => rtng.Amount) / Ratings.Count : 0.0d;
    }
}