using System;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Payment : Base<Payment>, IUpdate<Payment>, IRollback, IEquatable<Base<Payment>>
    {
        public string OfficeId { get; set; }

        public string TaskId { get; set; }

        public string BidId { get; set; }

        public string CraftsmanId { get; set; }

        public string SwishPaymentId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public PaymentState PaymentState { get; set; }

        public string PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string currency { get; set; }

        public Payment() { }

        public Payment(string json) : base(json) { }

        protected override void UpdateFields(Payment that)
        {
            this.OfficeId = that.OfficeId;

            this.TaskId = that.TaskId;

            this.BidId = that.BidId;

            this.CraftsmanId = that.CraftsmanId;

            this.SwishPaymentId = that.SwishPaymentId;

            this.PaymentDate = that.PaymentDate;

            this.PaymentState = that.PaymentState;

            this.PaymentMethod = that.PaymentMethod;

            this.Amount = that.Amount;

            this.currency = that.currency;
        }
    }
}