using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a device client property with DPS settings.
    /// It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DpsX509CertificateDeviceAttribute : DpsDeviceAttribute
    {
        /// <summary>
        /// The PFX certificate to load for device provisioning authentication
        /// </summary>
        public string CertificatePath { get; set; }

        /// <summary>
        /// The password of the PFX certificate file
        /// </summary>
        public string CertificatePassword { get; set; }
    }
}