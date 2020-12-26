namespace IoTHubClientGeneratorTest
{
    public class TestDeviceStatusHandler
    {
        [TestCase("TestDeviceStatus")]
        public static string TestDeviceStatus =>
            @"namespace TestDeviceStatus
{
    [IoTHub()]
    class MyIoTHubClient
    {
         [IoTHubDeviceStatusChangesHandler]
         private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
         {
         }
    }
}";
        
        [TestCase("TestConnectionStatusProperty")]
        public static string TestConnectionStatusProperty =>
            @"namespace TestConnectionStatusProperty
{
    [IoTHub()]
    class MyIoTHubClient
    {
         [ConnectionStatus] 
         private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }
    }
}";
        
        [TestCase("TestConnectionStatusPropertyAndMethod")]
        public static string TestConnectionStatusPropertyAndMethod =>
            @"namespace TestConnectionStatusPropertyAndMethod
{
    [IoTHub()]
    class MyIoTHubClient
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