using System.Windows.Input;
using Toolit.Models.Layouts;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class CreateTaskModalViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
            System.Threading.Tasks.Task MoveToTips(CraftLayoutModel craftMdl);
        }

        private readonly ICallback view;
        
        private CraftLayoutModel _selectedTaskCraft;

        public CraftLayoutModel SelectedTaskCraft
        {
            get => _selectedTaskCraft;
            set
            {
                _selectedTaskCraft = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; }
        public ICommand MoveToTipsCommand { get; }

        public CreateTaskModalViewModel(ICallback view, CraftLayoutModel craftMdl)
        {
            this.view = view;
            SelectedTaskCraft = craftMdl;

            CloseCommand = new AsyncCommand(Close);
            MoveToTipsCommand = new AsyncCommand(MoveToTips);
        }
        
        public async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
        public async System.Threading.Tasks.Task MoveToTips()
        {
            await view.MoveToTips(SelectedTaskCraft);
        }
    }
}