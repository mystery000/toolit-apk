using AppTrackingTransparency;
using Toolit.Interfaces;
using Toolit.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppTrackingService))]
namespace Toolit.iOS.Services
{
    public class AppTrackingService : IAppTrackingService
    {
        public bool IsTrackingPermissionDetermined()
        {
            return ATTrackingManager.TrackingAuthorizationStatus != ATTrackingManagerAuthorizationStatus.NotDetermined;
        }

        public async System.Threading.Tasks.Task RequestTrackingPermission()
        {
            if (UIDevice.CurrentDevice
                .CheckSystemVersion(14, 0))
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var trackingStatus = await ATTrackingManager.RequestTrackingAuthorizationAsync();
                });
            }
        }
    }
}