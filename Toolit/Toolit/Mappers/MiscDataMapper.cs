using Toolit.Models.Ui;
using Xamarin.Forms;

namespace Toolit.Mappers
{
    public static class MiscDataMapper
    {
        public static UserUiModel ToUserUiModel(this User mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new UserUiModel()
            {
                Id = mdl.Id,
                
                FirstName = mdl.FirstName,
                PreferredName = mdl.PreferredName,
                LastName = mdl.LastName,
                
                Email = mdl.Email,
                Phone = mdl.Phone,
                Address = mdl.Address,
                ImageUrl = (mdl.ImageSource as UriImageSource)?.Uri.ToString()
            };
        }

        public static PaymentUiModel ToPaymentUiModel(this Payment mdl)
        {
            if (ReferenceEquals(null, mdl))
            {
                return null;
            }

            return new PaymentUiModel()
            {
                Id = mdl.Id,
                CraftsmanId = mdl.CraftsmanId,
                OfficeId = mdl.OfficeId,
                TaskId = mdl.TaskId,
                SwishPaymentId = mdl.SwishPaymentId,
                BidId = mdl.BidId,
                
                Amount = mdl.Amount,
                PaymentMethod = mdl.PaymentMethod,
                Modified = mdl.Modified.ToLocalTime()
            };
        }
    }
}