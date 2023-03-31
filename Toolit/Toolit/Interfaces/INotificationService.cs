using System.Threading.Tasks;

namespace Toolit.Interfaces
{
    public interface INotificationService
    {
        bool HasNotificationPermission();
        Task<(bool isSuccess, string error)> RequestNotificationPermissionAndRegister();
    }
}