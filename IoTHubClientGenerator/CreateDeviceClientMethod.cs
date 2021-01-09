using System.Collections.Generic;
using System.Text;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateDeviceClientMethod(string methodName, AttributeSyntax attributeSyntax)
        {
            AppendLine($"private Microsoft.Azure.Devices.Client.DeviceClient {methodName}()");
            using (Block())
            {
                var parameterNameList = new List<string>();
                if (attributeSyntax?.ArgumentList != null)
                {
                    AppendLine();
                    AppendLine("#pragma warning disable CS4014");
                    AppendLine();
                    foreach (var argument in attributeSyntax.ArgumentList.Arguments)
                    {
                        var attAssignment = $"the{argument.NameEquals}";
                        parameterNameList.Add(argument.NameEquals?.ToString().TrimEnd('=', ' ', '\t').Trim());
                        var attExpression = argument.Expression.ToString();
                        CreateVariableAssignmentLineFromAttributeParameter(attAssignment, attExpression);
                    }
                    AppendLine();
                    AppendLine("#pragma warning restore CS4014");
                    AppendLine();
                }
                else
                {
                    parameterNameList.Add(nameof(DeviceAttribute.ConnectionString));
                    AppendLine("var theConnectionString=System.Environment.GetEnvironmentVariable(\"ConnectionString\");");
                }

                var createDeviceError = new StringBuilder();

                string clientOptionsPropertyName = GetClientOptionsPropertyName();

                var hasTransportSettingsAttributes = HandleTransportSettingsAttributes();

                string authenticationMethodPropertyName = GetAuthenticationMethodPropertyName();

                var creationFunctionEntry = new StringBuilder();
                if (parameterNameList.Contains(nameof(DeviceAttribute.ConnectionString)))
                {
                    createDeviceError.Append("ConnectionString ");
                    creationFunctionEntry.Append("cs_");
                }

                if (parameterNameList.Contains(nameof(DeviceAttribute.Hostname)))
                {
                    createDeviceError.Append("Hostname ");
                    creationFunctionEntry.Append("hn_");
                }

                if (parameterNameList.Contains(nameof(DeviceAttribute.GatewayHostname)))
                {
                    createDeviceError.Append("GatewayHostname ");
                    creationFunctionEntry.Append("gw_");
                }

                if (parameterNameList.Contains(nameof(DeviceAttribute.TransportType)))
                {
                    createDeviceError.Append("TransportType ");
                    creationFunctionEntry.Append("tt_");
                }

                if (parameterNameList.Contains(nameof(DeviceAttribute.DeviceId)))
                {
                    createDeviceError.Append("DeviceId ");
                    creationFunctionEntry.Append("did_");
                }

                if (hasTransportSettingsAttributes)
                {
                    createDeviceError.Append("ITransportSettings[] ");
                    creationFunctionEntry.Append("ts_");
                }

                if (authenticationMethodPropertyName is not null) 
                {
                    createDeviceError.Append("AuthenticationMethod ");
                    creationFunctionEntry.Append("am_");
                }

                if (clientOptionsPropertyName is not null) 
                {
                    createDeviceError.Append("ClientOptions ");
                    creationFunctionEntry.Append("co_");
                }

                if (creationFunctionEntry.Length == 0) //no parameters
                {
                    Location location = null;
                    if (attributeSyntax != null)
                    {
                        location = Location.Create(attributeSyntax.SyntaxTree, attributeSyntax.Span);
                    }
                    _diagnosticsManager.Report(DiagnosticId.DeviceParametersError, location);
                    return;
                }

                Append("var deviceClient = ");

                creationFunctionEntry.Remove(creationFunctionEntry.Length - 1, 1); //remove the last _

                //for debug:
                //AppendLine($"//{creationFunctionEntry}");

                switch (creationFunctionEntry.ToString())
                {
                    case "cs":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)});");
                        break;

                    case "cs_co":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, {clientOptionsPropertyName});");
                        break;

                    case "cs_ts":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, transportSettings);");
                        break;

                    case "cs_ts_co":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, transportSettings, {clientOptionsPropertyName});");
                        break;

                    case "cs_tt":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.TransportType)});");
                        break;

                    case "cs_tt_co":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.TransportType)}, {clientOptionsPropertyName});");
                        break;

                    case "cs_did":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)});");
                        break;

                    case "cs_did_co":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, {clientOptionsPropertyName});");
                        break;

                    case "cs_tt_did":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, the{nameof(DeviceAttribute.TransportType)});");
                        break;

                    case "cs_did_ts":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, transportSettings);");
                        break;

                    case "cs_did_ts_co":
                        AppendLine(
                            $"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, transportSettings, {clientOptionsPropertyName});");
                        break;

                    case "hn_gw_tt_am":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethodPropertyName}, the{nameof(DeviceAttribute.TransportType)});");
                        break;

                    case "hn_gw_tt_am_co":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethodPropertyName}, the{nameof(DeviceAttribute.TransportType)}, {clientOptionsPropertyName});");
                        break;

                    case "hn_gw_ts_am":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethodPropertyName}, transportSettings);");
                        break;

                    case "hn_gw_ts_am_co":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethodPropertyName}, transportSettings, {clientOptionsPropertyName});");
                        break;

                    case "hn_gw_am":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethodPropertyName});");
                        break;

                    case "hn_gw_am_co":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethodPropertyName}, {clientOptionsPropertyName});");
                        break;

                    case "hn_tt_am":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethodPropertyName}, the{nameof(DeviceAttribute.TransportType)});");
                        break;

                    case "hn_tt_am_co":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethodPropertyName}, the{nameof(DeviceAttribute.TransportType)}, {clientOptionsPropertyName});");
                        break;

                    case "hn_ts_am":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethodPropertyName}, transportSettings);");
                        break;

                    case "hn_ts_am_co":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethodPropertyName}, transportSettings, {clientOptionsPropertyName});");
                        break;

                    case "hn_am":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethodPropertyName});");
                        break;

                    case "hn_am_co":
                        AppendLine(
                            $"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethodPropertyName}, {clientOptionsPropertyName});");
                        break;

                    default:
                        Location location = null;
                        if (attributeSyntax != null)
                        {
                            location = Location.Create(attributeSyntax.SyntaxTree, attributeSyntax.Span);
                        }
                        _diagnosticsManager.Report(DiagnosticId.ParametersMismatch, location, createDeviceError.ToString());
                        
                        AppendLine(" null;");
                        break;
                }
                AppendLine("return deviceClient;");
            }
        }
    }
}