﻿using System.Collections.Generic;
using System.Linq;

namespace Toolit.Validation
{
    public class ValidatablePair<T> : INotifyPropertyChangedBase, IValidatable<ValidatablePair<T>>
    {
        public List<IValidationRule<ValidatablePair<T>>> Validations { get; } = new List<IValidationRule<ValidatablePair<T>>>();

        public bool IsValid { get; set; } = true;

        public List<string> Errors { get; set; } = new List<string>();

        public ValidatableObject<T> Item1 { get; set; } = new ValidatableObject<T>();

        public ValidatableObject<T> Item2 { get; set; } = new ValidatableObject<T>();

        public ValidatablePair()
        {
            // Subscribe to changes.
            Item1.PropertyChanged += (obj, args) =>
            {
                Validate();
                OnPropertyChanged("IsValid");
                OnPropertyChanged("Errors");
            };
            Item2.PropertyChanged += (obj, args) =>
            {
                Validate();
                OnPropertyChanged("IsValid");
                OnPropertyChanged("Errors");
            };
        }

        public bool Validate()
        {
            var item1IsValid = Item1.Validate();
            var item2IsValid = Item2.Validate();
            if (item1IsValid && item2IsValid)
            {
                Errors.Clear();

                IEnumerable<string> errors = Validations.Where(v => !v.Check(this))
                    .Select(v => v.ValidationMessage);

                Errors = errors.ToList();
                Item2.Errors = Errors;
                Item2.IsValid = !Errors.Any();
            }

            IsValid = !Item1.Errors.Any() && !Item2.Errors.Any();

            return this.IsValid;
        }
    }
}
