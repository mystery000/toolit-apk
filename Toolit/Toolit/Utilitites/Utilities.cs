using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Toolit
{
    public class Utilities
    {
        public static DateTime DateTimeFromRFC3339(string t)
        {
            return XmlConvert.ToDateTime(t, XmlDateTimeSerializationMode.Utc);
        }

        public static bool ValidateEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static bool ValidatePassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) &&
                   Regex.IsMatch(password, @"^.{6,}$");
        }

        public static bool ValidateRepeatPassword(string password, string passwordAgain)
        {
            return !string.IsNullOrWhiteSpace(passwordAgain) &&
                   string.Compare(password, passwordAgain, StringComparison.InvariantCulture) == 0;
        }

        public static bool ValidatePhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) &&
                   Regex.IsMatch(phone, @"^(\+[0-9]{10,11})$");
        }

        public static byte[] GetImageStreamAsBytes(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static DateTime ParseISO8601Date(string g)
        {
            if (g == null)
            {
                throw new NullReferenceException("Cannot parse string as date.");
            }

            string[] dateFormats = { 
                // UTC
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fZ",
                "yyyy-MM-ddTHH:mm:ss.ffZ",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:ss.ffffZ",
                "yyyy-MM-ddTHH:mm:ss.fffffZ",
                "yyyy-MM-ddTHH:mm:ss.ffffffZ",
                "yyyy-MM-ddTHH:mm:ss.fffffffZ",
                "yyyy-MM-ddTHH:mm:ss.ffffffffZ",
                "yyyy-MM-ddTHH:mm:ss.fffffffffZ",
                "yyyy-MM-ddTHH:mm:ss.ffffffffffZ",
                // With offset.
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm:ss.f",
                "yyyy-MM-ddTHH:mm:ss.ff",
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss.ffff",
                "yyyy-MM-ddTHH:mm:ss.fffff",
                "yyyy-MM-ddTHH:mm:ss.ffffff",
                "yyyy-MM-ddTHH:mm:ss.fffffff",
                "yyyy-MM-ddTHH:mm:ss.ffffffff",
                "yyyy-MM-ddTHH:mm:ss.fffffffff",
                "yyyy-MM-ddTHH:mm:ss.ffffffffff",
                // Without tz.
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.fzzz",
                "yyyy-MM-ddTHH:mm:ss.ffzzz",
                "yyyy-MM-ddTHH:mm:ss.fffzzz",
                "yyyy-MM-ddTHH:mm:ss.ffffzzz",
                "yyyy-MM-ddTHH:mm:ss.fffffzzz",
                "yyyy-MM-ddTHH:mm:ss.ffffffzzz",
                "yyyy-MM-ddTHH:mm:ss.fffffffzzz",
                "yyyy-MM-ddTHH:mm:ss.ffffffffzzz",
                "yyyy-MM-ddTHH:mm:ss.fffffffffzzz",
                "yyyy-MM-ddTHH:mm:ss.ffffffffffzzz",
            };

            // Truncate to max 7 digits of milliseconds due to C# limitations.
            var p = g.IndexOf('.');
            if (p >= 0)
            {
                int end;
                if (g[g.Length - 1] == 'Z')
                {
                    end = g.Length - 1;
                }
                else
                {
                    var s = g.IndexOf('+');
                    if (s >= 0)
                    {
                        end = s;
                    }
                    else
                    {
                        s = g.IndexOf('-');
                        if (s >= 0)
                        {
                            end = s;
                        }
                        else
                        {
                            end = -1;
                        }
                    }
                }

                if (end >= 0 && end - p > 7)
                {
                    g = g.Substring(0, p + 8) + g.Substring(end);
                }
            }

            return DateTime.ParseExact(g, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime();
        }

        public static DayOfWeek GetStartWeekDay()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        }
    }
}