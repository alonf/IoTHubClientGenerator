using System;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDpsTpm(AttributeSyntax attributeSyntax)
        {
            AppendLine("using var security = new SecurityProviderTpmHsm(theId);");
            CreateProvisioningDeviceClient(attributeSyntax);
            AppendLine("IAuthenticationMethod auth = new DeviceAuthenticationWithTpm(result.DeviceId,security);");
            return () => { };
        }
    }
}
