using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a property that reflect the current device client connection status and the reason
    /// for having the current status.
    /// </summary>
    /// <example>
    ///  [ConnectionStatus] 
    ///  private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }
    /// </example>
   
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConnectionStatusAttribute : Attribute
    {

    }
}