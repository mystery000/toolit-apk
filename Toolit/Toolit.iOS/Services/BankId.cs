using Foundation;
using UIKit;
using Toolit.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(BankId))]
namespace Toolit.iOS
{
    public class BankId : IBankId
    {
        public bool StartBankIdWithoutPNum(string autostartToken)
        {
            try
            {
                // TODO
                var request = new NSUrl("bankid:///?autostarttoken=" + autostartToken + "&redirect=toolit%3A%2F%2F%2F%3F");
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