using System.Windows.Input;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels.Popups
{
    public class FullVideoPopupViewModel : BaseViewModel
    {
        private readonly ICallback _view;

        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
        }
        
        private MediaSource _videoSource;

        public MediaSource VideoSource
        {
            get => _videoSource;
            set
            {
                _videoSource = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; private set; }

        public FullVideoPopupViewModel(ICallback view, MediaSource source) : base()
        {
            _view = view;
            VideoSource = source;

            CloseCommand = new AsyncCommand(Close);
        }

        private async System.Threading.Tasks.Task Close()
        {
            await _view.Close();
        }
    }
}