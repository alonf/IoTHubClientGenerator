using System;
using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDpsX509Certificate(AttributeSyntax attributeSyntax)
        {
            AppendLine("using X509Certificate2 certificate = LoadProvisioningCertificate(theCertificatePath, theCertificatePassword);");
            AppendLine(" using var security = new SecurityProviderX509Certificate(certificate);");

            CreateProvisioningDeviceClient(attributeSyntax);
            AppendLine("IAuthenticationMethod auth = new DeviceAuthenticationWithX509Certificate(result.DeviceId, certificate);");

            return () =>
            {
                AppendLine("private X509Certificate2 LoadProvisioningCertificate(string certificatePath, string certificatePassword)");
                AppendLine("{");
                using (Indent(this))
                {
                    AppendLine("var certificateCollection = new X509Certificate2Collection();");
                    AppendLine(
                        "certificateCollection.Import(certificatePath, certificatePassword, X509KeyStorageFlags.UserKeySet);");
                    AppendLine("X509Certificate2 certificate = null;");
                    AppendLine("foreach (X509Certificate2 element in certificateCollection)");
                    AppendLine("{");
                    using (Indent(this))
                    {
                        AppendLine("if (certificate == null && element.HasPrivateKey)");
                        AppendLine("{");
                        using (Indent(this))
                        {
                            AppendLine("certificate = element;");
                        }

                        AppendLine("}");
                        AppendLine("else");
                        AppendLine("{");
                        using (Indent(this))
                        {
                            AppendLine("element.Dispose();");
                        }

                        AppendLine("}");
                    }

                    AppendLine("}");
                    AppendLine();
                    AppendLine("if (certificate == null)");
                    AppendLine("{");
                    using (Indent(this))
                    {
                        var certificateNameValue = GetAttributePropertyValue(attributeSyntax,
                            nameof(DpsX509CertificateDeviceAttribute.CertificatePath));
                        AppendLine(
                            $"throw new FileNotFoundException(\"{certificateNameValue} did not contain any certificate with a private key.\");");
                    }

                    AppendLine("}");
                    AppendLine("return certificate;");
                }

                AppendLine("}");
                AppendLine();
            };
        }
    }
}