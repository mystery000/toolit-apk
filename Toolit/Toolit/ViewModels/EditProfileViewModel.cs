using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Toolit.Resourses;
using Toolit.Validation;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;

        public User User { get; set; }
        
        public ValidatableObject<string> FullName { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Email { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Address { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> Phone { get; } = new ValidatableObject<string>();
        
        public ICommand SaveCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public EditProfileViewModel(ICallback view)
        {
            this.view = view;

            SaveCommand = new AsyncCommand(Save);
            BackCommand = new AsyncCommand(Back);
            
            Email.Validations.Add(new IsValidEmailRule<string> { ValidationMessage = "EmailNotValid" });
            Phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "PhoneCannotBeEmpty" });
            Address.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "AddressCannotBeEmpty" });
            FullName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "NameCannotBeEmpty" });
        }

        public override void Navigated()
        {
            base.Navigated();
            
            User = dao.ActiveUser;
            
            FullName.Value = $"{User.PreferredName} {User.LastName}";
            Email.Value = User.Email;
            // trim country code
            Phone.Value = User.Phone.TrimStart('+').Substring(2);
            Address.Value = User.Address;
            
            userDialogs.HideLoading();
        }

        public override void NavigatingFrom()
        {
            base.NavigatingFrom();
        }
        
        
        private async System.Threading.Tasks.Task Save()
        {
            if (FullName.IsValid &&
                Address.IsValid &&
                Phone.IsValid)
            {
                try
                {
                    dao.ActiveUser.PreferredName = FullName.Value.Split().First();
                    dao.ActiveUser.LastName = FullName.Value.Split().LastOrDefault() ?? string.Empty;
                    dao.ActiveUser.Address = Address.Value;
                    dao.ActiveUser.Phone = $"{Phone.Value}";
                    
                    // temp
                    dao.ActiveUser.MiddleNames = !string.IsNullOrWhiteSpace(dao.ActiveUser.MiddleNames)
                        ? dao.ActiveUser.MiddleNames
                        : string.Empty;
                    dao.ActiveUser.Message = !string.IsNullOrWhiteSpace(dao.ActiveUser.Message)
                        ? dao.ActiveUser.Message
                        : string.Empty;
                    dao.ActiveUser.Description = !string.IsNullOrWhiteSpace(dao.ActiveUser.Description)
                        ? dao.ActiveUser.Description
                        : string.Empty;
                    dao.ActiveUser.Postcode = !string.IsNullOrWhiteSpace(dao.ActiveUser.Postcode)
                        ? dao.ActiveUser.Postcode
                        : string.Empty;
                    dao.ActiveUser.City = !string.IsNullOrWhiteSpace(dao.ActiveUser.City)
                        ? dao.ActiveUser.City
                        : string.Empty;
                    dao.ActiveUser.Country = !string.IsNullOrWhiteSpace(dao.ActiveUser.Country)
                        ? dao.ActiveUser.Country
                        : "SE";
                    dao.ActiveUser.Modified = DateTime.Now.Normalize();

                    await dao.Update(dao.ActiveUser);
                    userDialogs.Toast(AppResources.UserUpdateSuccessString);
                
                    await Back();
                }
                catch (Exception ex)
                {
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
