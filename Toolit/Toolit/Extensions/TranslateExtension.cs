using System;
using System.Reflection;
using System.Resources;
using Toolit.Resourses;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit
{
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        private static readonly Lazy<ResourceManager> ResourceManager =
            new Lazy<ResourceManager>(() => new ResourceManager("Heimstaden_App.Resourses.AppResources", typeof(TranslateExtension).GetTypeInfo().Assembly));

        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return string.Empty;
            }

            System.Diagnostics.Debug.WriteLine("ALPHA '" + AppResources.Culture + "'");

            var translation = ResourceManager.Value.GetString(Text, AppResources.Culture) ?? Text;

            return translation;
        }
    }
}
