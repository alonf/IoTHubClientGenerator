using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDpsSymmetricKey(AttributeSyntax attributeSyntax)
        {
            AppendLine("using var security = new SecurityProviderSymmetricKey(theId, thePrimarySymmetricKey, null);");
            CreateProvisioningDeviceClient(attributeSyntax);
            AppendLine("IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId,security.GetPrimaryKey());");

            return () =>
            {
                AppendLine("private static string ComputeDerivedSymmetricKey(string enrollmentKey, string deviceId)");
                AppendLine("{");
                using (Indent(this))
                {
                    AppendLine("if (string.IsNullOrWhiteSpace(theEnrollmentKey))");
                    AppendLine("{");
                    using (Indent(this))
                    {
                        AppendLine("return enrollmentKey;");
                    }

                    AppendLine("}");
                    AppendLine("using var hmac = new HMACSHA256(Convert.FromBase64String(theEenrollmentKey));");
                    AppendLine("return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(deviceId)));");
                }
                AppendLine("}");
            };
        }
    }
}