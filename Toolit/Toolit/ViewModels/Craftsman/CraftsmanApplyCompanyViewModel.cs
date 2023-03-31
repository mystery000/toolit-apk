using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Models.Ui;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class CraftsmanApplyCompanyViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
            System.Threading.Tasks.Task MoveToApplyInfo(CraftsmanUiModel newCraftsmanModel);
        }
        
        private readonly ICallback view;
        private readonly CraftsmanUiModel _newCraftsmanModel;
        
        private bool _hasFTax;

        public ValidatableGroup Form { get; } = new ValidatableGroup();
        public ValidatableObject<string> CompanyName { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> OrgNumber { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> CompanyAddress { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> AccountNumber { get; } = new ValidatableObject<string>();

        public bool HasFTax
        {
            get => _hasFTax;
            set
            {
                _hasFTax = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleFTaxYesBtnCommand { get; set; }
        public ICommand ToggleFTaxNoBtnCommand { get; set; }
        public ICommand MoveToApplyInfoCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public CraftsmanApplyCompanyViewModel(ICallback view, CraftsmanUiModel newCraftsmanModel)
        {
            this.view = view;
            _newCraftsmanModel = newCraftsmanModel;
            
            BackCommand = new Command(Back);
            ToggleFTaxYesBtnCommand = new Command(ToggleFTaxYesBtn);
            ToggleFTaxNoBtnCommand = new Command(ToggleFTaxNoBtn);
            MoveToApplyInfoCommand = new Command(MoveToApplyInfo);
            CompanyName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            OrgNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            ////OrgNumber.Validations.Add(new IsLengthValidRule<string> { ValidationMessage = "OrgNumberToShort", MinimumLength = 10 });
            //OrgNumber.Validations.Add(new IsLengthValidRule<string> { ValidationMessage = "Org Number To Long", MaximumLength = 10 });
            CompanyAddress.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            AccountNumber.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            
            Form.Add(new IIsValid[] { OrgNumber, CompanyAddress, CompanyName, AccountNumber });
        }

        public async void Back()
        {
            await view.Back();
        }

        public async void MoveToApplyInfo()
        {
            if (Form.IsValid)
            {
                _newCraftsmanModel.CompanyName = CompanyName.Value;
                _newCraftsmanModel.OrgNumber = OrgNumber.Value;
                _newCraftsmanModel.CompanyAddress = CompanyAddress.Value;
                _newCraftsmanModel.AccountNumber = AccountNumber.Value;
                _newCraftsmanModel.FTax = HasFTax;
                
                await view.MoveToApplyInfo(_newCraftsmanModel);
            }
        }

        public void ToggleFTaxYesBtn()
        {
            HasFTax = true;
        }
        
        public void ToggleFTaxNoBtn()
        {
            HasFTax = false;
        }
    }
}