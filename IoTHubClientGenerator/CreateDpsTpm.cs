using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDpsTpm(AttributeSyntax attributeSyntax)
        {
            AppendLine("using var security = new Microsoft.Azure.Devices.Provisioning.Security.SecurityProviderTpmHsm(theId);");
            CreateProvisioningDeviceClient(attributeSyntax);
            AppendLine("IAuthenticationMethod auth = new Microsoft.Azure.Devices.Client.DeviceAuthenticationWithTpm(result.DeviceId,security);");
            return () => { };
        }
    }
}
