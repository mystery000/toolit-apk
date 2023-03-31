using System.ComponentModel;
using System.Drawing;
using Toolit;
using Toolit.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace Toolit.iOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control != null)
            {
                Control.Layer.BorderWidth = 0;
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
                return;

            // Check only for Numeric keyboard
            if (this.Element.Keyboard == Keyboard.Numeric)
                this.AddDoneButton();
        }

        
        /// <summary>
        /// <para>Add toolbar with Done button</para>
        /// </summary>
        protected void AddDoneButton()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                {
                    this.Control.ResignFirstResponder();
                    var baseEntry = this.Element.GetType();
                    ((IEntryController)Element).SendCompleted();
                });

                toolbar.Items = new UIBarButtonItem[] {
                    new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                    doneButton
                };
                this.Control.InputAccessoryView = toolbar;
            });
        }
    }
}