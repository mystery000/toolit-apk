using System;
using Toolit.Controls;
using Toolit.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DarkStatusBarPage), typeof(DarkStatusBarPageRenderer))]
namespace Toolit.iOS.Renderers
{
    public class DarkStatusBarPageRenderer : PageRenderer
    {
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.DarkContent;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
        }
    }
}