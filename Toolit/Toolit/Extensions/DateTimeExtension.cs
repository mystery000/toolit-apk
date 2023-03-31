using System;
using System.Globalization;
using System.Xml;

namespace Toolit
{
    public static class DateTimeExtension
    {
        public static string ToRFC3339(this DateTime value)
        {
            return XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);
        }
        
        public static DateTime FromRFC3339(this string value)
        {
            return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime dateDone)
                ? dateDone
                : DateTime.MinValue;
        }

        public static bool EqualsWithResolution(this DateTime t1, DateTime t2, TimeSpan s)
        {
            return t1.CompareToWithResolution(t2, s) == 0;
        }

        // Negative == precedes.
        public static int CompareToWithResolution(this DateTime t1, DateTime t2, TimeSpan s)
        {
            var x = t1.Year - t2.Year;
            if (x != 0)
            {
                return x;
            }

            x = t1.Month - t2.Month;
            if (x != 0)
            {
                return x;
            }

            if (s.Days > 0 || s.Hours > 0 || s.Minutes > 0)
            {
                x = t1.Day - t2.Day;
                if (x != 0)
                {
                    return x;
                }
            }

            if (s.Hours > 0 || s.Minutes > 0)
            {
                x = t1.Hour - t2.Hour;
                if (x != 0)
                {
                    return x;
                }
            }

            if (s.Minutes > 0)
            {
                x = t1.Minute - t2.Minute;
                if (x != 0)
                {
                    return x;
                }
            }

            return 0;
        }

        public static int ChangeFirstDayOfWeek(this DayOfWeek dayOfWeek)
        {
            var shift = (int)dayOfWeek - (int)Utilities.GetStartWeekDay();

            return shift >= 0 ? shift : (7 + shift);
        }

        public static DateTime Normalize(this DateTime dt, bool convertToUtc = true)
        {
            if (convertToUtc)
            {
                return dt.ToUniversalTime().AddSeconds(-dt.Second).AddMilliseconds(-dt.Millisecond);
            }
            else
            {
                return dt.AddSeconds(-dt.Second).AddMilliseconds(-dt.Millisecond);
            }
        }

        public static DateTime GetClosestMidnightInTimezone(this DateTime dateTime, TimeZoneInfo tz)
        {
            if (tz == null) { return DateTime.MinValue; }
            if (tz.GetUtcOffset(dateTime) >= TimeZoneInfo.Local.GetUtcOffset(dateTime) &&
                dateTime.Date.Equals(dateTime)) { return dateTime; }

            var tzTime = TimeZoneInfo.ConvertTime(dateTime, tz); // convert time to timezone
            var tzMidnight = tzTime.Date.AddDays(1); // get closest midnight

            return tzMidnight;
        }
    }
}