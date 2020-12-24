using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// register a method with a prototype such as: private Task&lt;MethodResponse&gt; WriteToConsoleAsync(MethodRequest methodRequest)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DirectMethodAttribute : Attribute
    {
        /// <summary>
        /// The name that the cloud uses to call this method
        /// If not set, the name is the decorated C# method name
        /// </summary>
        public string CloudMethodName { get; set; }
        
    }
}