namespace IoTHubClientGeneratorTest
{
    public class TestTwinProperties
    {
        [TestCase("TestReportedProperties")] 
        public static string TestReportedProperties => 
@"namespace TestReportedProperties
{
    [IoTHub()]
    class MyIoTHubClient
    {
        [Reported(""valueFromTheDevice"")] private string _reportedPropertyDemo;

        [Reported(""ReportedPropertyAutoNameDemo"", ""reportedPropertyAutoNameDemo"")] private string _reportedPropertyAutoNameDemo;
    }
}";
        
        [TestCase("TestDesiredProperties")] 
        public static string TestDesiredProperties => 
@"namespace TestDesiredProperties
{
    [IoTHub()]
    class MyIoTHubClient
    {
         [Desired] public string DesiredProperty { get; private set; }
        [Desired(""valueFromTheCloud"")] private string DesiredPropertyDemo { get; set; }
    }
}";
        
        [TestCase("TestTwinPropertiesAndErrorHandling")] 
        public static string TestTwinPropertiesAndErrorHandling => 
@"namespace TestTwinPropertiesAndErrorHandling
{
    [IoTHub()]
    class MyIoTHubClient
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
                Console.WriteLine($""Error: {errorMessage}"");
                Console.WriteLine($""An IotHubException was caught, but will try to recover and retry: {exception}"");
            }
        }
    }
}";
    }
}

