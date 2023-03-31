using Toolit.Droid.Services;
using Toolit.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(AppTrackingService))]
namespace Toolit.Droid.Services
{
    public class AppTrackingService : IAppTrackingService
    {
        public bool IsTrackingPermissionDetermined()
        {
            throw new System.NotImplementedException();
        }

        public System.Threading.Tasks.Task RequestTrackingPermission()
        {
            throw new System.NotImplementedException();
        }
    }
}