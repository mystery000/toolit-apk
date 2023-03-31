using System;
using System.Runtime.Serialization;
using Xamarin.Forms;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;

namespace Toolit
{
    [DataContract]
    public class Office : Base<Office>, IUpdate<Office>, IRollback, IEquatable<Base<Office>>
    {
        public List<I18nString> Name { get; set; }

        public GeometryCollection Area { get; set; }

        public decimal BrokeragePercentage { get; set; }

        public Office() { }

        public Office(string json) : base(json) { }

        protected override void UpdateFields(Office that)
        {
            Name = that.Name;
            Area = that.Area;
            BrokeragePercentage = that.BrokeragePercentage;
        }
    }
}