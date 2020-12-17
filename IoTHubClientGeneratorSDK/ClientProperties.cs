using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    public class ClientProperties
    {
        public IAuthenticationMethod AuthenticationMethod { get; set; }
        public ITransportSettings TransportSettings { get; set; }

        public ClientOptions ClientOptions { get; set; }

        public string AlternateConnectionString { get; set; }
    }
}