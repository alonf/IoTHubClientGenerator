using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// decorate a function with a prototype such as: private async Task StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
    /// It will be called whenever there is a change in the connectivity state of the device client
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class IoTHubDeviceStatusChangesHandlerAttribute : Attribute
    {

    }
}