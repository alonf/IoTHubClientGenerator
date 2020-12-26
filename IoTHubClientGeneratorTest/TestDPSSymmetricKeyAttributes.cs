namespace IoTHubClientGeneratorTest
{
    public class TestDPSSymmetricKeyAttributes
    {
        [TestCase("TestDPSSymmetricKey")]
        public static string TestDPSSymmetricKey =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDPSSymmetricKey
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsSymmetricKeyDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Http1,
            TransportType=TransportType.Amqp, PrimarySymmetricKey=""%PrimaryDPSKey%"")]
        private DeviceClient MyClient {get;set;}
    }
}";
        [TestCase("TestDPSSymmetricKeyWithTransportSettings")]
        public static string TestDPSSymmetricKeyWithTransportSettings =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace TestDPSSymmetricKey
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsSymmetricKeyDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt,
            PrimarySymmetricKey=""%PrimaryDPSKey%"")]
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
        [TestCase("TestDPSSymmetricKeyWithTransportSettingsAndClientOptions")]
        public static string TestDPSSymmetricKeyWithTransportSettingsAndClientOptions =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace TestDPSSymmetricKey
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsSymmetricKeyDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt,
            PrimarySymmetricKey=""%PrimaryDPSKey%"")]
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
        
        [TestCase("TestDPSSymmetricKeyWithClientOptions")]
        public static string TestDPSSymmetricKeyWithClientOptions =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDPSSymmetricKey
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsSymmetricKeyDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Amqp,TransportType=TransportType.Amqp,
            PrimarySymmetricKey=""%PrimaryDPSKey%"")]
        private DeviceClient MyClient {get;set;}

        [ClientOptions]
        public ClientOptions ClientOptions { get; } = new();
    }
}";
        [TestCase("TestDPSSymmetricKeyWithTransportSettingsAndClientOptionsAndErrorHandling")]
        public static string TestDPSSymmetricKeyWithTransportSettingsAndClientOptionsAndErrorHandling =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Client.Exceptions;
using System;

namespace TestDPSSymmetricKey
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsSymmetricKeyDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt,
            PrimarySymmetricKey=""%PrimaryDPSKey%"")]
        private DeviceClient MyClient {get;set;}

        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
            if (exception is IotHubException {IsTransient: true})
            {
                Console.WriteLine($""Error: {errorMessage}"");
                Console.WriteLine($""An IotHubException was caught, but will try to recover and retry: {exception}"");
            }
        }

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
    }
}