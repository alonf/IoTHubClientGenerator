namespace IoTHubClientGeneratorTest
{
    public class TestDeviceStatusHandler
    {
        [TestCase("TestDeviceStatus")]
        public static string TestDeviceStatus =>
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestDeviceStatus
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
         [IoTHubDeviceStatusChangesHandler]
         private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
         {
         }
    }
}";

        [TestCase("TestConnectionStatusProperty")]
        public static string TestConnectionStatusProperty =>
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
    }
}";

        [TestCase("TestConnectionStatusPropertyAndMethod")]
        public static string TestConnectionStatusPropertyAndMethod =>
            @"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

namespace TestConnectionStatusPropertyAndMethod
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
         [ConnectionStatus] 
         private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }

         [IoTHubDeviceStatusChangesHandler]
         private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
         {
         }
    }
}";
    }
}
