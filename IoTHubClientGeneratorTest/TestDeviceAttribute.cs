namespace IoTHubClientGeneratorTest
{
    public class TestDeviceAttribute
    {
        [TestCase("TestSimpleDevice")] 
        public static string TestSimpleDevice => 
@"
namespace TestSimpleDevice
{
    [IoTHub()]
    class MyIoTHubClient
    {
        [Device]
        private DeviceClient MyClient {get;set;}
    }
}";
        [TestCase("TesConnectionString")] 
        public static string TesConnectionString => 
@"
namespace TesConnectionString
{
    [IoTHub()]
    class MyIoTHubClient
    {
        [Device(ConnectionString=""HostName=HomeAutomationHub.azure-devices.net;SharedAccessKeyName=device;SharedAccessKey=ROQYwme5GAWZxKdI5rIjLsimSMTfltIdLm/Cki3qfBq=""]
        private DeviceClient MyClient {get;set;}
    }
}";
        
        [TestCase("TesConnectionStringEnv")] 
        public static string TesConnectionStringEnv => 
            @"
namespace TesConnectionStringEnv
{
    [IoTHub()]
    class MyIoTHubClient
    {
        [Device(ConnectionString=""%ConStr%""]
        private DeviceClient MyClient {get;set;}
    }
}";
        
        [TestCase("TesTransportSettings")] 
        public static string TesTransportSettings => 
            @"
namespace TesTransportSettings
{
    [IoTHub()]
    class MyIoTHubClient
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
        [Device(ConnectionString=""%ConStr%""]
        private DeviceClient MyClient {get;set;}
    }
}";
        
        [TestCase("TesTransportSettingsAndClientOptions")] 
        public static string TesTransportSettingsAndClientOptions => 
            @"
namespace TesTransportSettingsAndClientOptions
{
    [IoTHub()]
    class MyIoTHubClient
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
        [Device(ConnectionString=""%ConStr%""]
        private DeviceClient MyClient {get;set;}
    }
}";
    }
}