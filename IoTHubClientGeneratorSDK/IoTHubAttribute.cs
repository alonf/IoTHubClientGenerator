using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a class to serves as an IoTHub management class
    /// At least one class must be decorated as IoTHub to activate the source code generation.
    /// There can be more then one IoTHub class in case we need to be connected with more than one Device Client
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IoTHubAttribute : Attribute
    {
        /// <summary>
        /// if used, a send method with the prototype:  private async Task MethodNameAsync(string jsonPayload, string messageId, CancellationToken cancellationToken, IDictionary&lt;string, string&gt; properties = null) is created
        /// call this method to send the telemetry data to the IoT Hub
        /// </summary>
        public string GeneratedSendMethodName { get; set; }
    }
}