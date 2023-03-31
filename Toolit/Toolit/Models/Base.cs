using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace Toolit
{
    [DataContract]
    public abstract class Base<T> where T : Base<T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Id { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Hide { get; set; }

        [JsonIgnore]
        private string rawAt;

        private string RawAt
        {
            get
            {
                return rawAt;
            }
            set
            {
                if (value != null)
                {
                    HeldAt = Utilities.ParseISO8601Date(value); // Make sure it parses.
                }

                rawAt = value;
            }
        }

        [JsonIgnore]
        private DateTime? HeldAt { get; set; }

        public DateTime Modified // Latest modified timestamp.
        {
            get
            {
                if (RawAt == null)
                {
                    return DateTime.MinValue;
                }
                else if (HeldAt == null)
                {
                    HeldAt = Utilities.ParseISO8601Date(RawAt);
                }

                return HeldAt.Value;
            }
            set
            {
                RawAt = value.ToString("o");
                HeldAt = value;
            }
        }

        public delegate void UpdateHandler(T b);
        private event UpdateHandler UpdateEvent;

        public Base() { }

        public Base(string json)
        {
            Update(json, true);
        }

        protected abstract void UpdateFields(T that);

        private bool Update(T that, bool rollback)
        {
            if (this.Modified < that.Modified || rollback)
            {
                this.Id = that.Id;
                this.Hide = that.Hide;
                UpdateFields(that);
                this.Modified = that.Modified;
                if (!rollback)
                {
                    UpdateEvent?.Invoke(that); // Notify subscribers of update.
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Update(T that)
        {
            return Update(that, false);
        }

        private bool Update(string json, bool rollback)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var that = new DataContractJsonSerializer(this.GetType(), new DataContractJsonSerializerSettings { }).ReadObject(ms) as T; //DateTimeFormat = new DateTimeFormat("o")
            ms.Close();
            return Update(that, rollback);
        }

        public bool Update(string that)
        {
            return Update(that, false);
        }

        public string Json
        {
            get
            {
                var ms = new MemoryStream();
                var ser = new DataContractJsonSerializer(this.GetType(), new DataContractJsonSerializerSettings { }); //DateTimeFormat = new DateTimeFormat("o")
                ser.WriteObject(ms, this);
                var json = ms.ToArray();
                ms.Close();
                return Encoding.UTF8.GetString(json, 0, json.Length);
            }
        }

        private string transaction = null;

        public void Transaction()
        {
            if (transaction != null)
            {
                throw new IllegalStateException("Transaction already in progress.");
            }

            transaction = Json;
        }

        public void Rollback()
        {
            if (transaction == null)
            {
                throw new IllegalStateException("No transaction in progress.");
            }

            Update(transaction, true);
            transaction = null;
        }

        public void Commit()
        {
            if (transaction == null)
            {
                throw new IllegalStateException("No transaction in progress.");
            }

            transaction = null;
        }

        public void Subscribe(UpdateHandler subscriber)
        {
            UpdateEvent += subscriber;
        }

        public void Unsubscribe(UpdateHandler subscriber)
        {
            UpdateEvent -= subscriber;
        }

        public override string ToString()
        {
            return "Id: " + Id + " " + this.GetType();
        }

        public bool Equals(Base<T> that)
        {
            return Id == that.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Base<T> that ? Id == that.Id : false;
        }

        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Base<T> a, Base<T> b)
        {
            return a?.Id == b?.Id;
        }

        public static bool operator !=(Base<T> a, Base<T> b)
        {
            return !(a == b);
        }
    }
}
