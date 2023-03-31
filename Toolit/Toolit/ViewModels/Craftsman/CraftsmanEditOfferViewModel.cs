using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using Toolit.Extensions;
using Toolit.Helpers;
using Toolit.Mappers;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class CraftsmanEditOfferViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        private readonly string _taskId;
        private readonly string _bidId;
        private readonly decimal _brokerageFee;
        private BidUiModel _displayedBid;

        public BidUiModel DisplayedBid
        {
            get => _displayedBid;
            set
            {
                _displayedBid = value;
                OnPropertyChanged();
            }
        }

        public ValidatableObject<string> MaterialCost { get; } = new ValidatableObject<string>()
        {
            ValidateOnChange = true
        };
        public ValidatableObject<string> LaborCost { get; } = new ValidatableObject<string>()
        {
            ValidateOnChange = true
        };
        
        public ValidatableGroup EditBidForm { get; } = new ValidatableGroup();
        
        public bool IsNewBidCostInfoVisible => !MaterialCost.IsInvalidAndChanged &&
                                               !LaborCost.IsInvalidAndChanged &&
                                               !string.IsNullOrWhiteSpace(LaborCost.Value) &&
                                               !string.IsNullOrWhiteSpace(MaterialCost.Value);

        public ICommand BackCommand { get; }
        public ICommand SaveCommand { get; }

        public CraftsmanEditOfferViewModel(ICallback view, string taskId, string bidId, decimal brokerageFee)
        {
            this.view = view;
            _taskId = taskId;
            _bidId = bidId;
            _brokerageFee = brokerageFee;

            BackCommand = new AsyncCommand(Back);
            SaveCommand = new AsyncCommand(Save);
            
            MaterialCost.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            LaborCost.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            
            EditBidForm.Add(new IIsValid[] { LaborCost, MaterialCost });
        }
        
        
        public override async void Navigated()
        {
            base.Navigated();

            try
            {
                DisplayedBid = (await dao.GetBid(Settings.ActiveOffice, _taskId, _bidId)).ToBidUiModel();
                DisplayedBid.Task = (await dao.GetTask(Settings.ActiveOffice, _taskId)).ToTaskUiModel();
                DisplayedBid.BrokerageFee = _brokerageFee;
                
                MaterialCost.Value = DisplayedBid.MaterialCost.ToString(CultureInfo.CurrentCulture);
                LaborCost.Value = DisplayedBid.LabourCost.ToString(CultureInfo.CurrentCulture);
                
                EditBidForm.Validated += EditBidFormOnValidated;
                EditBidFormOnValidated(this, EventArgs.Empty);
                
                userDialogs.HideLoading();
            }
            catch (Exception ex)
            {
                userDialogs.HideLoading();
                HandleError(ex, ex.Message);
            }
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
            
            EditBidForm.Validated -= EditBidFormOnValidated;
        }

        private void EditBidFormOnValidated(object sender, EventArgs e)
        {
            decimal.TryParse(LaborCost.Value.Replace(',', '.'), NumberStyles.Any, 
                CultureInfo.InvariantCulture, out var laborCost);
            decimal.TryParse(MaterialCost.Value.Replace(',', '.'), NumberStyles.Any, 
                CultureInfo.InvariantCulture, out var materialCost);

            var newBidUiMdl = BidCostHelper.ConstructNewBidModel(laborCost, materialCost, DisplayedBid.Task.UseRotRut, _brokerageFee);

            DisplayedBid.LabourCost = newBidUiMdl.LabourCost;
            DisplayedBid.MaterialCost = newBidUiMdl.MaterialCost;
            DisplayedBid.RootDeduction = newBidUiMdl.RootDeduction;
            DisplayedBid.Vat = newBidUiMdl.Vat;
            DisplayedBid.FinalBid = newBidUiMdl.FinalBid;
            
            OnPropertyChanged(nameof(IsNewBidCostInfoVisible));
        }

        private async System.Threading.Tasks.Task Save()
        {
            if (MaterialCost.IsValid &&
                LaborCost.IsValid)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);

                    var apiMdl = await dao.GetBid(Settings.ActiveOffice, _taskId, _bidId);

                    decimal.TryParse(LaborCost.Value.Replace(',', '.'), NumberStyles.Any, 
                        CultureInfo.InvariantCulture, out var laborCost);
                    decimal.TryParse(MaterialCost.Value.Replace(',', '.'), NumberStyles.Any, 
                        CultureInfo.InvariantCulture, out var materialCost);

                    var newBidUiMdl = BidCostHelper.ConstructNewBidModel(laborCost, materialCost, DisplayedBid.Task.UseRotRut, _brokerageFee);

                    apiMdl.LabourCost = newBidUiMdl.LabourCost;
                    apiMdl.MaterialCost = newBidUiMdl.MaterialCost;
                    apiMdl.RootDeduction = newBidUiMdl.RootDeduction;
                    apiMdl.Vat = newBidUiMdl.Vat;
                    apiMdl.FinalBid = newBidUiMdl.FinalBid;

                    await dao.Update(apiMdl);
                    
                    userDialogs.HideLoading();
                    await Back();
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
        }
        
        public async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }
    }
}
