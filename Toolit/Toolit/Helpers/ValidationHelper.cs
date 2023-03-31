using System.Linq;
using System.Text.RegularExpressions;

namespace Toolit.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsPersonalNumberValid(string personalNumber)
        {
            return !string.IsNullOrWhiteSpace(personalNumber);
        }
        
        public static bool IsFirstNameValid(string firstName)
        {
            return !string.IsNullOrWhiteSpace(firstName);
        }

        public static bool IsLastNameValid(string lastName)
        {
            return !string.IsNullOrWhiteSpace(lastName);
        }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.All(char.IsDigit);
        }


        public static bool IsAddressValid(string address)
        {
            return !string.IsNullOrWhiteSpace(address);
        }

        public static bool IsPostalCodeValid(string postalCode)
        {
            return !string.IsNullOrWhiteSpace(postalCode);
        }

        public static bool IsCountyValid(string county)
        {
            return !string.IsNullOrWhiteSpace(county);
        }


        public static bool IsEmailValid(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static bool IsPasswordValid(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length > 7;
        }
    }
}