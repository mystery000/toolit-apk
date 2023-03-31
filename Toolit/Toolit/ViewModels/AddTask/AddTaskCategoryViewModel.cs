using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Layouts;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AddTaskCategoryViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToToS();
            System.Threading.Tasks.Task OpenCreateTaskPopup(CraftLayoutModel craftMdl);
            System.Threading.Tasks.Task OpenCraftsmanRegistrationPopup(bool isACraftsman);
        }

        private readonly ICallback view;
        
        public ICommand MoveToToSCommand { get; }
        public ICommand OpenCreateTaskPopupCommand { get; }
        public ICommand OpenCraftsmanRegistrationPopupCommand { get; }


        private bool _isACraftsman;
        public bool IsACraftsman
        {
            get => _isACraftsman;
            set
            {
                _isACraftsman = value;
                OnPropertyChanged();
            }
        }
        
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

        private bool showCreateNewModal;
        public bool ShowCreateNewModal
        {
            get { return showCreateNewModal; }
            set
            {
                showCreateNewModal = value;
                OnPropertyChanged();
            }
        }
        
        private bool showApplyModal;

        public bool ShowApplyModal
        {
            get { return showApplyModal; }
            set
            {
                showApplyModal = value;
                OnPropertyChanged();
            }
        }

        public AddTaskCategoryViewModel(ICallback view)
        {
            IsATab = true;
            this.view = view;
            
            OpenCreateTaskPopupCommand = new AsyncCommand<CraftLayoutModel>(OpenCreateTaskPopup);
            MoveToToSCommand = new Command(MoveToToS);
            OpenCraftsmanRegistrationPopupCommand = new AsyncCommand(OpenCraftsmanRegistrationPopup);

            CraftList = new ObservableCollection<CraftLayoutModel>(AppConstants.CraftModels);
        }


        public override async void Navigated()
        {
            base.Navigated();

            IsACraftsman = false;
            
            try
            {
                var craftsmanMdl = await dao.GetCraftsman(Settings.ActiveOffice, dao.ActiveUser.Id);

                // if no exception, then can move on
                IsACraftsman = !craftsmanMdl.Deleted;
            }
            catch (Exception ex)
            {
                IsACraftsman = false;
            }
        }

        public async void MoveToToS()
        {
            await view.MoveToToS();
        }

        public async System.Threading.Tasks.Task OpenCreateTaskPopup(CraftLayoutModel craftLayoutModel)
        {
            await view.OpenCreateTaskPopup(craftLayoutModel);
        }

        public async System.Threading.Tasks.Task OpenCraftsmanRegistrationPopup()
        {
            await view.OpenCraftsmanRegistrationPopup(IsACraftsman);
        }
    }
}
