using System.Collections.Generic;
using System.Linq;

namespace Toolit.Validation
{
    public class ValidatableObject<T> : INotifyPropertyChangedBase, IValidatable<T>
    {
        public List<IValidationRule<T>> Validations { get; } = new List<IValidationRule<T>>();

        public List<string> Errors { get; set; } = new List<string>();

        public bool ValidateOnChange { get; set; } = true;

        T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;

                OnPropertyChanged("Value");
                if (ValidateOnChange)
                {
                    Validate();
                    OnPropertyChanged("Errors");
                }

                if (!IsChanged)
                {
                    IsChanged = true;
                }
            }
        }

        private bool isChanged = false;
        public bool IsChanged
        {
            get
            {
                return isChanged;
            }

            set
            {
                if (isChanged != value)
                {
                    isChanged = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsInvalidAndChanged");
                }
            }
        }

        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                return isValid;
            }

            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsInvalidAndChanged");
                }
                else
                {
                    isValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsInvalidAndChanged
        {
            get
            {
                return !IsValid && IsChanged;
            }
        }

        public virtual bool Validate()
        {
            Errors.Clear();

            IEnumerable<string> errors = Validations.Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage);

            Errors = errors.ToList();
            OnPropertyChanged("Errors");
            IsValid = !Errors.Any();

            return this.IsValid;
        }

        public virtual void AddError(string error)
        {
            Errors.Add(error);
            OnPropertyChanged("Errors");
            IsValid = false;
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
