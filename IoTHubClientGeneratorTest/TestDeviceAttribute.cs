namespace IoTHubClientGeneratorTest
{
    public class TestDeviceAttribute
    {
        [TestCase("TestSimpleDevice")] 
        public static string TestSimpleDevice => 
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestSimpleDevice
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Device]
        private DeviceClient MyClient {get;set;}
    }
}";
        [TestCase("TestConnectionString")] 
        public static string TestConnectionString => 
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestConnectionString
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Device(ConnectionString=""HostName=HomeAutomationHub.azure-devices.net;SharedAccessKeyName=device;SharedAccessKey=ROQYwme5GAWZxKdI5rIjLsimSMTfltIdLm/Cki3qfBq="")]
        private DeviceClient MyClient {get;set;}
    }
}";
        
        [TestCase("TestConnectionStringEnv")] 
        public static string TestConnectionStringEnv => 
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestConnectionStringEnv
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Device(ConnectionString=""%ConStr%"")]
        private DeviceClient MyClient {get;set;}
    }
}";
        
        [TestCase("TestTransportSettings")] 
        public static string TestTransportSettings => 
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using System;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;

namespace TestTransportSettings
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
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
        [Device(ConnectionString=""%ConStr%"")]
        private DeviceClient MyClient {get;set;}
    }
}";
        
        [TestCase("TestTransportSettingsAndClientOptions")] 
        public static string TestTransportSettingsAndClientOptions => 
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using System;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;

namespace TestTransportSettingsAndClientOptions
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
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
        [Device(ConnectionString=""%ConStr%"")]
        private DeviceClient MyClient {get;set;}
    }
}";
    }
}