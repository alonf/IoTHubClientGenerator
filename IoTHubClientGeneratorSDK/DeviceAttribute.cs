using System;
using System.Security;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DeviceAttribute : Attribute
    {
        /// <summary>
        /// Connection string for the IoT hub (with DeviceId)
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// The fully-qualified DNS host name of IoT Hub
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// The fully-qualified DNS host name of Gateway
        /// </summary>
        public string GatewayHostname { get; set; }

        /// <summary>
        /// The default transport type (Http1, AMQP or MQTT) if not at least one TransportAttribute property exist
        /// </summary>
        public TransportType TransportType { get; set; }

        /// <summary>
        /// Device Identifier
        /// </summary>
        public string DeviceId { get; set; }

        

        /// <summary>
        /// Try to reconnect if the connection dropped
        /// </summary>
        public bool AutoReconnect { get; set; } = false;
        
    }
}