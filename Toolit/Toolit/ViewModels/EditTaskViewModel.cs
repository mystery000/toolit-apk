using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Layouts;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels
{
    public class EditTaskViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task DeleteTask(string taskId);
        }
        
        private readonly ICallback view;

        private readonly string _taskId;
        
        private TaskUiModel _displayedTask;
        private CraftLayoutModel _craftModel;

        public TaskUiModel DisplayedTask
        {
            get => _displayedTask;
            set
            {
                _displayedTask = value;
                OnPropertyChanged();
            }
        }

        public ValidatableGroup Form { get; } = new ValidatableGroup();
        public ValidatableObject<string> Title { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Description { get; } = new ValidatableObject<string>();
        
        public ICommand BackCommand { get; private set; }
        public ICommand ToggleTagSelectionCommand { get; set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand DeleteTaskCommand { get; private set; }
        
        public EditTaskViewModel(ICallback view, string taskId)
        {
            this.view = view;
            _taskId = taskId;
            
            BackCommand = new AsyncCommand(Back);
            ToggleTagSelectionCommand = new AsyncCommand<TagLayoutModel>(ToggleTagSelection);
            SaveCommand = new AsyncCommand(Save);
            DeleteTaskCommand = new AsyncCommand(DeleteTask);
            
            Title.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            Description.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            Form.Add(new IIsValid[] { Title, Description });
        }

        public override async void Navigated()
        {
            base.Navigated();
            
            try
            {
                DisplayedTask = (await dao.GetTask(Settings.ActiveOffice, _taskId)).ToTaskUiModel();
                
                Title.Value = DisplayedTask.Title;
                Description.Value = DisplayedTask.Description;
                
                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }
        
        private async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }
        
        public async System.Threading.Tasks.Task ToggleTagSelection(TagLayoutModel mdl)
        {
            mdl.IsSelected = !mdl.IsSelected;
        }
        
        private async System.Threading.Tasks.Task Save()
        {
            try
            {
                if (Form.IsValid)
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);
                    
                    var taskApiMdl = await dao.GetTask(Settings.ActiveOffice, _taskId);

                    taskApiMdl.Title = Title.Value;
                    taskApiMdl.Description = Description.Value;

                    await dao.Update(taskApiMdl);
                
                    userDialogs.HideLoading();
                    await view.Back();
                }
                else
                {
                    userDialogs.Toast(AppResources.PleaseFillAllFieldsErrorString);
                }
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }
        
        private async System.Threading.Tasks.Task DeleteTask()
        {
            await view.DeleteTask(_taskId);
        }
    }
}