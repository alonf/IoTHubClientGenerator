using System;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Add a <see cref="ITransportSettings"/> instance to the Device Client creation
    /// You can have more than one property decorated with this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TransportSettingAttribute : Attribute
    {
    }
}