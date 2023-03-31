using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Ad : Base<Ad>, IUpdate<Ad>, IRollback, IEquatable<Base<Ad>>
    {
        public string Image { get; set; }

        public string Video { get; set; }

        public string Company { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Deleted { get; set; }
       

        public Ad() { }

        public Ad(string json) : base(json) { }

        protected override void UpdateFields(Ad that)
        {
            this.Image = that.Image;
            this.Video = that.Video;
            this.Company = that.Company;
            this.Title = that.Title;
            this.Url = that.Url;
            this.Text = that.Text;
            this.Deleted = that.Deleted;
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

        [JsonIgnore]
        public ImageSource VideoSource
        {
            get
            {
                if (this.Video != null)
                {
#if DEBUG
                    return ImageSource.FromUri(new Uri("https://toolitplay.blob.core.windows.net/videos/" + this.Video));
#else
                    return ImageSource.FromUri(new Uri("https://toolitlive.blob.core.windows.net/videos/" + this.Video));
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