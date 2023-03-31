using Xamarin.CommunityToolkit.Core;
using Xamarin.Forms;

namespace Toolit.Models.Misc
{
    public class MediaFile
    {
        public StreamImageSource Source { get; set; }
        public MediaSource MediaSource { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public bool IsVideo { get; set; }
    }
}