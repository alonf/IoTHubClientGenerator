using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a device client property with DPS settings.
    /// It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DpsSymmetricKeyDeviceAttribute : DpsDeviceAttribute
    {
        /// <summary>
        /// The primary key for individual or group enrollment
        /// </summary>
        public string PrimarySymmetricKey { get; set; }
        
        /// <summary>
        /// The secondary key for individual or group enrollment
        /// </summary>
        public string SecondarySymmetricKey { get; set; }
    }
}