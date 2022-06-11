namespace IoTHubClientGeneratorTest
{
    public class TestNoNamespace
    {
        [TestCase("TestNoNamespaceDevice")] 
        public static string NoNamespace => 
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;

[IoTHub()]
partial class MyIoTHubClient
{
    [Device]
    private DeviceClient MyClient {get;set;}
}";
    }
}