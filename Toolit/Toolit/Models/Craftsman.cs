using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Craftsman : Base<Craftsman>, IUpdate<Craftsman>, IRollback, IEquatable<Base<Craftsman>>
    {
        public string OfficeId { get; set; }

        public Craft[] Crafts { get; set; }
        
        public string AboutText { get; set; }
        
        public string AboutHeader { get; set; }
        
        public string CompanyName { get; set; }
        
        public string OrgNumber { get; set; }
        
        public string CompanyAddress { get; set; }
        
        public string WorkArea { get; set; }
        
        public int CompletedJobs { get; set; }
        
        public string MemberSince { get; set; }

        public string CraftsmanName { get; set; }

        public string UserId { get; set; }

        public string AccountNumber { get; set; }

        public Rating[] Ratings { get; set; }

        public bool FTax { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Deleted { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Frozen { get; set; }

        public Craftsman() { }

        public Craftsman(string json) : base(json) { }

        protected override void UpdateFields(Craftsman that)
        {
            this.Crafts = that.Crafts;
            this.AboutText = that.AboutText;
            this.AboutHeader = that.AboutHeader;
            this.CompanyName = that.CompanyName;
            this.OrgNumber = that.OrgNumber;
            this.CompanyAddress = that.CompanyAddress;
            this.CompletedJobs = that.CompletedJobs;
            this.WorkArea = that.WorkArea;
            this.MemberSince = that.MemberSince;
            this.CraftsmanName = that.CraftsmanName;
            this.UserId = that.UserId;
            this.AccountNumber = that.AccountNumber;
            this.Ratings = that.Ratings;
            this.OfficeId = that.OfficeId;
            this.FTax = that.FTax;
            this.Deleted = that.Deleted;
            this.Frozen = that.Frozen;
        }
    }
}