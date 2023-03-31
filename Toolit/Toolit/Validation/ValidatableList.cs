using System.Collections.Generic;
using System.Linq;

namespace Toolit.Validation
{
    public class ValidatableList<T> : INotifyPropertyChangedBase, IIsValid
    {
        public List<IValidationRule<IEnumerable<T>>> Validations { get; } = new List<IValidationRule<IEnumerable<T>>>();

        public List<string> Errors { get; set; } = new List<string>();

        private readonly List<ValidatableObject<T>> objects = new List<ValidatableObject<T>>();

        private bool isValid = false;
        public bool IsValid
        {
            get
            {
                return isValid;
            }

            set
            {
                isValid = value;
                OnPropertyChanged();
            }
        }

        public void Add(ValidatableObject<T>[] o)
        {
            for (var i = 0; i < o.Length; i++)
            {
                Add(o[i]);
            }
        }

        public void Add(ValidatableObject<T> o)
        {
            o.PropertyChanged += (obj, args) =>
            {
                if (args.PropertyName == "Value")
                {
                    // Check rules.
                    var values = new List<T>(objects.Count);
                    for (var i = 0; i < objects.Count; i++) {
                        values.Add(objects[i].Value);
                    }

                    IEnumerable<string> errors = Validations.Where(v => !v.Check(values)).Select(v => v.ValidationMessage);
                    Errors = errors.ToList();
                    OnPropertyChanged("Errors");
                    IsValid = !Errors.Any();
                }
            };

            objects.Add(o);
        }

        public virtual void AddError(string error)
        {
            Errors.Add(error);
            OnPropertyChanged("Errors");
            IsValid = false;
        }
    }
}

