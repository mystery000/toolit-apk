using Android.Content;
using Google.Android.Material.BottomNavigation;
using Toolit;
using Toolit.Droid.Renderers;
using Xam.Shell.Badge.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
[assembly: ExportRenderer(typeof(AppShell), typeof(ShellTabbarRenderer))]
namespace Toolit.Droid.Renderers
{
    public class ShellTabbarRenderer : ShellRenderer
    {
        public ShellTabbarRenderer(Context context) : base(context)
        {
        }
        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new PaddedTabBarAppearance(this, shellItem);
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        {
            return new BadgeShellItemRenderer(this);
        }
    }
    public class PaddedTabBarAppearance : ShellBottomNavViewAppearanceTracker
    {
        public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            var parameters = bottomView.LayoutParameters;
            parameters.Height = 200;
            bottomView.LayoutParameters = parameters;
            base.SetAppearance(bottomView, appearance);
        }
        public PaddedTabBarAppearance(IShellContext shellContext, ShellItem shellItem) : base(shellContext, shellItem)
        {
        }
    }
}