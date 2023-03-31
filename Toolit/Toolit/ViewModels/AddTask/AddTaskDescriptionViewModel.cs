using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Models.Layouts;
using Toolit.Models.Misc;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AddTaskDescriptionViewModel : BaseViewModel
    {

        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task Skip();
            System.Threading.Tasks.Task MoveToLocation(CraftLayoutModel craftMdl, 
                IList<MediaFile> media, Task newTask);
        }

        private readonly ICallback view;
        
        public CraftLayoutModel CraftMdl { get; }
        public IList<MediaFile> Media { get; }
        
        public ValidatableGroup Form { get; } = new ValidatableGroup();
        public ValidatableObject<string> Title { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Description { get; } = new ValidatableObject<string>();

        public ICommand MoveToLocationCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public ICommand SkipCommand { get; private set; }

        public AddTaskDescriptionViewModel(ICallback view, CraftLayoutModel craftMdl, IList<MediaFile> media)
        {
            this.view = view;
            
            CraftMdl = craftMdl;
            Media = media;

            BackCommand = new Command(Back);
            SkipCommand = new Command(Skip);
            MoveToLocationCommand = new Command(MoveToLocation);
            
            Title.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            Description.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            Form.Add(new IIsValid[] { Title, Description });
        }

        public async void Back()
        {
            await view.Back();
        }

        public async void Skip()
        {
            await view.Skip();
        }

        public async void MoveToLocation()
        {
            if (Form.IsValid)
            {
                var newTask = new Task()
                {
                    Title = Title.Value,
                    Description = Description.Value
                };
                await view.MoveToLocation(CraftMdl, Media, newTask);
            }
            else
            {
                userDialogs.Toast(AppResources.PleaseFillAllFieldsErrorString);
            }
        }
    }
}