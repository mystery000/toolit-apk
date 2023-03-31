using System;
using System.Windows.Input;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class DeleteTaskPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {            
            System.Threading.Tasks.Task FullyClose();
            System.Threading.Tasks.Task Close();
        }

        private readonly ICallback view;
        private readonly string _taskId;

        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        
        public DeleteTaskPopupViewModel(ICallback view, string taskId)
        {
            this.view = view;
            _taskId = taskId;

            DeleteCommand = new AsyncCommand(Delete);
            CloseCommand = new AsyncCommand(Close);
        }

        private async System.Threading.Tasks.Task Delete()
        {
            try
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                var taskMdl = await dao.GetTask(Settings.ActiveOffice, _taskId);
                await dao.Delete(taskMdl);

                userDialogs.HideLoading();
                await view.FullyClose();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }

        private async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
    }
}