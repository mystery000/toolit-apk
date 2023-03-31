using System.Collections.ObjectModel;

namespace Toolit.Models.Layouts
{
    public class CraftLayoutModel : INotifyPropertyChangedBase
    {
        private bool _isSelected;
        
        public string ServerId { get; set; }
        public int Code { get; set; }
        public string LocalName { get; set; }
        public string LocalDescription { get; set; }
        public string AddTaskIcon { get; set; }
        public string FilterIcon { get; set; }
        public string LocalFullHeader { get; set; }
        public string LocalFullDescription { get; set; }
        public string LocalJobDescriptionPlaceholder { get; set; }

        public ObservableCollection<TagLayoutModel> Tags { get; set; }

        public virtual bool IsSelected
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