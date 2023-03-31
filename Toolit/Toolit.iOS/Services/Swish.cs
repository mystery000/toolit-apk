using Foundation;
using UIKit;
using Toolit.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(Swish))]
namespace Toolit.iOS
{
    public class Swish : ISwish
    {
        public bool StartSwish(string autostartToken)
        {
            try
            {
                // TODO
                var request = new NSUrl("swish://paymentrequest?token=" + autostartToken + "&callbackurl=toolit%3A%2F%2F");
                if (UIApplication.SharedApplication.CanOpenUrl(request))
                {
                    return UIApplication.SharedApplication.OpenUrl(request);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}