using System.Collections.Generic;
using Toolit.Models.Layouts;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanApplyCompanyPage : ContentPage, CraftsmanApplyCompanyViewModel.ICallback
    {
        public CraftsmanApplyCompanyPage(CraftsmanUiModel newCraftsmanModel)
        {
            BindingContext = new CraftsmanApplyCompanyViewModel(this, newCraftsmanModel);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task MoveToApplyInfo(CraftsmanUiModel newCraftsmanModel)
        {
            await Shell.Current.Navigation.PushAsync(new CraftsmanApplyInfoPage(newCraftsmanModel));
        }
    }
}