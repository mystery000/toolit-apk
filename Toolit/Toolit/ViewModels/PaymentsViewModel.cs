
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class PaymentsViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToMyTask(string taskId);
            System.Threading.Tasks.Task MoveToOtherTask(string taskId);
        }

        private readonly ICallback view;

        private ObservableCollection<PaymentUiModel> payments;
        public ObservableCollection<PaymentUiModel> Payments
        {
            get { return payments; }
            set
            {
                payments = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; private set; }
        public ICommand MoveToPaymentTaskCommand { get; private set; }

        public PaymentsViewModel(ICallback view)
        {
            this.view = view;
            
            BackCommand = new Command(Back);
            MoveToPaymentTaskCommand = new AsyncCommand<PaymentUiModel>(MoveToPaymentTask);
        }

        
        public override void Navigated()
        {
            base.Navigated();
            
            if (!IsSubscribedToDao)
            {
                dao.Subscribe(HandlePaymentSuccess, HandleError);
                IsSubscribedToDao = true;
            }
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
            
            if (IsSubscribedToDao)
            {
                dao.Unsubscribe(HandlePaymentSuccess, HandleError);
                IsSubscribedToDao = false;
            }
        }
        

        private async void HandlePaymentSuccess(Payment[] data, string nonce, DateTime updated)
        {
            try
            {
                Payments = new ObservableCollection<PaymentUiModel>((await System.Threading.Tasks.Task.WhenAll(data.Select(async pmnt =>
                {
                    var uiMdl = pmnt.ToPaymentUiModel();

                    try
                    {
                        uiMdl.Task = (await dao.GetTask(pmnt.OfficeId, pmnt.TaskId)).ToTaskUiModel();
                        uiMdl.Bid = (await dao.GetBid(pmnt.OfficeId, pmnt.TaskId, pmnt.BidId)).ToBidUiModel();
                        uiMdl.Craftsman = (await dao.GetCraftsman(pmnt.OfficeId, pmnt.CraftsmanId)).ToCraftsmanUiModel();
                        uiMdl.Craftsman.User = (await dao.GetUser(uiMdl.Craftsman.UserId)).ToUserUiModel();
                    }
                    catch (Exception ex)
                    {
                        HandleError(ex, ex.Message);
                    }
                    
                    return uiMdl;
                }))).OrderByDescending(pmnt => pmnt.Modified));
                
                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }
        
        public async void Back()
        {
            await view.Back();
        }
        private async System.Threading.Tasks.Task MoveToPaymentTask(PaymentUiModel mdl)
        {
            if (mdl?.Task != null)
            {
                if (mdl.Task.UserId.Equals(dao.ActiveUser?.Id))
                {
                    await view.MoveToMyTask(mdl.Task.Id);
                }
                else
                {
                    await view.MoveToOtherTask(mdl.Task.Id);
                }
            }
        }
        
    }
}

