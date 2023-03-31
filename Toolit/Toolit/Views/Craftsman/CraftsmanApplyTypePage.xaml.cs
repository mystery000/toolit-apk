using System.Collections.Generic;
using Toolit.Models.Layouts;
using Toolit.Models.Ui;
using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanApplyTypePage : ContentPage, CraftsmanApplyTypeViewModel.ICallback
    {
        public CraftsmanApplyTypePage()
        {
            BindingContext = new CraftsmanApplyTypeViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async System.Threading.Tasks.Task MoveToApplyCompany(CraftsmanUiModel newCraftsmanModel)
        {
            await Shell.Current.Navigation.PushAsync(new CraftsmanApplyCompanyPage(newCraftsmanModel));
        }
    }
}