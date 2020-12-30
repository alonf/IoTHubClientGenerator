using System;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDpsX509Certificate(AttributeSyntax attributeSyntax)
        {
            AppendLine("using System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = LoadProvisioningCertificate(theCertificatePath, theCertificatePassword);");
            AppendLine("using var security = new Microsoft.Azure.Devices.Shared.SecurityProviderX509Certificate(certificate);");

            CreateProvisioningDeviceClient(attributeSyntax);
            AppendLine("IAuthenticationMethod auth = new Microsoft.Azure.Devices.Client.DeviceAuthenticationWithX509Certificate(result.DeviceId, certificate);");

            return () =>
            {
                AppendLine("private System.Security.Cryptography.X509Certificates.X509Certificate2 LoadProvisioningCertificate(string certificatePath, string certificatePassword)");
                using (Block())
                {
                    AppendLine("var certificateCollection = new System.Security.Cryptography.X509Certificates.X509Certificate2Collection();");
                    AppendLine("certificateCollection.Import(certificatePath, certificatePassword, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.UserKeySet);");
                    AppendLine("System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = null;");
                    using (Foreach("var element in certificateCollection"))
                    {
                        using (If("certificate == null && element.HasPrivateKey"))
                        {
                            AppendLine("certificate = element;");
                        }
                        using (Else())
                        {
                            AppendLine("element.Dispose();");
                        }
                    }
                    AppendLine();
                    using (If("certificate == null"))
                    {
                        AppendLine("throw new System.IO.FileNotFoundException($\"{certificatePath} did not contain any certificate with a private key.\");");
                    }
                    AppendLine("return certificate;");
                }
                AppendLine();
            };
        }
    }
}