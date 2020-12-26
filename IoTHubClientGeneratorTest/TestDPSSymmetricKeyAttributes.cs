namespace IoTHubClientGeneratorTest
{
    public class TestDPSSymmetricKeyAttributes
    {
        [TestCase("TestDPSSymmetricKey")]
        public static string TestDPSSymmetricKey =>
            @"
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