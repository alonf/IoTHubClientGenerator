using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Put on a method with the prototype: void HandleError(string message, System.Exception exception)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IoTHubErrorHandlerAttribute : Attribute
    {
    }
}