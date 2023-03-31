using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Toolit.Models.Layouts;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class CraftsmanApplyTypeViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToApplyCompany(CraftsmanUiModel newCraftsmanModel);
        }

        private readonly ICallback view;
        
        
        private ObservableCollection<CraftLayoutModel> _craftList;
        public ObservableCollection<CraftLayoutModel> CraftList
        {
            get => _craftList;
            set
            {
                _craftList = value;
                OnPropertyChanged();
            }
        }

        public bool CanContinue => CraftList?.Any(crft => crft.IsSelected) ?? false;
        
        public ICommand CraftTappedCommand { get; private set; }
        public ICommand MoveToApplyCompanyCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        

        public CraftsmanApplyTypeViewModel(ICallback view)
        {
            this.view = view;
            
            BackCommand = new Command(Back);
            CraftTappedCommand = new AsyncCommand<CraftLayoutModel>(CraftTapped);
            MoveToApplyCompanyCommand = new Command(MoveToApplyCompany);

            CraftList = new ObservableCollection<CraftLayoutModel>(AppConstants.CraftModels);
            foreach (var craft in CraftList)
            {
                craft.IsSelected = false;
            }
        }

        public async void Back()
        {
            await view.Back();
        }

        private async System.Threading.Tasks.Task CraftTapped(CraftLayoutModel craft)
        {
            if (!craft.IsSelected)
            {
                foreach (var craftLayoutModel in CraftList)
                {
                    craftLayoutModel.IsSelected = false;
                }

                craft.IsSelected = true;
            }
            else
            {
                craft.IsSelected = false;
            }

            OnPropertyChanged(nameof(CanContinue));
        }
        
        public async void MoveToApplyCompany()
        {
            await view.MoveToApplyCompany(new CraftsmanUiModel()
            {
                Crafts = CraftList
                    .Where(crft => crft.IsSelected)
                    .Select(crft => new CraftUiModel() {CraftType = crft.ServerId})
                    .ToList()
            });
        }

    }
}