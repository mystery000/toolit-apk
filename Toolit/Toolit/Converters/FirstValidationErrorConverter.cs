using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using Toolit.Resourses;
using Xamarin.Forms;

namespace Toolit
{
	public class FirstValidationErrorConverter : IValueConverter
	{
		private static readonly Lazy<ResourceManager> ResourceManager =
			new Lazy<ResourceManager>(() => new ResourceManager("Toolit.Resourses.AppResources", typeof(FirstValidationErrorConverter).GetTypeInfo().Assembly));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var g = value is ICollection<string> errors && errors.Count > 0 ? errors.ElementAt(0) : null;

			// Translate.
			if (g != null)
			{
				g = ResourceManager.Value.GetString(g, AppResources.Culture) ?? g;
			}

			return g;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
