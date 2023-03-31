using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Task : Base<Task>, IUpdate<Task>, IRollback, IEquatable<Base<Task>>
    {
        public string Title { get; set; }

        public string OfficeId { get; set; }

        public string UserId { get; set; }

        public string[] Crafts { get; set; }

        public string[] Tags { get; set; }

        public string Description { get; set; }
        
        public string DateDone { get; set; }

        public string Address { get; set; }

        public string Postcode { get; set; }

        public string City { get; set; }

        public decimal Price { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Images { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Videos { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string AcceptedBid { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? Finished { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? Rated { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PaymentId { get; set; }

        public PublishStatus PublishStatus { get; set; }

        public bool UseRotRut { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RealestateUnion { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ApartmentNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PropertyDesignation { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool ShowPublicly { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Deleted { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool CraftsmanIndicatedFinished { get; set; }

        public Task() { }

        public Task(string json) : base(json) { }

        protected override void UpdateFields(Task that)
        {
            this.Title = that.Title;
            this.Description = that.Description;
            this.UserId = that.UserId;
            this.Crafts = that.Crafts;
            this.Tags = that.Tags;
            this.DateDone = that.DateDone;
            this.Price = that.Price;
            this.Images = that.Images;
            this.Videos = that.Videos;
            this.AcceptedBid = that.AcceptedBid;
            this.Address = that.Address;
            this.Postcode = that.Postcode;
            this.City = that.City;
            this.PaymentId = that.PaymentId;
            this.Rated = that.Rated;
            this.Finished = that.Finished;
            this.PublishStatus = that.PublishStatus;
            this.PropertyDesignation = that.PropertyDesignation;
            this.RealestateUnion = that.RealestateUnion;
            this.ApartmentNumber = that.ApartmentNumber;
            this.UseRotRut = that.UseRotRut;
            this.ShowPublicly = that.ShowPublicly;
            this.Deleted = that.Deleted;
        }

        [JsonIgnore]
        public ImageSource[] ImageSources
        {
            get
            {
                if (this.Images?.Length > 0)
                {
#if DEBUG
                    return this.Images.Select(img => ImageSource.FromUri(new Uri("https://toolitplay.blob.core.windows.net/images/" + img))).ToArray();
#else
                    return this.Images.Select(img => ImageSource.FromUri(new Uri("https://toolitlive.blob.core.windows.net/images/" + img))).ToArray();
#endif             
                }
                else
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public ImageSource[] VideoSources
        {
            get
            {
                if (this.Videos?.Length > 0)
                {
#if DEBUG
                    return this.Videos.Select(vid => ImageSource.FromUri(new Uri("https://toolitplay.blob.core.windows.net/videos/" + vid))).ToArray();
#else
                    return this.Videos.Select(vid => ImageSource.FromUri(new Uri("https://toolitlive.blob.core.windows.net/videos/" + vid))).ToArray();
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
