using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    public class Message : Base<Message>, IUpdate<Message>, IRollback, IEquatable<Base<Message>>, IComparable<Message>
    {
        public string OfficeId { get; set; }

        public string TaskId { get; set; }

        public string BidId { get; set; }

        public string ChatId { get; set; }

        public string UserId { get; set; }

        public DateTime Sent { get; set; }
        public bool IsRead { get; set; }

        public string Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Image { get; set; }
        
        public PublishStatus PublishStatus { get; set; }

        public Message() { }

        public Message(string json) : base(json) { }

        protected override void UpdateFields(Message that)
        {
            this.OfficeId = that.OfficeId;
            this.TaskId = that.TaskId;
            this.BidId = that.BidId;
            this.ChatId = that.ChatId;
            this.Text = that.Text;
            this.UserId = that.UserId;
            this.Sent = that.Sent;
            this.PublishStatus = that.PublishStatus;
            this.Image = that.Image;
            this.IsRead = that.IsRead;
        }

        public int CompareTo(Message that)
        {
            return this.Sent.CompareTo(that.Sent);
        }
        
        
        [JsonIgnore]
        public ImageSource ImageSource
        {
            get
            {
                if (this.Image != null)
                {
#if DEBUG
                    return ImageSource.FromUri(new Uri("https://toolitplay.blob.core.windows.net/images/" + this.Image));
#else
                    return ImageSource.FromUri(new Uri("https://toolitlive.blob.core.windows.net/images/" + this.Image));
#endif             
                }
                else
                {
                    return null;
                }
            }
        }
    }
}