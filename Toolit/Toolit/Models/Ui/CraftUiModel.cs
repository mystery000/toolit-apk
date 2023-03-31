namespace Toolit.Models.Ui
{
    public class CraftUiModel : INotifyPropertyChangedBase
    {
        private CraftStatus _status;
        private string _craftType;

        public string Id { get; set; }
        public string CertificateId { get; set; }

        public CraftStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public string CraftType
        {
            get => _craftType;
            set
            {
                _craftType = value;
                OnPropertyChanged();
            }
        }
    }
}