using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Bid : Base<Bid>, IUpdate<Bid>, IRollback, IEquatable<Base<Bid>>
    {
        public string OfficeId { get; set; }

        public string CraftsmanId { get; set; }
                
        public string TaskId { get; set; }
        
        public string BidMessage { get; set; }

        public decimal LabourCost { get; set; }
        
        public decimal MaterialCost { get; set; }

        public decimal RootDeduction { get; set; }

        public decimal Vat { get; set; }

        public decimal FinalBid { get; set; }

        public bool IsCancelled { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Deleted { get; set; }


        public Bid() { }

        public Bid(string json) : base(json) { }

        protected override void UpdateFields(Bid that)
        {
            this.CraftsmanId = that.CraftsmanId;
            this.TaskId = that.TaskId;
            this.LabourCost = that.LabourCost;
            this.MaterialCost = that.MaterialCost;
            this.FinalBid = that.FinalBid;
            this.Vat = that.Vat;
            this.BidMessage = that.BidMessage;
            this.IsCancelled = that.IsCancelled;
            this.OfficeId = that.OfficeId;
            this.Deleted = that.Deleted;
        }
    }
}