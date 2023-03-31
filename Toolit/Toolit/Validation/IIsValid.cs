using System.ComponentModel;

namespace Toolit.Validation
{
    public interface IIsValid : INotifyPropertyChanged
    {
        bool IsValid { get; set; }
    }
}
