using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class User : Base<User>, IUpdate<User>, IRollback, IEquatable<Base<User>>
    {
        public string FirstName { get; set; }
        
        public string Nid { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string MiddleNames { get; set; }

        public string LastName { get; set; }
        
        public string PreferredName { get; set; }
        
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[] Images { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }

        public string Description { get; set; }
        
        //[DataMember(Name = "rating", EmitDefaultValue = false)]
        //public int[] Rating { get; set; }
        
        public string Address { get; set; }
        
        public string Postcode { get; set; }
        
        public string City { get; set; }
        
        public string Country { get; set; }

        public DateTime Started { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Device> Devices { get; set; }

        public User() { }

        public User(string json) : base(json) { }

        protected override void UpdateFields(User that)
        {
            this.FirstName = that.FirstName;
            this.Nid = that.Nid;
            this.MiddleNames = that.MiddleNames;
            this.LastName = that.LastName;
            this.PreferredName = that.PreferredName;
            this.Message = that.Message;
            this.Images = that.Images;
            this.Email = that.Email;
            this.Phone = that.Phone;
            this.Description = that.Description;
            //this.Rating = that.Rating;
            this.Address = that.Address;
            this.Postcode = that.Postcode;
            this.City = that.City;
            this.Country = that.Country;
            this.Started = that.Started;
        }
      
        public string FullName
        {
            get
            {
                if (MiddleNames != null)
                {
                    return FirstName + " " + MiddleNames + " " + LastName;
                }
                else
                {
                    return FirstName + " " + LastName;
                }
            }
        }

        public string Name
        {
            get
            {
                return PreferredName + " " + LastName;
            }
        }

        [JsonIgnore]
        public ImageSource ImageSource
        {
            get
            {
                if (this.Images?.FirstOrDefault() != null)
                {
#if DEBUG
                    return ImageSource.FromUri(new Uri("https://toolitplay.blob.core.windows.net/images/" + this.Images.First()));
#else
                    return ImageSource.FromUri(new Uri("https://toolitlive.blob.core.windows.net/images/" + this.Images.First()));
#endif             
                }
                else
                {
                    return null;
                }
            }
        }

        public class Device
        {
            public string Handle { get; set; }
            public string AppId { get; set; }
            public int Build { get; set; }
            public string OS { get; set; }
            public string OSVer { get; set; }
        }
    }
}