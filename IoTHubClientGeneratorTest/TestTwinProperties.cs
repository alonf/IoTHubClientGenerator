namespace IoTHubClientGeneratorTest
{
    public class TestTwinProperties
    {
        [TestCase("TestReportedProperties")] 
        public static string TestReportedProperties => 
@"
using IoTHubClientGeneratorSDK;
using System;

namespace TestReportedProperties
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Reported(""valueFromTheDevice"")] private string _reportedPropertyDemo;

        [Reported(""ReportedPropertyAutoNameDemo"", ""reportedPropertyAutoNameDemo"")] private string _reportedPropertyAutoNameDemo;
    }
}";
        
        [TestCase("TestDesiredProperties")] 
        public static string TestDesiredProperties => 
@"
using IoTHubClientGeneratorSDK;
using System;

namespace TestDesiredProperties
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Desired] public string DesiredProperty { get; private set; }
        [Desired(""valueFromTheCloud"")] private string DesiredPropertyDemo { get; set; }
    }
}";
        
        
        [TestCase("TestNonStringReportedProperties")] 
        public static string TestNonStringReportedProperties => 
            @"
using IoTHubClientGeneratorSDK;
using System;

namespace TestNonStringReportedProperties
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Reported(""valueFromTheDevice"")] private int _reportedPropertyDemo;

        [Reported(""ReportedPropertyDouble"", ""reportedPropertyDouble"")] private double _reportedPropertyDouble;

        [Reported(""ReportedPropertyObject"", ""reportedPropertyObject"")] private object _reportedPropertyObject;

        [Reported(""ReportedPropertyDecimal"", ""reportedPropertyDecimal"")] private decimal _reportedPropertyDecimal;

        [Reported(""ReportedPropertyUint"", ""reportedPropertyUint"")] private uint _reportedPropertyUint;
    }
}";
        
        [TestCase("TestNonStringDesiredProperties")] 
        public static string TestNonStringDesiredProperties => 
            @"
using IoTHubClientGeneratorSDK;
using System;

namespace TestNonStringReportedProperties
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Desired] public int DesiredProperty { get; private set; }
        [Desired(""doubleProperty"")] private double DoubleProperty { get; set; }
        [Desired(""decimalProperty"")] private decimal DecimalProperty { get; set; }
        [Desired(""uintProperty"")] private uint UintProperty { get; set; }
    }
}";
        
        
        
        [TestCase("TestTwinPropertiesAndErrorHandling")] 
        public static string TestTwinPropertiesAndErrorHandling => 
@"
using IoTHubClientGeneratorSDK;
using System;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace TestTwinPropertiesAndErrorHandling
{
    [IoTHub()]
    partial class MyIoTHubClient
    {
        [Desired] public string DesiredProperty { get; private set; }
        [Desired(""valueFromTheCloud"")] private string DesiredPropertyDemo { get; set; }
        [Reported(""valueFromTheDevice"")] private string _reportedPropertyDemo;
        [Reported(""ReportedPropertyAutoNameDemo"", ""reportedPropertyAutoNameDemo"")] private string _reportedPropertyAutoNameDemo;

        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
            if (exception is IotHubException {IsTransient: true})
            {
                System.Console.WriteLine($""Error: {errorMessage}"");
                System.Console.WriteLine($""An IotHubException was caught, but will try to recover and retry: {exception}"");
            }
        }
    }
}";
    }
}

