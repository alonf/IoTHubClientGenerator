using System;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDpsSymmetricKey(AttributeSyntax attributeSyntax)
        {
            var dpsEnrollmentType =
                GetAttributePropertyValue(attributeSyntax, nameof(DpsDeviceAttribute.EnrollmentType));
            if (dpsEnrollmentType == nameof(DPSEnrollmentType) + "." + nameof(DPSEnrollmentType.Group))
            {
                AppendLine("thePrimarySymmetricKey = ComputeDerivedSymmetricKey(thePrimarySymmetricKey, theId);");
            }
            
            AppendLine("using var security = new SecurityProviderSymmetricKey(theId, thePrimarySymmetricKey, null);");
            
            CreateProvisioningDeviceClient(attributeSyntax);
            AppendLine("IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId,security.GetPrimaryKey());");

            return () =>
            {
                AppendLine("private static string ComputeDerivedSymmetricKey(string enrollmentKey, string deviceId)");
                using (Block())
                {
                    using (If("string.IsNullOrWhiteSpace(deviceId)"))
                    {
                        AppendLine("return enrollmentKey; //individual enrollment type"); 
                    }
                    AppendLine("using var hmac = new System.Security.Cryptography.HMACSHA256(System.Convert.FromBase64String(enrollmentKey));");
                    AppendLine("return Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(deviceId)));");
                }
            };
        }
    }
}