using System;
using System.Collections.Generic;

namespace Toolit.Validation
{
    public class ValidatableGroup : INotifyPropertyChangedBase, IIsValid
    {
        private readonly List<IIsValid> objects = new List<IIsValid>();

        public event EventHandler Validated;

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

        public void Add(IIsValid[] o)
        {
            for (var i = 0; i < o.Length; i++)
            {
                Add(o[i]);
            }
        }

        public void Add(IIsValid o)
        {
            o.PropertyChanged += (obj, args) =>
            {
                if (args.PropertyName == "IsValid")
                {
                    var ok = true;
                    for (var i = 0; i < objects.Count; i++)
                    {
                        if (!objects[i].IsValid)
                        {
                            ok = false;
                            break;
                        }
                    }
                    IsValid = ok;
                    Validated?.Invoke(this, EventArgs.Empty);
                }
            };

            objects.Add(o);
        }
    }
}
