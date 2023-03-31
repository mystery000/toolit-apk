using Toolit.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Toolit.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CraftsmanEditMessagePage : ContentPage, CraftsmanEditMessageViewModel.ICallback
    {
        public CraftsmanEditMessagePage(string taskId, string bidId)
        {
            BindingContext = new CraftsmanEditMessageViewModel(this, taskId, bidId);
            InitializeComponent();
        }

        public async System.Threading.Tasks.Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}