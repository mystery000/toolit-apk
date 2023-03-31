using System;
using System.Windows.Input;
using Toolit.Models.Layouts;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Toolit.ViewModels.Popups
{
    public class DeleteAccountModalViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
            System.Threading.Tasks.Task ResetToWelcome();
        }

        private readonly ICallback view;

        public ICommand CloseCommand { get; }
        public ICommand ConfirmDeleteAccountCommand { get; }

        public DeleteAccountModalViewModel(ICallback view)
        {
            this.view = view;

            CloseCommand = new AsyncCommand(Close);
            ConfirmDeleteAccountCommand = new AsyncCommand(ConfirmDeleteAccount);
        }
        
        public async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
        
        private async System.Threading.Tasks.Task ConfirmDeleteAccount()
        {
            try
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                
                await dao.Delete(dao.ActiveUser);
                
                await Settings.SetSessionTimeoutTime(DateTime.MinValue);

                userDialogs.HideLoading();
                userDialogs.Toast(AppResources.AccountDeletedConfirmationMessage);
                await view.ResetToWelcome();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.ToString());
            }
        }
    }
}