using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Toolit.Validation
{
    public class IsValuePositiveRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }

        public bool Check(T value)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()) || !decimal.TryParse(value.ToString().Replace(',', '.'), 
                NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
            {
                return false;
            }

            return val >= Decimal.Zero;
        }
    }
}
