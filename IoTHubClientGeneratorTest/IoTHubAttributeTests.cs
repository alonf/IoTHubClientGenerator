namespace IoTHubClientGeneratorTest
{
    public class IoTHubAttributeTests
    {
        [TestCase("TestIoTHubOnly")]
        public static string TestIoTHubOnly => 
@"
namespace TestIoTHubOnly
{
    [IoTHub]
    class MyIoTHubClient
    {
        
    }
}";
        
        [TestCase("TestIoTHubGenerateSendTelemetry")]
        public static string TestIoTHubGenerateSendTelemetry => 
@"
namespace TestIoTHubGenerateSendTelemetry
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetry"")]
    class MyIoTHubClient
    {
        
    }
}";
        
        [TestCase("TestTwoIoTHubsGeneration")]
        public static string TestTwoIoTHubsGeneration => 
@"
namespace TestTwoIoTHubsGeneration
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetry"")]
    class MyIoTHubClient1
    {
        
    }
    [IoTHub()]
    class MyIoTHubClient2
    {
        
    }
}";
        
    }
}