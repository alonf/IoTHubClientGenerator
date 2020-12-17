using System.Text;
using Microsoft.CodeAnalysis;

namespace IoTHubClientGenerator
{
    using Microsoft.CodeAnalysis;


    public class IoTHubCodeGenerator
    {
        public void GenerateClientFactory(GeneratorExecutionContext context, StringBuilder sb)
        {
            sb.Append(@"
            partial class ClientFactory
            {
                partial void OnBeforeDeviceClientCreation(ClientProperties properties);

                public DeviceClient Create()
                {
                    var clientProperties = new ClientProperties();
                    //populate clientProperties from attributes

                    //partial method to enable altering the creation settings
                    OnBeforeDeviceClientCreation(clientProperties);

                    //one of:
                    // AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(); //if key is not null or empty
                    // AuthenticationMethodFactory.CreateAuthenticationWithSharedAccessPolicyKey(); //if policy is not null or empty
                    // AuthenticationMethodFactory.CreateAuthenticationWithToken(); //if token is not null or empty
                    return null;
                }
            }");
        }

    }
    
}