using System;
using System.ComponentModel;
using Foundation;
using Toolit;
using Toolit.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]

namespace Toolit.iOS
{
    class CustomEditorRenderer : EditorRenderer
    {
        private UILabel placeholderLabel;
        private NSLayoutConstraint[] vConstraints;
        private NSLayoutConstraint[] hConstraints;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName != nameof(CustomEditor.Text))
                return;

            if (Element == null || Control == null)
                return;

            Control.BackgroundColor = UIColor.Clear;

            CustomEditor editor = (CustomEditor)Element;
            if (editor.Text != "")
            {
                if (placeholderLabel == null)
                    FixHorizontalScroll();
            }
            else
            {
                if (placeholderLabel != null)
                    UndoFixHorizontalScroll();
            }
        }

        private void UndoFixHorizontalScroll()
        {
            placeholderLabel.RemoveFromSuperview();
            placeholderLabel = null;

            Control.RemoveConstraints(vConstraints);
            vConstraints = null;

            Control.RemoveConstraints(hConstraints);
            hConstraints = null;
        }

        private void FixHorizontalScroll()
        {
            placeholderLabel = new UILabel();

            UIEdgeInsets edgeInsets = Control.TextContainerInset;
            nfloat lineFragmentPadding = Control.TextContainer.LineFragmentPadding;

            Control.AddSubview(placeholderLabel);

            vConstraints = NSLayoutConstraint.FromVisualFormat(
                "V:|-" + edgeInsets.Top + "-[PlaceholderLabel]-" + edgeInsets.Bottom + "-|", 0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { placeholderLabel }, new NSObject[] { new NSString("PlaceholderLabel") })
            );

            hConstraints = NSLayoutConstraint.FromVisualFormat(
                "H:|-" + lineFragmentPadding + "-[PlaceholderLabel]-" + lineFragmentPadding + "-|",
                0, new NSDictionary(),
                NSDictionary.FromObjectsAndKeys(
                    new NSObject[] { placeholderLabel }, new NSObject[] { new NSString("PlaceholderLabel") })
            );

            placeholderLabel.TranslatesAutoresizingMaskIntoConstraints = true;

            Control.AddConstraints(hConstraints);
            Control.AddConstraints(vConstraints);

            Control.SetNeedsLayout();
            Control.LayoutIfNeeded();
        }
    }
}