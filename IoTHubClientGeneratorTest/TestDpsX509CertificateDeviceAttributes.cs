namespace IoTHubClientGeneratorTest
{
    public class TestDpsX509CertificateDeviceAttributes
    {
        [TestCase("TestDpsX509Certificate")]
        public static string TestDpsX509Certificate =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDpsX509Certificate
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsX509CertificateDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Http1,
            TransportType=TransportType.Amqp, CertificatePath=""%certificateFilePath%"", CertificatePassword=""%CertPassword%"")]
        private DeviceClient MyClient {get;set;}
    }
}";
        [TestCase("TestDpsX509CertificateWithTransportSettings")]
        public static string TestDpsX509CertificateWithTransportSettings =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace TestDpsX509Certificate
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsX509CertificateDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt,
            CertificatePath=""%certificateFilePath%"", CertificatePassword=""%CertPassword%"")]
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
        [TestCase("TestDpsX509CertificateWithTransportSettingsAndClientOptions")]
        public static string TestDpsX509CertificateWithTransportSettingsAndClientOptions =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using System;

namespace TestDpsX509Certificate
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsX509CertificateDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Mqtt,
            CertificatePath=""%certificateFilePath%"", CertificatePassword=""%CertPassword%"")]
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
        
        [TestCase("TestDpsX509CertificateWithClientOptions")]
        public static string TestDpsX509CertificateWithClientOptions =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDpsX509Certificate
{
    [IoTHub]
    class MyIoTHubClient
    {
        [DpsX509CertificateDevice(DPSIdScope=""%DpsScopeId%"", Id=""1"", EnrollmentGroupId=""GroupId"", 
            EnrollmentType=DPSEnrollmentType.Individual, DPSTransportType=TransportType.Amqp,TransportType=TransportType.Amqp,
            CertificatePath=""%certificateFilePath%"", CertificatePassword=""%CertPassword%"")]
        private DeviceClient MyClient {get;set;}

        [ClientOptions]
        public ClientOptions ClientOptions { get; } = new();
    }
}";
    }
}