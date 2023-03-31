namespace Toolit.Interfaces
{
    public interface IAppTrackingService
    {
        bool IsTrackingPermissionDetermined();
        System.Threading.Tasks.Task RequestTrackingPermission();
    }
}