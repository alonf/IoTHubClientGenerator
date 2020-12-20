using System;

namespace IoTHubClientGeneratorSDK
{
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