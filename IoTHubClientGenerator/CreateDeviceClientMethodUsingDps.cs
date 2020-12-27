using System;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private Action CreateDeviceClientMethodUsingDps(string methodName, AttributeSyntax attributeSyntax)
        {
            return () =>
            {
                Action additionalCode = () => { };
                AppendLine($"private async Task<Microsoft.Azure.Devices.Client.DeviceClient> {methodName}()");
                AppendLine("{");
                using (Indent(this))
                {
                    if (attributeSyntax?.ArgumentList != null)
                    {
                        foreach (var argument in attributeSyntax.ArgumentList.Arguments)
                        {
                            var attAssignment = $"the{argument.NameEquals}";
                            var attExpression = argument.Expression.ToString();
                            if (attExpression.StartsWith("\"%") && attExpression.EndsWith("%\""))
                            {
                                attExpression =
                                    $"System.Environment.GetEnvironmentVariable(\"{attExpression.TrimStart('%', '"').TrimEnd('%', '"')}\")";
                            }

                            AppendLine($"var {attAssignment}{attExpression};");
                        }
                    }
                    else
                    {
                        _generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(new
                                DiagnosticDescriptor("IoTGen007", "IoT Hub Generator Error",
                                    "Dps attribute must define properties",
                                    "Error", DiagnosticSeverity.Warning, true),
                            Location.Create(attributeSyntax!.SyntaxTree, attributeSyntax.Span)));
                        return;
                    }

                    string clientOptionsPropertyName = GetClientOptionsPropertyName();

                    var hasTransportSettingsAttributes = HandleTransportSettingsAttributes();

                    switch (attributeSyntax.Name + "Attribute")
                    {
                        case nameof(DpsX509CertificateDeviceAttribute):
                            additionalCode = CreateDpsX509Certificate(attributeSyntax);
                            break;

                        case nameof(DpsSymmetricKeyDeviceAttribute):
                            additionalCode = CreateDpsSymmetricKey(attributeSyntax);
                            break;

                        case nameof(DpsTpmDeviceAttribute):
                            additionalCode = CreateDpsTpm(attributeSyntax);
                            break;
                    }


                    Append("var deviceClient = DeviceClient.Create(result.AssignedHub, auth, ");
                    Append(hasTransportSettingsAttributes ? "transportSettings" : "theTransportType");

                    AppendLine(clientOptionsPropertyName != null ? $", {clientOptionsPropertyName});" : ");");

                    AppendLine("return deviceClient;");
                }

                AppendLine("}");
                additionalCode();
            };
        }
    }
}