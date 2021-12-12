using System;

namespace F29API.Services
{
    public class AboutService
    {
        public const string VERSION = "v0.1.72";

        public void Initialize()
        {
            InstanceID = Guid.NewGuid().ToString();
        }

        public string InstanceID { get; private set; }

        public string Version => VERSION;
    }
}
