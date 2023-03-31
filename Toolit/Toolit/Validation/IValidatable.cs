using System.Collections.Generic;
using System.ComponentModel;

namespace Toolit.Validation
{
    public interface IValidatable<T> : IIsValid, INotifyPropertyChanged
    {
        List<IValidationRule<T>> Validations { get; }

        List<string> Errors { get; set; }

        bool Validate();
    }
}
