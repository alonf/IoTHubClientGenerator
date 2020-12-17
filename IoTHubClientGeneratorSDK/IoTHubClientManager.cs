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
        public static DeviceClient CreateAuthenticationWithRegistrySymmetricKey(DeviceAttribute deviceAttribute)
        {
            return null;
        }

        public static DeviceClient CreateAuthenticationWithSharedAccessPolicyKey(DeviceAttribute deviceAttribute)
        {
            return null;
        }

        public static DeviceClient CreateAuthenticationWithToken(DeviceAttribute deviceAttribute)
        {
            return null;
        }
    }
}