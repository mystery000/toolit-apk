using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class DataPolicyViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        public ICommand BackCommand { get; private set; }

        public DataPolicyViewModel(ICallback view)
        {
            this.view = view;
            BackCommand = new Command(Back);
        }

        public async void Back()
        {
            await view.Back();
        }
    }
}
