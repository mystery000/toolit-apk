using System.Collections.Generic;

namespace Toolit.Helpers
{
    public class DeviceInstallation
    {
        public string InstallId { get; set; }

        public string Platform { get; set; }

        public string Token { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}