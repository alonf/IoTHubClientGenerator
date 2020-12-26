using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private string GetClientOptionsPropertyName()
        {
            string clientOptionsPropertyName = null;
            var clientOptionAttribute = GetAttributes(nameof(ClientOptionsAttribute)).ToArray();
            if (clientOptionAttribute.Length > 0)
            {
                clientOptionsPropertyName =
                    ((PropertyDeclarationSyntax) clientOptionAttribute[0].Value).Identifier.ToString();
            }

            return clientOptionsPropertyName;
        }

        private bool HandleTransportSettingsAttributes()
        {
            var transportSettingsAttributes = GetAttributes(nameof(TransportSettingAttribute)).ToArray();
            if (transportSettingsAttributes.Length == 0)
                return false;
            AppendLine("ITransportSettings [] transportSettings = new [] {");
            foreach (var transportSettingsAttribute in transportSettingsAttributes)
            {
                Append(((PropertyDeclarationSyntax) transportSettingsAttribute.Value).Identifier.ToString());
                Append(", ");
            }

            TrimEnd(2);
            AppendLine("};");

            return true;
        }

        
        private string GetAuthenticationMethodPropertyName()
        {
            string authenticationMethodPropertyName = null;
            var authenticationMethodAttributes = GetAttributes(nameof(AuthenticationMethodAttribute)).ToArray();
            if (authenticationMethodAttributes.Length > 0)
            {
                authenticationMethodPropertyName =
                    ((PropertyDeclarationSyntax) authenticationMethodAttributes[0].Value).Identifier.ToString();
            }
            return authenticationMethodPropertyName;
        }
    }
}