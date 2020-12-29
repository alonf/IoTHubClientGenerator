using System;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Provide a client option instance that is used when creating the device client
    /// The property should return an <see cref="ClientOptions"/> instance
    /// </summary>
    /// <example>
    /// private ClientOptions _clientOptions = new ClientOptions() {FileUploadTransportSettings = new Http1TransportSettings(){Proxy = new System.Net.WebProxy("https://myproxy")}};
    /// </example>
   
    [AttributeUsage(AttributeTargets.Property)]
    public class ClientOptionsAttribute : Attribute
    {
    }
}