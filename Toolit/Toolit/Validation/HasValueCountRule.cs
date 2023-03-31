using System.Collections.Generic;

namespace Toolit.Validation
{
    public class HasValueCountRule<T> : IValidationRule<IEnumerable<T>>
    {
        public string ValidationMessage { get; set; }
        public int MinimumCount { get; set; } = 0;
        public int MaximumCount { get; set; } = int.MaxValue;
        public T Value { get; set; } = default;

        public bool Check(IEnumerable<T> value)
        {
            if (value == null)
            {
                return false;
            }

            var count = 0;
            foreach (var v in value)
            {
                switch (v)
                {
                    case string g:
                        if (g == Value as string)
                        {
                            count++;
                        }
                        break;
                    case bool b:
                        if (b == Value as bool?)
                        {
                            count++;
                        }
                        break;
                }
            }
            
            return (count >= MinimumCount && count <= MaximumCount);
        }
    }
}
