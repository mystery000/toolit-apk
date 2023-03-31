using Android.Content;
using Android.Content.Res;
using Android.Text;
using Toolit;
using Toolit.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]

namespace Toolit.Droid
{
    public class CustomEditorRenderer : EditorRenderer
    {
        public CustomEditorRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Background = null;
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}