using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataPolicyPage : ContentPage, DataPolicyViewModel.ICallback
    {
        public DataPolicyPage()
        {
            BindingContext = new DataPolicyViewModel(this);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}