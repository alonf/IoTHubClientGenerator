namespace IoTHubClientGeneratorTest
{
    public class TestCodeGenerationErrorsAndWarnings
    {
       [TestCase("TestMaxElementsShouldBeDecorated")]
        public static string TestMinElementsShouldBeDecorated =>
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestConnectionStatusProperty
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
         [ConnectionStatus] 
         private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }

        [ConnectionStatus] 
         private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus1 { get; set; }
    }
}";
       
        [TestCase("TestAttributeParametersMismatch")]
        public static string TestAttributeParametersMismatch =>
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestAttributeParametersMismatch
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [Device(DeviceId = ""id"")]
        private DeviceClient DeviceClient {get;set;}
         
    }
}";
        
        [TestCase("TestDpsAttributeMissingProperties")]
        public static string TestDpsAttributeMissingProperties =>
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestAttributeParametersMismatch
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [DpsSymmetricKeyDevice(Id=""id"", DPSIdScope=""scopeId"")]
        private DeviceClient DeviceClient {get;set;}
    }
}";
        
        [TestCase("TestSingleDeviceOrDPSAttribute")]
        public static string TestSingleDeviceOrDPSAttribute =>
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestSingleDeviceOrDPSAttribute
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        [Device]
        private DeviceClient DeviceClient {get;set;}

        [DpsSymmetricKeyDevice(DPSIdScope=""scope"", DPSTransportType=TransportType.Mqtt, TransportType=TransportType.Mqtt,
        EnrollmentGroupId=""%EnrollmentGroupId%"", EnrollmentType=DPSEnrollmentType.Group, 
        Id=""%RegistrationId%"", PrimarySymmetricKey=""%SymKey%"")]
        private DeviceClient DpsDeviceClient {get;set;}
         
    }
}";
    }
}