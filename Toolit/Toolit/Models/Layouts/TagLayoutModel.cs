namespace Toolit.Models.Layouts
{
    public class TagLayoutModel : INotifyPropertyChangedBase
    {
        private bool _isSelected;
        
        public string ServerId { get; set; }

        public string LocalName { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }
}