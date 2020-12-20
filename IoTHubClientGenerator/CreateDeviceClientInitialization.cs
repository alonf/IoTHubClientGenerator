using System.Collections.Generic;
using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateDeviceClientInitialization()
        {
            string connectionStatusPropertyName = null;
            var connectionStatusAttributes = GetAttributes(nameof(ConnectionStatusAttribute)).ToArray();
            if (connectionStatusAttributes.Length != 0)
            {
                var connectionStatusAttribute = connectionStatusAttributes[0];
                _isConnectionStatusExist = true;

                var connectionStatusPropertyDeclarationSyntax =
                    (PropertyDeclarationSyntax) connectionStatusAttribute.Value;
                _connectionStatusAccessText = connectionStatusPropertyDeclarationSyntax.Identifier + ".ToTuple().Item1";
                connectionStatusPropertyName = connectionStatusPropertyDeclarationSyntax.Identifier.ToString();
            }

            string connectionStatusMethodName = null;
            var iotHubDeviceStatusChangesHandlerAttributes =
                GetAttributes(nameof(IoTHubDeviceStatusChangesHandlerAttribute)).ToArray();
            if (iotHubDeviceStatusChangesHandlerAttributes.Length != 0)
            {
                var iotHubDeviceStatusChangesHandlerAttribute = iotHubDeviceStatusChangesHandlerAttributes[0];
                connectionStatusMethodName =
                    ((MethodDeclarationSyntax) iotHubDeviceStatusChangesHandlerAttribute.Value).Identifier.ToString();
            }
            
            string createDeviceClientMethodName; 
            AttributeSyntax createDeviceClientMethodAttributeSyntax;
            AppendLine("private async Task InitIoTHubClientAsync()");
            AppendLine("{");
            using (Indent(this))
            {
                AppendLine("try", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this, _isErrorHandlerExist))
                {
                    //get the Device attribute
                    var deviceAttributes = GetAttributes(nameof(DeviceAttribute)).ToArray();
                    var deviceAttribute = deviceAttributes[0];
                    
                    _deviceClientPropertyName = ((PropertyDeclarationSyntax) (deviceAttribute.Value)).Identifier.ToString();
                    createDeviceClientMethodName = $"Create{_deviceClientPropertyName}";
                    AppendLine($"{_deviceClientPropertyName} =  {createDeviceClientMethodName}();");
                    createDeviceClientMethodAttributeSyntax = deviceAttribute.Key;
                }

                if (connectionStatusMethodName != null && _isConnectionStatusExist == false)
                {
                    AppendLine($"{_deviceClientPropertyName}.SetConnectionStatusChangesHandler({connectionStatusMethodName});");
                }

                if (_isConnectionStatusExist)
                {
                    AppendLine($"{_deviceClientPropertyName}.SetConnectionStatusChangesHandler((status, reason) =>");
                    AppendLine("{");
                    using (Indent(this))
                    {
                        AppendLine($"{connectionStatusPropertyName} = (status, reason);");
                        if (connectionStatusMethodName != null)
                            AppendLine($"{connectionStatusMethodName}(status, reason);");
                    }
                    AppendLine("});");
                }

                if (GetAttributes(nameof(DesiredAttribute)).Any())
                {
                    AppendLine($"await {_deviceClientPropertyName}.SetDesiredPropertyUpdateCallbackAsync(HandleDesiredPropertyUpdateAsync, null);");
                }

                var c2dMessageAttributes = GetAttributes(nameof(C2DMessageAttribute)).ToArray();
                if (c2dMessageAttributes.Length != 0)
                {
                    var c2dMessageAttribute = c2dMessageAttributes[0];
                    var c2dMessageHandlerMethodName =
                        ((MethodDeclarationSyntax) c2dMessageAttribute.Value).Identifier.ToString();
                    var isc2dMessageHandlerMethodAsync =
                        ((MethodDeclarationSyntax) c2dMessageAttribute.Value).Modifiers.Any(a=>a.ToString() == "async");

                    var autoComplete = c2dMessageAttribute.Key.ArgumentList?.Arguments
                        .Where(a => a.NameEquals.ToString().StartsWith(nameof(C2DMessageAttribute.AutoComplete)))
                        .Select(a => a.Expression.ToString()).FirstOrDefault();

                    if (autoComplete != null && autoComplete.ToLower() == "true")
                    {
                        AppendLine($"await {_deviceClientPropertyName}.SetReceiveMessageHandlerAsync(async (message, _) =>");
                        AppendLine("{");
                        using (Indent(this))
                        {
                            AppendLine("try");
                            AppendLine("{");
                            using (Indent(this))
                            {
                                if (isc2dMessageHandlerMethodAsync)
                                    Append("await ", true);
                                AppendLine($"{c2dMessageHandlerMethodName}(message);");
                               
                                AppendLine($"await {_deviceClientPropertyName}.CompleteAsync(message);");
                            }
                            AppendLine("}");
                            AppendLine("catch(System.Exception exception)");
                            AppendLine("{");
                            using (Indent(this))
                            {
                                AppendLine($"await {_deviceClientPropertyName}.RejectAsync(message);");
                                AppendLine("string errorMessage =\"Error handling cloud to device message. The message has been rejected\";");
                                AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                            }
                            AppendLine("}");
                        }
                        AppendLine("}, null);");
                    }
                    else
                    {
                        AppendLine($"await {_deviceClientPropertyName}.SetReceiveMessageHandlerAsync(async (message, userContext) => {c2dMessageHandlerMethodName}(message), null);");
                    }
                }
                CreateDirectMethodCallback();

                
               
                //register for direct method cloud callback if needed: DeviceClient.SetMethodHandlerAsync() (per method), and DeviceClient.SetMethodDefaultHandlerAsync() for default
               
                
                
                AppendLine("}", _isErrorHandlerExist);
                AppendLine("catch (System.Exception exception)", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this, _isErrorHandlerExist))
                {
                    AppendLine("string errorMessage = \"Error initiating device client\";", _isErrorHandlerExist);
                    AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                }

                AppendLine("}", _isErrorHandlerExist);
            }

            AppendLine("}");
            AppendLine();

           CreateDeviceClientMethod(createDeviceClientMethodName, createDeviceClientMethodAttributeSyntax);
        }
    }
}