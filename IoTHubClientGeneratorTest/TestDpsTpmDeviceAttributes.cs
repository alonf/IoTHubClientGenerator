namespace IoTHubClientGeneratorTest
{
    public class TestDpsTpmDeviceAttributes
    {
        [TestCase("TestDpsTpmAttributes")]
        public static string TestDpsTpm =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDpsTpm
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [DpsTpmDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Http1,
            TransportType=TransportType.Amqp)]
        private DeviceClient MyClient {get;set;}
    }
}";
        [TestCase("TestDpsTpmWithTransportSettings")]
        public static string TestDpsTpmWithTransportSettings =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace TestDpsTpm
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [DpsTpmDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt)]
        private DeviceClient MyClient {get;set;}

        [TransportSetting]
        public ITransportSettings AmqpTransportSettings { get; } = new AmqpTransportSettings(TransportType.Amqp)
        {
            AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings {MaxPoolSize = 5},
            IdleTimeout = TimeSpan.FromMinutes(1)
        };

        [TransportSetting]
        public ITransportSettings MqttTransportSetting { get; } = new MqttTransportSettings(TransportType.Mqtt)
        {
            DefaultReceiveTimeout = TimeSpan.FromMinutes(2)
        };
    }
}";
        [TestCase("TestDpsTpmWithTransportSettingsAndClientOptions")]
        public static string TestDpsTpmWithTransportSettingsAndClientOptions =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace TestDpsTpm
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [DpsTpmDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt)]
        private DeviceClient MyClient {get;set;}

        [TransportSetting]
        public ITransportSettings AmqpTransportSettings { get; } = new AmqpTransportSettings(TransportType.Amqp)
        {
            AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings {MaxPoolSize = 5},
            IdleTimeout = TimeSpan.FromMinutes(1)
        };
        
        [ClientOptions]
        public ClientOptions ClientOptions { get; } = new();

        [TransportSetting]
        public ITransportSettings MqttTransportSetting { get; } = new MqttTransportSettings(TransportType.Mqtt)
        {
            DefaultReceiveTimeout = TimeSpan.FromMinutes(2)
        };
    }
}";
        
        [TestCase("TestDpsTpmWithClientOptions")]
        public static string TestDpsTpmWithClientOptions =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDpsTpm
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [DpsTpmDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Amqp,TransportType=TransportType.Amqp)]
        private DeviceClient MyClient {get;set;}

        [ClientOptions]
        public ClientOptions ClientOptions { get; } = new();
    }
}";
    }
}