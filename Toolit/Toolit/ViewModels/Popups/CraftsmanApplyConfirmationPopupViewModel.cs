using System.Windows.Input;
using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.ViewModels.Popups
{
    public class CraftsmanApplyConfirmationPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToMain();
        }

        private readonly ICallback view;

        public ICommand MoveToMainCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public CraftsmanApplyConfirmationPopupViewModel(ICallback view)
        {
            this.view = view;
            
            BackCommand = new Command(Back);
            MoveToMainCommand = new Command(MoveToMain);
        }

        public async void Back()
        {
            await view.Back();
        }

        public async void MoveToMain()
        {
            await view.MoveToMain();
        }
    }
}