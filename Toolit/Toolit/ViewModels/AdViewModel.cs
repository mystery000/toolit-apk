using System.Collections.ObjectModel;
using System.Windows.Input;
using Toolit.Helpers;
using Toolit.Models.Ui;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Toolit.ViewModels
{
    public class AdViewModel : BaseViewModel
    {
        public interface ICallback
        {
            System.Threading.Tasks.Task Back();
        }

        private readonly ICallback view;
        private AdUiModel _displayedAd;

        public AdUiModel DisplayedAd
        {
            get => _displayedAd;
            set
            {
                _displayedAd = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToUrlCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public AdViewModel(ICallback view, AdUiModel ad)
        {
            this.view = view;
            DisplayedAd = ad;
            
            GoToUrlCommand = new AsyncCommand(GoToUrl);
            BackCommand = new Command(Back);
        }

        private async System.Threading.Tasks.Task GoToUrl()
        {
            await EssentialsHelper.TryOpenWebBrowser(DisplayedAd.Url);
        }

        public async void Back()
        {
            await view.Back();
        }
    }
}
