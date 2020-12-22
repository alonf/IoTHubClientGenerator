using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a device client property with DPS settings.
    /// It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DpsDeviceAttribute : Attribute
    {
        /// <summary>
        /// The DPS scope if when using DPS
        /// </summary>
        public string DPSIdScope { get; set; }

        /// <summary>
        /// The DPS service host url when using DPS
        /// </summary>
        public string DPSHost { get; set; }

        /// <summary>
        /// The key or token belong to a group and not individual device when using DPS
        /// </summary>
        public DPSEnrollmentType DPSEnrollmentType { get; set; }
    }
}