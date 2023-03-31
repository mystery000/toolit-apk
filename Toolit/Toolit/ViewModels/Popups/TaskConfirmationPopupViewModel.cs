using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class TaskConfirmationPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToShell();
        }
        
        private readonly ICallback view;

        public ICommand MoveToShellCommand { get; }

        public TaskConfirmationPopupViewModel(ICallback view)
        {
            this.view = view;
            
            MoveToShellCommand = new AsyncCommand(MoveToShell);
        }
        
        public async System.Threading.Tasks.Task MoveToShell()
        {
            await view.MoveToShell();
        }
    }
}