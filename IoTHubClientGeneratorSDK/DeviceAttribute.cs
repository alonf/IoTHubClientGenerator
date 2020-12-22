using System;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a property that will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();
    /// If not provided, a default Device property is created
    /// Example:
    /// <example>
    /// [Device(DeviceId = "%deviceId%", ConnectionString = "%connectionString%", TransportType = TransportType.Amqp)]
    /// public DeviceClient Client { get; set; }
    /// </example>
    /// </summary>
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