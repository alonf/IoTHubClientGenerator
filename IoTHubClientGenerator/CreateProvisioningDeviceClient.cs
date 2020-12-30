using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        void CreateProvisioningDeviceClient(AttributeSyntax attributeSyntax)
        {
            var dpsTransportType =
                GetAttributePropertyValue(attributeSyntax, nameof(DpsDeviceAttribute.DPSTransportType));
                
            GenerateProvisioningTransportHandler(dpsTransportType);

           
            if (attributeSyntax.ArgumentList!.Arguments.Any(a =>
                a.NameEquals?.ToString().TrimEnd('=', ' ', '\t') == nameof(DpsTpmDeviceAttribute.GlobalDeviceEndpoint) == false))
            
            {
                AppendLine($"var the{nameof(DpsDeviceAttribute.GlobalDeviceEndpoint)} = \"global.azure-devices-provisioning.net\";");
            }
            AppendLine();
            AppendLine("ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(theGlobalDeviceEndpoint, theDPSIdScope, security, transport);");
            AppendLine("DeviceRegistrationResult result = await provClient.RegisterAsync();");
            AppendLine();
            using (If("result.Status != ProvisioningRegistrationStatusType.Assigned"))
            {
                AppendLine("throw new Exception($\"Registration status did not assign a hub, status: {result.Status}\");");
            }
            AppendLine();
        }
    }
}