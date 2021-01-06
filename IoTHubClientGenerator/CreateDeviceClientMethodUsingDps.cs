﻿using System;
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
                using (Block())
                {
                    if (attributeSyntax?.ArgumentList != null)
                    {
                        foreach (var argument in attributeSyntax.ArgumentList.Arguments)
                        {
                            var attAssignment = $"the{argument.NameEquals}";
                            var attExpression = argument.Expression.ToString();
                            CreateVariableAssignmentLineFromAttributeParameter(attAssignment, attExpression);
                        }
                    }
                    else
                    {
                        _diagnosticsManager.Report(DiagnosticId.DpsAttributeMissingProperties, Location.Create(attributeSyntax!.SyntaxTree, attributeSyntax.Span));
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
                additionalCode();
            };
        }
    }
}