using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Toolit.ViewModels.Popups
{
    public class FullImagePopupViewModel : BaseViewModel
    {
        private readonly ICallback _view;

        public interface ICallback
        {
            System.Threading.Tasks.Task Close();
        }
        
        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public ICommand CloseCommand { get; private set; }

        public FullImagePopupViewModel(ICallback view, ImageSource source) : base()
        {
            _view = view;
            Image = source;

            CloseCommand = new AsyncCommand(Close);
        }

        private async System.Threading.Tasks.Task Close()
        {
            await _view.Close();
        }
    }
}