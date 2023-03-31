using System.Collections.Generic;
using System.IO;
using Android.Graphics;
using Android.Media;
using Toolit.Droid.Services;
using Toolit.Interfaces;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ThumbnailService))]
namespace Toolit.Droid.Services
{
    public class ThumbnailService : IThumbnailService
    {
        public ImageSource GenerateVideoThumbnail(string url)
        {
            MediaMetadataRetriever retriever = new MediaMetadataRetriever();
            retriever.SetDataSource(url);
            Bitmap bitmap = retriever.GetFrameAtTime(0);
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                byte[] bitmapData = stream.ToArray();
                return ImageSource.FromStream(() => new MemoryStream(bitmapData));
            }
            return null;
        }
    }
}