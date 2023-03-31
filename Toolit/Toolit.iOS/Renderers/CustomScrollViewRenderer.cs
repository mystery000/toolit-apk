using Toolit;
using Toolit.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomScrollView), typeof(CustomScrollViewRenderer))]
namespace Toolit.iOS
{
    public class CustomScrollViewRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            Bounces = false;
        }
    }
}