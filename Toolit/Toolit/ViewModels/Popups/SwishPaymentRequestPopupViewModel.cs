using System;
using System.Windows.Input;
using Microsoft.AppCenter.Crashes;
using Toolit.Helpers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels.Popups
{
    public class SwishPaymentRequestPopupViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task MoveToConfirmation();
            System.Threading.Tasks.Task Close();
        }

        private readonly ICallback view;
        private readonly BidUiModel _bid;
        private readonly ISwish _swish;

        public ICommand PayCommand { get; private set; }
        public ICommand MoveToToSCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        
        public SwishPaymentRequestPopupViewModel(ICallback view, BidUiModel bid)
        {
            this.view = view;
            _bid = bid;

            _swish = DependencyService.Get<ISwish>();

            PayCommand = new AsyncCommand(Pay);
            MoveToToSCommand = new AsyncCommand(MoveToToS);
            CloseCommand = new AsyncCommand(Close);
        }

        private async System.Threading.Tasks.Task Pay()
        {
            try
            {
                userDialogs.ShowLoading(AppResources.LoadingString);
                
                (string PaymentRequestToken, string PaymentId) paymentData = 
                    await dao.AcceptBid(_bid.OfficeId, _bid.TaskId, _bid.Id);
                
                if (_swish.StartSwish(paymentData.PaymentRequestToken))
                {
                    var result =
                        await dao.WaitForPaymentChange(_bid.OfficeId, _bid.TaskId, _bid.Id, paymentData.PaymentId);
                    if (result == PaymentState.PaidToEscrow)
                    {
                        userDialogs.HideLoading();
                        await view.MoveToConfirmation();
                    }
                    else
                    {
                        userDialogs.HideLoading();
                        Crashes.TrackError(new IllegalStateException("State is not PaidToEscrow"));
                        userDialogs.Toast(AppResources.SwishPaymentFailedErrorString);
                    }
                }
                else
                {
                    userDialogs.HideLoading();
                    userDialogs.Toast(AppResources.SwishNotInstalledErrorString);
                }
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                userDialogs.Toast(AppResources.SwishPaymentFailedErrorString);
                Crashes.TrackError(ex);
            }
        }

        private async System.Threading.Tasks.Task MoveToToS()
        {
            await EssentialsHelper.TryOpenWebBrowser(AppResources.ToolitTosUrl);
        }
        
        private async System.Threading.Tasks.Task Close()
        {
            await view.Close();
        }
    }
}