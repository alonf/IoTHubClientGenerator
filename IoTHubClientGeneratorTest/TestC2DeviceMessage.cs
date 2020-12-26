namespace IoTHubClientGeneratorTest
{
    public class TestC2DeviceMessageGeneration
    {
        [TestCase("TestC2DeviceMessage")] 
        public static string TestC2DeviceMessage =>
@"
using IoTHubClientGeneratorSDK;

namespace TestC2DeviceMessage
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetryAsync"")]
    class MyIoTHubClient
    {
        
    }
}";
        
        [TestCase("TestC2DeviceMessageWithErrorHandling")] 
        public static string TestC2DeviceMessageWithErrorHandling =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client.Exceptions;
using System;

namespace TestC2DeviceMessageWithErrorHandling
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetryAsync"")]
    class MyIoTHubClient
    {
        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
            if (exception is IotHubException {IsTransient: true})
            {
                Console.WriteLine($""Error: {errorMessage}"");
                Console.WriteLine($""An IotHubException was caught, but will try to recover and retry: {exception}"");
            }
        }
    }
}";
    }
}