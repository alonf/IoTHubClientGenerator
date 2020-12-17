using System;
using System.Security;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DeviceAttribute : Attribute
    {
        /// <summary>
        /// The fully-qualified DNS host name of IoT Hub
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The fully-qualified DNS host name of Gateway
        /// </summary>
        public string GatewayHostname { get; set; }

        /// <summary>
        /// The transportType used (Http1, AMQP or MQTT)
        /// </summary>
        public TransportType TransportType { get; set; }

        /// <summary>
        /// Device Identifier
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Key associated with the device, module, shared-access-policy or DPS symmetric key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Name of the shared access policy to use
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        /// Module Identifier
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Security token associated with the device
        /// </summary>
        public string Token { get; set; }

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

        /// <summary>
        /// The X509 Certificate file path for DPS
        /// </summary>
        public string CertificatePath { get; set; }

        /// <summary>
        /// The X509 Certificate password for DPS
        /// </summary>
        public SecureString CertificatePassword { get; set; }

        /// <summary>
        /// Try to reconnect if the connection dropped
        /// </summary>
        public bool AutoReconnect { get; set; } = false;
        
    }
}