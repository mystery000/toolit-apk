using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace Toolit
{
    [DataContract]
    public class Craft
    {
        public string Id { get; set; }

        public CraftStatus Status { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string CertificateId { get; set; }

        public string CraftType { get; set; }
    }
}