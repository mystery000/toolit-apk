using System;
using Toolit.Resourses;

namespace Toolit.Models.Ui
{
    public class BidUiModel : INotifyPropertyChangedBase
    {
        private decimal _price;
        private string _bidMessage;
        private decimal _labourCost;
        private decimal _materialCost;
        private decimal _vat;
        private decimal _rootDeduction;
        private decimal _finalBid;
        private bool _isCancelled;
        private TaskUiModel _task;
        private CraftsmanUiModel _craftsman;
        private decimal _brokerageFee;

        public string Id { get; set; }
        public string TaskId { get; set; }
        public string OfficeId { get; set; }
        public string CraftsmanId { get; set; }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public string BidMessage
        {
            get => _bidMessage;
            set
            {
                _bidMessage = value;
                OnPropertyChanged();
            }
        }

        public decimal LabourCost
        {
            get => _labourCost;
            set
            {
                _labourCost = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LabourCostWithVat));
            }
        }

        public decimal LabourCostWithVat
        {
            get
            {
                var vat = Math.Round(LabourCost * AppConstants.VatRate, 2, MidpointRounding.ToEven);
                return LabourCost + vat;
            }
        }

        public decimal MaterialCost
        {
            get => _materialCost;
            set
            {
                _materialCost = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MaterialCostWithVat));
            }
        }
        public decimal MaterialCostWithVat
        {
            get
            {
                var vat = Math.Round(MaterialCost * AppConstants.VatRate, 2, MidpointRounding.ToEven);
                return MaterialCost + vat;
            }
        }

        public decimal Vat
        {
            get => _vat;
            set
            {
                _vat = value;
                OnPropertyChanged();
            }
        }

        public decimal RootDeduction
        {
            get => _rootDeduction;
            set
            {
                _rootDeduction = value;
                OnPropertyChanged();
            }
        }

        public decimal FinalBid
        {
            get => _finalBid;
            set
            {
                _finalBid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedFinalBid));
            }
        }

        public bool IsCancelled
        {
            get => _isCancelled;
            set
            {
                _isCancelled = value;
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

        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
                OnPropertyChanged();
            }
        }

        public decimal BrokerageFee
        {
            get => _brokerageFee;
            set
            {
                _brokerageFee = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FormattedBrokerageFee));
            }
        }

        public string FormattedFinalBid => $"{FinalBid:N2}kr";
        
        public string FormattedBrokerageFee => string.Format(AppResources.BrokerageFeeFormatString, (FinalBid * BrokerageFee));
    }
}