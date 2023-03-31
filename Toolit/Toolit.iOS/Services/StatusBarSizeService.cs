using Toolit.Interfaces;
using Toolit.iOS.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBarSizeService))]
namespace Toolit.iOS.Services
{
    public class StatusBarSizeService : IStatusBarSizeService
    {
        public double GetStatusBarHeight()
        {
            return UIApplication.SharedApplication.StatusBarFrame.Size.Height;
        }
    }
}
