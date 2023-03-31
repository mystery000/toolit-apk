using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanEditOfferPage : ContentPage, CraftsmanEditOfferViewModel.ICallback
    {
        public CraftsmanEditOfferPage(string taskId, string bidId, decimal brokerageFee)
        {
            BindingContext = new CraftsmanEditOfferViewModel(this, taskId, bidId, brokerageFee);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}