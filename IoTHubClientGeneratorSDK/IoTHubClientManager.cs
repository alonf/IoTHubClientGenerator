using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    //todo: may be created in the code generator, hence we do not need this
    public static class IoTHubClientManager
    {
        public static async Task ReconnectAsync()
        {
            await Task.Delay(1);
        }

        public static async Task RunAsync()
        {
            await Task.Delay(1);
        }
        
        public static DeviceClient DeviceClient { get; set; } 
        
        
        public static ConnectionStatus ConnectionStatus { get; private set; }
    }
}