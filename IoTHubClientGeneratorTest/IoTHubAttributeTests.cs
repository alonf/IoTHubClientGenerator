namespace IoTHubClientGeneratorTest
{
    public class IoTHubAttributeTests
    {
        [TestCase("TestIoTHubOnly")]
        public static string TestIoTHubOnly => 
@"
using IoTHubClientGeneratorSDK;
using System;

namespace TestIoTHubOnly
{
    [IoTHub]
    partial class MyIoTHubClient
    {
        
    }
}";
        
        [TestCase("TestIoTHubGenerateSendTelemetry")]
        public static string TestIoTHubGenerateSendTelemetry => 
@"
using IoTHubClientGeneratorSDK;
using System;

namespace TestIoTHubGenerateSendTelemetry
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetry"")]
    partial class MyIoTHubClient
    {
        
    }
}";
        
        [TestCase("TestIoTHubGenerateSendTelemetryWithErrorHandling")]
        public static string TestIoTHubGenerateSendTelemetryWithErrorHandling => 
            @"
using IoTHubClientGeneratorSDK;
using System;

namespace TestIoTHubGenerateSendTelemetryWithErrorHandling
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetry"")]
    partial class MyIoTHubClient
    {
        
        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
             System.Console.WriteLine($""Error: {errorMessage}"");
             System.Console.WriteLine($""An Exception was caught: {exception}"");
        }
    }
}";
        
        [TestCase("TestIoTHubGenerateSendTelemetryWithErrorHandlingAndConnectionStatus")]
        public static string TestIoTHubGenerateSendTelemetryWithErrorHandlingAndConnectionStatus => 
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using System;

namespace TestIoTHubGenerateSendTelemetryWithErrorHandlingAndConnectionStatus
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetry"")]
    partial class MyIoTHubClient
    {
        
        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
             System.Console.WriteLine($""Error: {errorMessage}"");
             System.Console.WriteLine($""An Exception was caught: {exception}"");
        }

        [ConnectionStatus] 
        private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }

        [IoTHubDeviceStatusChangesHandler]
        private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
        }
    }
}";

        
        [TestCase("TestTwoIoTHubsGeneration")]
        public static string TestTwoIoTHubsGeneration => 
@"
using IoTHubClientGeneratorSDK;
using System;

namespace TestTwoIoTHubsGeneration
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetry"")]
    partial class MyIoTHubClient1
    {
        
    }
    [IoTHub()]
    partial class MyIoTHubClient2
    {
        
    }
}";
        
    }
}