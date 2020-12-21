using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a method with a prototype: private async Task OnC2dMessageReceivedAsync(Message receivedMessage)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class C2DMessageAttribute : Attribute
    {
        /// <summary>
        /// If set to true, the message is auto completed if there is no exception thrown
        /// </summary>
        public bool AutoComplete { get; set; } = true;
       
    }
}