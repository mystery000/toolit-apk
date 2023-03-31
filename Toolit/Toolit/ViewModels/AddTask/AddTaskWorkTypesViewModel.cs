using System.Collections.Generic;
using System.Windows.Input;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AddTaskWorkTypesViewModel : BaseViewModel
    {

        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task Skip();
            System.Threading.Tasks.Task MoveToDescription(CraftLayoutModel craftMdl, IList<MediaFile> media);
        }

        private readonly ICallback view;
        
        public CraftLayoutModel CraftMdl { get; }
        public IList<MediaFile> Media { get; }

        public ICommand MoveToDescriptionCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand ToggleTagSelectionCommand { get; set; }


        public AddTaskWorkTypesViewModel(ICallback view, CraftLayoutModel craftMdl, IList<MediaFile> media)
        {
            CraftMdl = craftMdl;
            Media = media;
            
            this.view = view;

            BackCommand = new AsyncCommand(Back);
            MoveToDescriptionCommand = new AsyncCommand(MoveToDescription);
            ToggleTagSelectionCommand = new AsyncCommand<TagLayoutModel>(ToggleTagSelection);
        }

        public async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }

        public async System.Threading.Tasks.Task MoveToDescription()
        {
            await view.MoveToDescription(CraftMdl, Media);
        }

        public async System.Threading.Tasks.Task ToggleTagSelection(TagLayoutModel mdl)
        {
            mdl.IsSelected = !mdl.IsSelected;
        }
    }
}
