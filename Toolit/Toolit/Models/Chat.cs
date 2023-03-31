using System;
using System.Runtime.Serialization;

namespace Toolit
{
    public class Chat : Base<Chat>, IUpdate<Chat>, IRollback, IEquatable<Base<Chat>>
    {
        public string OfficeId { get; set; }

        public string TaskId { get; set; }

        public string BidId { get; set; }
        
        [DataMember(Name = "modified")]
        public DateTime Modified { get; set; }
        
        public Chat() { }

        public Chat(string json) : base(json) { }

        protected override void UpdateFields(Chat that)
        {
            this.OfficeId = that.OfficeId;
            this.BidId = that.BidId;
            this.TaskId = that.TaskId;
        }
    }
}