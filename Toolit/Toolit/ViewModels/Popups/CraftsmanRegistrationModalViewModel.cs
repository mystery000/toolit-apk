using System.Windows.Input;
using Toolit.Models.Layouts;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class CraftsmanRegistrationModalViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
            System.Threading.Tasks.Task MoveToApply();
            System.Threading.Tasks.Task MoveToApplyNewCraft();
        }

        private readonly ICallback view;
        private readonly bool _isACraftsman;

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
        public ICommand MoveToApplyCommand { get; }

        public CraftsmanRegistrationModalViewModel(ICallback view, bool isACraftsman)
        {
            this.view = view;
            _isACraftsman = isACraftsman;

            CloseCommand = new AsyncCommand(Close);
            MoveToApplyCommand = new AsyncCommand(MoveToApply);
        }
        
        public async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
        
        public async System.Threading.Tasks.Task MoveToApply()
        {
            if (!_isACraftsman)
            {
                await view.MoveToApply();
            }
            else
            {
                await view.MoveToApplyNewCraft();
            }
        }
    }
}