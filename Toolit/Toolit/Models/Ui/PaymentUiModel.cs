using System;

namespace Toolit.Models.Ui
{
    public class PaymentUiModel : INotifyPropertyChangedBase
    {
        private string _paymentState;
        private string _paymentMethod;
        private decimal _amount;
        private string _currency;
        
        private TaskUiModel _task;
        private BidUiModel _bid;
        private CraftsmanUiModel _craftsman;
        private DateTime _modified;

        public string Id { get; set; }
        public string OfficeId { get; set; }
        public string TaskId { get; set; }
        public string BidId { get; set; }
        public string CraftsmanId { get; set; }
        public string SwishPaymentId { get; set; }

        public string PaymentState
        {
            get => _paymentState;
            set
            {
                _paymentState = value;
                OnPropertyChanged();
            }
        }

        public string PaymentMethod
        {
            get => _paymentMethod;
            set
            {
                _paymentMethod = value;
                OnPropertyChanged();
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public string Currency
        {
            get => _currency;
            set
            {
                _currency = value;
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

        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
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