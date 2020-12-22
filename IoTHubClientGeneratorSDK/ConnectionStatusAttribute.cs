using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a property that reflect the current device client connection status and the reason
    /// for having the current status. 
    /// Example:
    /// <example>
    ///  [ConnectionStatus] 
    ///  private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ConnectionStatusAttribute : Attribute
    {

    }
}