using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a device client property with DPS settings.
    /// It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DpsTpmDeviceAttribute : DpsDeviceAttribute
    {
        /// <summary>
        /// Gets the TPM endorsement key. Use this option by itself to get the EK needed to create a DPS individual enrollment
        /// </summary>
        public bool GetTpmEndorsementKey { get; set; }
    }
}