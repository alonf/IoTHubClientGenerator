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
    class MyIoTHubClient
    {
        [DirectMethod]
        private Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)
        {
            Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
        
        [DirectMethod(CloudMethodName = ""TestMethod"")]
        private Task<MethodResponse> WriteToConsole2Async(MethodRequest methodRequest)
        {
            Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
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
    class MyIoTHubClient
    {
        [DirectMethod]
        private Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)
        {
            Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
            if (exception is IotHubException {IsTransient: true})
            {
                Console.WriteLine($""Error: {errorMessage}"");
                Console.WriteLine($""An IotHubException was caught, but will try to recover and retry: {exception}"");
            }
        }

        [DirectMethod(CloudMethodName = ""TestMethod"")]
        private Task<MethodResponse> WriteToConsole2Async(MethodRequest methodRequest)
        {
            Console.WriteLine($""\t *** {methodRequest.Name} was called."");
            Console.WriteLine($""\t{methodRequest.DataAsJson}\n"");
            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }
    }
}";
    }
}