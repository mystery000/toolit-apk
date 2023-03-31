using System;
using System.Collections.ObjectModel;
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
    public class CraftsmanEditBioViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }


        private readonly ICallback view;
        
        private readonly string _officeId;
        private readonly string _craftsmanId;
        
        private CraftsmanUiModel _craftsman;

        public CraftsmanUiModel Craftsman
        {
            get => _craftsman;
            set
            {
                _craftsman = value;
                OnPropertyChanged();
            }
        }

        public ValidatableObject<string> AboutText { get; } = new ValidatableObject<string>();
        public ValidatableObject<string> AboutHeader { get; } = new ValidatableObject<string>();
        
        public ICommand SaveChangesCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public CraftsmanEditBioViewModel(ICallback view, string officeId, string craftsmanId)
        {
            this.view = view;

            _officeId = officeId;
            _craftsmanId = craftsmanId;
            
            SaveChangesCommand = new AsyncCommand(SaveChanges);
            BackCommand = new AsyncCommand(Back);

            AboutHeader.Validations.Add(new IsNotNullOrEmptyRule<string>
                {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
            AboutText.Validations.Add(new IsNotNullOrEmptyRule<string>
                {ValidationMessage = AppResources.PleaseFillAllFieldsErrorString});
        }

        public override async void Navigated()
        {
            base.Navigated();

            if (!string.IsNullOrWhiteSpace(_officeId) && !string.IsNullOrWhiteSpace(_craftsmanId))
            {
                try
                {
                    Craftsman = (await dao.GetCraftsman(_officeId, _craftsmanId)).ToCraftsmanUiModel();
            
                    AboutHeader.Value = Craftsman.AboutHeader;
                    AboutText.Value = Craftsman.AboutText;
                    
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
            if (AboutHeader.IsValid &&
                AboutText.IsValid)
            {
                try
                {
                    userDialogs.ShowLoading(AppResources.LoadingString);
                    var apiMdl = await dao.GetCraftsman(Craftsman.OfficeId, Craftsman.Id);

                    apiMdl.AboutHeader = AboutHeader.Value;
                    apiMdl.AboutText = AboutText.Value;
                    
                    await dao.Update(apiMdl);

                    userDialogs.Toast(AppResources.UserUpdateSuccessString);
                    userDialogs.HideLoading();
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