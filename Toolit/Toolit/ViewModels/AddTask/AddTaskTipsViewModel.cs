using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Models.Layouts;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AddTaskTipsViewModel : BaseViewModel
    {

        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task Skip();
            System.Threading.Tasks.Task MoveToMedia(CraftLayoutModel craftMdl);
        }

        private readonly ICallback view;
        
        public User User { get; set; }
        public CraftLayoutModel CraftMdl { get; }
        
        public ICommand MoveToMediaCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        
        public AddTaskTipsViewModel(ICallback view, CraftLayoutModel craftMdl)
        {
            CraftMdl = craftMdl;
            this.view = view;
            
            BackCommand = new Command(Back);
            MoveToMediaCommand = new Command(MoveToMedia);

            User = dao.ActiveUser;
        }

        public async void Back()
        {
            await view.Back();
        }

        public async void MoveToMedia()
        {
            await view.MoveToMedia(CraftMdl);
        }

        public async void Skip()
        {
            await view.Skip();
        }
    }
}
