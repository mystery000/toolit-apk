
using Xamarin.Forms;

namespace Toolit.Interfaces
{
    public interface IThumbnailService
    {
        ImageSource GenerateVideoThumbnail(string url);
    }
}