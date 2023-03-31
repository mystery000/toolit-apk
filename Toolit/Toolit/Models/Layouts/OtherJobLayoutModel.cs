using System;
using Toolit.Models.Ui;

namespace Toolit.Models.Layouts
{
    public class OtherJobLayoutModel : INotifyPropertyChangedBase
    {
        private TaskUiModel _task;
        private AdUiModel _ad;
        private DateTime _orderDate;

        public TaskUiModel Task
        {
            get => _task;
            set
            {
                _task = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAnAd));
            }
        }

        public AdUiModel Ad
        {
            get => _ad;
            set
            {
                _ad = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAnAd));
            }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set
            {
                _orderDate = value;
                OnPropertyChanged();
            }
        }

        public bool IsAnAd => !ReferenceEquals(Ad, null);
    }
}