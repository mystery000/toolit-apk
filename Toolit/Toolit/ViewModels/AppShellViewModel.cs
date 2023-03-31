using System;
using System.Linq;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private string _unreadMessagesBadgeValue;

        public string UnreadMessagesBadgeValue
        {
            get => _unreadMessagesBadgeValue;
            set
            {
                _unreadMessagesBadgeValue = value;
                OnPropertyChanged();
            }
        }

        public AppShellViewModel()
        {
            MessagingCenter.Subscribe<AuthMessage>(this, AppConstants.UserLoggedInEventMessage, sender =>
            {
                if (!IsSubscribedToDao)
                {
                    dao.Subscribe(HandleMessageSuccess, HandleError);
                    IsSubscribedToDao = true;
                }
            });
            MessagingCenter.Subscribe<AuthMessage>(this, AppConstants.UserLoggedOutEventMessage, sender =>
            {
                if (IsSubscribedToDao)
                {
                    dao.Unsubscribe(HandleMessageSuccess, HandleError);
                    IsSubscribedToDao = false;
                }
            });
        }

        private void HandleMessageSuccess(Message[] data, string nonce, DateTime updated)
        {
            try
            {
                var unreadMessagesCount = data.Count(msg => !msg.UserId.Equals(dao.ActiveUser?.Id) && !msg.IsRead);
                UnreadMessagesBadgeValue = unreadMessagesCount > 0 ? unreadMessagesCount.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
                HandleError(ex, ex.Message);
            }
        }
    }
}