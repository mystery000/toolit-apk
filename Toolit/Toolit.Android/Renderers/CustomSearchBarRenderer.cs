
using Toolit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinDemo.Droid.CustomRenderers;

[assembly: ExportRenderer(typeof(CustomSearchBar), typeof(CustomSearchBarRenderer))]
namespace XamarinDemo.Droid.CustomRenderers
{
    public class CustomSearchBarRenderer : SearchBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var plateId = Resources.GetIdentifier("android:id/search_plate", null, null);
                var plate = Control.FindViewById(plateId);
                plate.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}