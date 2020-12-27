namespace IoTHubClientGeneratorTest
{
    public class TestDirectMethodHandling
    {
        [TestCase("TestDirectMethod")]
        public static string TestDirectMethod =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using System;
using System.Threading.Tasks;

namespace TestDirectMethod
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [DirectMethod]
        private Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)
        {
            System.Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            System.Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
        
        [DirectMethod(CloudMethodName = ""TestMethod"")]
        private Task<MethodResponse> WriteToConsole2Async(MethodRequest methodRequest)
        {
            System.Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            System.Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }
}";
        
        [TestCase("TestDirectMethodWithErrorHandling")]
        public static string TestDirectMethodWithErrorHandling =>
@"
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using System;
using System.Threading.Tasks;

namespace TestDirectMethodWithErrorHandling
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [DirectMethod]
        private Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)
        {
            System.Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            System.Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
            if (exception is IotHubException {IsTransient: true})
            {
                System.Console.WriteLine($""Error: {errorMessage}"");
                System.Console.WriteLine($""An IotHubException was caught, but will try to recover and retry: {exception}"");
            }
        }

        [DirectMethod(CloudMethodName = ""TestMethod"")]
        private Task<MethodResponse> WriteToConsole2Async(MethodRequest methodRequest)
        {
            System.Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            System.Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }
}";
    }
}