using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class UserPaymentConfirmationPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
        }

        private readonly ICallback view;

        public ICommand CloseCommand { get; private set; }
        
        public UserPaymentConfirmationPopupViewModel(ICallback view)
        {
            this.view = view;

            CloseCommand = new AsyncCommand(Close);
        }

        private async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
    }
}