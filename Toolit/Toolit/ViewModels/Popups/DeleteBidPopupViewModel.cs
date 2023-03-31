using System;
using System.Windows.Input;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels.Popups
{
    public class DeleteBidPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
        }

        private readonly ICallback view;
        private readonly string _taskId;
        private readonly string _bidId;

        public ICommand DeleteCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        
        public DeleteBidPopupViewModel(ICallback view, string taskId, string bidId)
        {
            this.view = view;
            _taskId = taskId;
            _bidId = bidId;

            DeleteCommand = new AsyncCommand(Delete);
            CloseCommand = new AsyncCommand(Close);
        }

        private async System.Threading.Tasks.Task Delete()
        {
            try
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                var bidMdl = await dao.GetBid(Settings.ActiveOffice, _taskId, _bidId);
                await dao.Delete(bidMdl);

                userDialogs.HideLoading();
                await view.Close();
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