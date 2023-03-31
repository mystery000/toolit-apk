using Android.App;
using Android.Content;
using Android.Net;
using Toolit.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(BankIdService))]
namespace Toolit.Droid
{
    public class BankIdService : Activity, IBankId
    {
        public bool StartBankIdWithoutPNum(string autostartToken)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.SetData(Uri.Parse("https://app.bankid.com/?autostarttoken=" + autostartToken + "&redirect=null"));
            intent.SetFlags(ActivityFlags.NewTask);

            if (intent.ResolveActivity(Application.Context.PackageManager) != null)
            {
                Application.Context.StartActivity(intent);
                return true;
            }
            else
            {
                //Page not found
                return false;
            }
        }
    }
}