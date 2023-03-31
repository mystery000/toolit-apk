using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class CraftsmanEditMessageViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        private readonly string _taskId;
        private readonly string _bidId;
        
        public BidUiModel DisplayedBid { get; set; }
        public ValidatableObject<string> BidMessage { get; } = new ValidatableObject<string>();

        public ICommand SaveCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public CraftsmanEditMessageViewModel(ICallback view, string taskId, string bidId)
        {
            this.view = view;
            _taskId = taskId;
            _bidId = bidId;

            BackCommand = new Command(Back);
            SaveCommand = new AsyncCommand(Save);

            BidMessage.Validations.Add(new IsNotNullOrEmptyRule<string>
                {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
        }

        public override async void Navigated()
        {
            base.Navigated();

            try
            {
                DisplayedBid = (await dao.GetBid(Settings.ActiveOffice, _taskId, _bidId)).ToBidUiModel();
                
                BidMessage.Value = DisplayedBid.BidMessage;
                
                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }

        private async System.Threading.Tasks.Task Save()
        {
            if (BidMessage.IsValid)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    var apiMdl = await dao.GetBid(Settings.ActiveOffice, _taskId, _bidId);
                    
                    apiMdl.BidMessage = BidMessage.Value;

                    await dao.Update(apiMdl);
                    
                    userDialogs.HideLoading();
                    Back();
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
        }

        public async void Back()
        {
            await view.Back();
        }
    }
}
