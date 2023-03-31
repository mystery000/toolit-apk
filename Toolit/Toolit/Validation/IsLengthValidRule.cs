namespace Toolit.Validation
{
    public class IsLengthValidRule<T> : IValidationRule<T>
    {
        public string ValidationMessage { get; set; }
        public int MinimumLength { get; set; } = 0;
        public int MaximumLength { get; set; } = int.MaxValue;

        public bool Check(T value)
        {
            if (value == null)
            {
                return false;
            }

            var g = value as string;
            return (g.Length >= MinimumLength && g.Length <= MaximumLength);
        }
    }
}
