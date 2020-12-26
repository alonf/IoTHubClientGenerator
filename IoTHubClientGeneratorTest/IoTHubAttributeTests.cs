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
    class MyIoTHubClient
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
    class MyIoTHubClient
    {
        
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