using System;
using System.Runtime.Serialization;

namespace Toolit
{
    [DataContract]
    [KnownType(typeof(CardType))]
    [KnownType(typeof(Psp))]
    public class Card : Base<Card>, IUpdate<Card>, IRollback, IEquatable<Base<Card>>
    {
        public string UserId { get; set; }

        public CardType Type { get; set; }
      
        public string Last { get; set; }

        public Psp Psp { get; set; }

        public string Token { get; set; }

        public string Note { get; set; }

        public bool Active { get; set; }

        [IgnoreDataMember]
        private string rawExpiry;

        private string RawExpiry
        {
            get
            {
                return rawExpiry;
            }
            set
            {
                HeldExpiry = Utilities.ParseISO8601Date(value); // Make sure it parses.
                rawExpiry = value;
            }
        }

        [IgnoreDataMember]
        private DateTime? HeldExpiry { get; set; }

        [IgnoreDataMember]
        public DateTime Expiry // Latest modified timestamp.
        {
            get
            {
                if (RawExpiry == null)
                {
                    return DateTime.MinValue;
                }
                else if (HeldExpiry == null)
                {
                    HeldExpiry = Utilities.ParseISO8601Date(RawExpiry);
                }

                return HeldExpiry.Value;
            }
            set
            {
                RawExpiry = value.ToString("o");
                HeldExpiry = value;
            }
        }

        public Card() { }

        public Card(string json) : base(json) { }

        protected override void UpdateFields(Card that)
        {
            this.UserId = that.UserId;
            this.Type = that.Type;
            this.Note = that.Note;
            this.Active = that.Active;
            this.Last = that.Last;
            this.Expiry = that.Expiry;
        }

        public override string ToString()
        {
            return "Card: " + Last + " " + Id + " " + UserId;
        }
    }
}
