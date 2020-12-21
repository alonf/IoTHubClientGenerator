using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// register a method with a prototype such as: private Task&lt;MethodResponse&gt; WriteToConsoleAsync(MethodRequest methodRequest)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class C2DeviceCallbackAttribute : Attribute
    {

    }
}