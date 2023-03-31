using Android.App;
using Android.Content;
using Android.Net;
using Toolit.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(Swish))]
namespace Toolit.Droid
{
    public class Swish : ISwish
    {
        public bool StartSwish(string autostartToken)
        {
            var intent = Application.Context.PackageManager.GetLaunchIntentForPackage("se.bankgirot.swish");
            if (intent != null)
            {
                // TODO callback is null?
                intent.SetAction(Intent.ActionView);
                intent.SetData(Uri.Parse("swish://paymentrequest?token=" + autostartToken + "&callbackurl=null"));
                Application.Context.StartActivity(intent);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}