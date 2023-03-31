using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Toolit;
using Toolit.iOS.Renderers;
using Toolit.Models.Misc;
using UIKit;
using Xam.Shell.Badge.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AppShell), typeof(TabbedShellRenderer))]
namespace Toolit.iOS.Renderers
{
    public class TabbedShellRenderer : ShellRenderer
    {
        protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
        {
            return new PaddedTabBarAppearance();
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem item)
        {
            return new BadgeShellItemRenderer(this)
            {
                ShellItem = item
            };
        }
    }

    public class PaddedTabBarAppearance : ShellTabBarAppearanceTracker
    {
        public override void UpdateLayout(UITabBarController controller)
        {
            base.UpdateLayout(controller);

            var tabBar = controller.TabBar;
            var safeAreaBottomInset =
                UIApplication.SharedApplication.Windows.FirstOrDefault()?.SafeAreaInsets.Bottom ?? 0;

            // account for safe areas on iPhone X+
            if (safeAreaBottomInset > 0)
            {
                var tabBarHeight = 84 + safeAreaBottomInset / 2;

                // change tabbar height
                tabBar.Frame = new CGRect(
                    tabBar.Frame.X,
                    tabBar.Frame.Y + (tabBar.Frame.Height - tabBarHeight),
                    tabBar.Frame.Width,
                    tabBarHeight);

                foreach (var barItem in tabBar.Items)
                {
                    // change tab item margins
                    barItem.TitlePositionAdjustment = new UIOffset(0, 4);
                }

                // setup rounded corners
                var uiBezierPath = UIBezierPath.FromRoundedRect(
                    tabBar.Bounds,
                    UIRectCorner.TopLeft | UIRectCorner.TopRight,
                    new CGSize(15, 15));
                var cAShapeLayer = new CAShapeLayer();

                cAShapeLayer.Frame = tabBar.Bounds;
                cAShapeLayer.Path = uiBezierPath.CGPath;
                tabBar.Layer.Mask = cAShapeLayer;
            }
            else
            {
                var tabBarHeight = 72;

                // change tabbar height
                tabBar.Frame = new CGRect(
                    tabBar.Frame.X,
                    tabBar.Frame.Y + (tabBar.Frame.Height - tabBarHeight),
                    tabBar.Frame.Width,
                    tabBarHeight);

                foreach (var barItem in tabBar.Items)
                {
                    // change tab item margins
                    barItem.ImageInsets = new UIEdgeInsets(-8, 0, 0, 0);
                    barItem.TitlePositionAdjustment = new UIOffset(0, -8);
                }
                
                // setup rounded corners
                var uiBezierPath = UIBezierPath.FromRoundedRect(
                    tabBar.Bounds,
                    UIRectCorner.TopLeft | UIRectCorner.TopRight,
                    new CGSize(15, 15));
                var cAShapeLayer = new CAShapeLayer();

                cAShapeLayer.Frame = tabBar.Bounds;
                cAShapeLayer.Path = uiBezierPath.CGPath;
                tabBar.Layer.Mask = cAShapeLayer;
            }

        }
        
    }
}