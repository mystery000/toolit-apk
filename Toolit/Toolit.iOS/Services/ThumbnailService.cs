using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using Toolit.Interfaces;
using Toolit.iOS.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(ThumbnailService))]

namespace Toolit.iOS.Services
{
    public class ThumbnailService : IThumbnailService
    {
        public ImageSource GenerateVideoThumbnail(string url)
        {
            AVAssetImageGenerator imageGenerator =
                new AVAssetImageGenerator(AVAsset.FromUrl(NSUrl.FromFilename(url)));
            
            imageGenerator.AppliesPreferredTrackTransform = true;
            CMTime actualTime;
            NSError error;
            CGImage cgImage = imageGenerator.CopyCGImageAtTime(new CMTime(0, 1000000), out actualTime, out error);
            
            return ImageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream());
        }
    }
}