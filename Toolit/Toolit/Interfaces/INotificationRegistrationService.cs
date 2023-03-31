using System.Threading.Tasks;

namespace DentMeApp.Shared.Contracts.Services
{
    public interface INotificationRegistrationService
    {
        Task RegisterDeviceAsync(string cachedToken = null);
        Task<(bool isSuccess, string errorMsg)> RegisterForNotificationsAsync();

        Task HandleRemoteNotification();
        Task HandleFailedRegistration(string errorMsg);
    }
}