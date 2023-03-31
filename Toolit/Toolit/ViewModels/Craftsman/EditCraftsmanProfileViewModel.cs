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
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class EditCraftsmanProfileViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        
        private readonly string _officeId;
        private readonly string _craftsmanId;
        
        private UserUiModel _user;
        private CraftsmanUiModel _craftsman;
        private ObservableCollection<CraftLayoutModel> _craftList;

        public UserUiModel User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
                OnPropertyChanged();
            }
        }

        public ValidatableObject<string> PreferredName { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> LastName { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Email { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Address { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Phone { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> CompanyName { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> OrgNumber { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> CompanyAddress { get; } = new ValidatableObject<string>();

        public ObservableCollection<CraftLayoutModel> CraftList
        {
            get => _craftList;
            set
            {
                _craftList = value;
                OnPropertyChanged();
            }
        }
        public ICommand SaveChangesCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public EditCraftsmanProfileViewModel(ICallback view, string officeId, string craftsmanId)
        {
            this.view = view;
            
            _officeId = officeId;
            _craftsmanId = craftsmanId;
            
            SaveChangesCommand = new AsyncCommand(SaveChanges);
            BackCommand = new AsyncCommand(Back);
            
            Email.Validations.AddRange(new IValidationRule<string>[]{
                new IsNotNullOrEmptyRule<string>
                    {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString},
                new IsValidEmailRule<string> 
                    { ValidationMessage = AppResources.EmailNotValid }
            });
            Phone.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            LastName.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            Address.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage =AppResources.PleaseFillAllFieldsErrorString });
            PreferredName.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            CompanyName.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            OrgNumber.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });
            CompanyAddress.Validations.Add(new IsNotNullOrEmptyRule<string> 
                { ValidationMessage = AppResources.PleaseFillAllFieldsErrorString });

            CraftList = new ObservableCollection<CraftLayoutModel>(AppConstants.CraftModels);
        }


        public override async void Navigated()
        {
            base.Navigated();

            if (!string.IsNullOrWhiteSpace(_officeId) && !string.IsNullOrWhiteSpace(_craftsmanId))
            {
                try
                {
                    Craftsman = (await dao.GetCraftsman(_officeId, _craftsmanId)).ToCraftsmanUiModel();
                    User = (await dao.GetUser(Craftsman.UserId)).ToUserUiModel();
            
                    PreferredName.Value = User.PreferredName;
                    LastName.Value = User.LastName;
                    Email.Value = User.Email;
                    // trim country code
                    Phone.Value = User.Phone.TrimStart('+').Substring(2);
                    Address.Value = User.Address;
                    CompanyName.Value = Craftsman.CompanyName;
                    OrgNumber.Value = Craftsman.OrgNumber;
                    CompanyAddress.Value = Craftsman.CompanyAddress;

                    foreach (var craft in CraftList)
                    {
                        if (Craftsman.Crafts.Any(crft => crft.CraftType.Equals(craft.ServerId, StringComparison.InvariantCulture)))
                        {
                            craft.IsSelected = true;
                        }
                    }
                    userDialogs.HideLoading();
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
        }

        private async System.Threading.Tasks.Task SaveChanges()
        {
            if (PreferredName.IsValid &&
                LastName.IsValid &&
                Email.IsValid &&
                Phone.IsValid &&
                Address.IsValid &&
                CompanyName.IsValid &&
                OrgNumber.IsValid &&
                CompanyAddress.IsValid)
            {
                try
                {                    
                    userDialogs.ShowLoading(AppResources.LoadingString);
                    
                    var userApiMdl = dao.ActiveUser;
                    var craftsmanApiMdl = await dao.GetCraftsman(Craftsman.OfficeId, Craftsman.Id);

                    userApiMdl.PreferredName = PreferredName.Value;
                    userApiMdl.LastName = LastName.Value;
                    userApiMdl.Phone = $"+46{Phone.Value}";
                    userApiMdl.Address = Address.Value;
                    
                    craftsmanApiMdl.CompanyName = CompanyName.Value;
                    craftsmanApiMdl.OrgNumber = OrgNumber.Value;
                    craftsmanApiMdl.CompanyAddress = CompanyAddress.Value;
                    
                    await dao.Update(userApiMdl);
                    await dao.Update(craftsmanApiMdl);

                    userDialogs.HideLoading();
                    userDialogs.Toast(AppResources.UserUpdateSuccessString);
                    await Back();
                }
                catch (Exception ex)
                {
                    userDialogs.HideLoading();
                    HandleError(ex, ex.Message);
                }
            }
            else
            {
                userDialogs.Toast(AppResources.PleaseFillAllFieldsErrorString);
            }
        }
        
        public async System.Threading.Tasks.Task Back()
        {
            await view.Back();
        }
    }
}
