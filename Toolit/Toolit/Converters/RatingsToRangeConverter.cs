using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.Converters
{
    public class RatingsToRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IList<RatingUiModel> ratingsList &&
                parameter is string targetRatingString &&
                int.TryParse(targetRatingString, out int targetRating))
            {
                if (ratingsList.Any())
                {
                    return (double) ratingsList.Count(rtng => rtng.Amount == targetRating) /
                           (double) ratingsList.Count();
                }
            }

            return 0.0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}