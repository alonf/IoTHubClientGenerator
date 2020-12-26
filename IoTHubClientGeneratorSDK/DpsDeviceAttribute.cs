using System;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Base class for DPS based device creation
    /// It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();
    /// </summary>
    public abstract class DpsDeviceAttribute : Attribute
    {
        /// <summary>
        /// The Id Scope of the DPS instance
        /// </summary>
        public string DPSIdScope { get; set; }

        /// <summary>
        /// TThe registration Id when using individual enrollment, or the desired device Id when using group enrollment
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// An optional device identifier
        /// </summary>
        public string OptionalDeviceId { get; set; }
        
        /// <summary>
        /// The enrollment group identifier
        /// </summary>
        public string EnrollmentGroupId { get; set; }
        
        /// <summary>
        /// The type of enrollment: Individual or Group
        /// </summary>
        public DPSEnrollmentType EnrollmentType { get; set; }

        /// <summary>
        /// The global endpoint for devices to connect to
        /// </summary>
        public string GlobalDeviceEndpoint { get; set; } = "global.azure-devices-provisioning.net";

        /// <summary>
        /// The transport to use to communicate with the device provisioning instance. Possible values include Mqtt, Mqtt_WebSocket_Only, Mqtt_Tcp_Only, Amqp, Amqp_WebSocket_Only, Amqp_Tcp_only, and Http1
        /// </summary>
        public TransportType DPSTransportType { get; set; }
        
        /// <summary>
        /// The transport to use to communicate with the IoTHub. Possible values include Mqtt, Mqtt_WebSocket_Only, Mqtt_Tcp_Only, Amqp, Amqp_WebSocket_Only, Amqp_Tcp_only, and Http1
        /// </summary>
        public TransportType TransportType { get; set; }
    }
}