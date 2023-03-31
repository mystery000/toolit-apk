using System;
using System.Runtime.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Rating : Base<Rating>, IUpdate<Rating>, IRollback, IEquatable<Base<Rating>>
    {
        public string OfficeId { get; set; }

        public string CraftsmanId { get; set; }

        public string TaskId { get; set; }
        
        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        public string Header { get; set; }
        
        public string Text { get; set; }
        
        public string Created { get; set; }

        public int Amount { get; set; }

        public Rating() { }

        public Rating(string json) : base(json) { }

        protected override void UpdateFields(Rating that)
        {
            this.OfficeId = that.OfficeId;
            this.CraftsmanId = that.CraftsmanId;
            this.TaskId = that.TaskId;
            this.Text = that.Text;
            this.Header = that.Header;
            this.Amount = that.Amount;
            this.Created = that.Created;
        }
    }
}