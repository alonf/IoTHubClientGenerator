using System;
using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateDeviceClientInitialization()
        {
            Action createCreateDeviceMethod;
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

            AppendLine("public async Task InitIoTHubClientAsync()");
            AppendLine("{");
            AppendLine("await System.Threading.Tasks.Task.CompletedTask; //suppress async warning in case we don't generate any async call");
          
            using (Indent(this))
            {
                AppendLine("try", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this, _isErrorHandlerExist))
                {
                    //get DpsAttribute
                    var isDpsAttributeSet = IsAttributeExist(nameof(DpsX509CertificateDeviceAttribute),
                        nameof(DpsSymmetricKeyDeviceAttribute), nameof(DpsTpmDeviceAttribute));
                    if (isDpsAttributeSet)
                        createCreateDeviceMethod = CreateDeviceUsingDps();
                    else
                    {
                        createCreateDeviceMethod = CreateDevice();
                    }
                    /*
                     * Create Device using the Device attribute info
                     */
                }

                /*
                 * Set connection status change handler
                 */
                
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
                AppendLine();

                /*
                 * Handle desired properties
                 */
                if (GetAttributes(nameof(DesiredAttribute)).Any())
                {
                    AppendLine($"await {_deviceClientPropertyName}.SetDesiredPropertyUpdateCallbackAsync(HandleDesiredPropertyUpdateAsync, null);");
                }
                AppendLine();
                /*
                 * Handle cloud to device messages handling
                 */
                var c2dMessageAttributes = GetAttributes(nameof(C2DMessageAttribute)).ToArray();
                if (c2dMessageAttributes.Length != 0)
                {
                    var c2dMessageAttribute = c2dMessageAttributes[0];
                    var c2dMessageHandlerMethodName =
                        ((MethodDeclarationSyntax) c2dMessageAttribute.Value).Identifier.ToString();
                    var isc2dMessageHandlerMethodAsync =
                        ((MethodDeclarationSyntax) c2dMessageAttribute.Value).Modifiers.Any(a=>a.ToString() == "async");

                    var autoComplete = c2dMessageAttribute.Key.ArgumentList?.Arguments
                        .Where(a => a.NameEquals != null && a.NameEquals.ToString().StartsWith(nameof(C2DMessageAttribute.AutoComplete)))
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
                                AppendLine("message.Dispose();");
                                AppendLine("message = null;");
                            }
                            AppendLine("}");
                            AppendLine("catch(System.Exception)", !_isErrorHandlerExist);
                            AppendLine("catch(System.Exception exception)", _isErrorHandlerExist);
                            AppendLine("{");
                            using (Indent(this))
                            {
                                AppendLine($"await {_deviceClientPropertyName}.RejectAsync(message);");
                                AppendLine("if (message != null)");
                                Append("message.Dispose();", true);
                                AppendLine("string errorMessage =\"Error handling cloud to device message. The message has been rejected\";", _isErrorHandlerExist);
                                AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                            }
                            AppendLine("}");
                        }
                        AppendLine("}, null);");
                    }
                    else
                    {
                        AppendLine($"await {_deviceClientPropertyName}.SetReceiveMessageHandlerAsync(async (message, userContext) => await {c2dMessageHandlerMethodName}(message), null);");
                    }
                }

                AppendLine();
                CreateDirectMethodCallback();
                
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

            createCreateDeviceMethod();
        }

        private Action CreateDevice()
        {
            bool shouldGenerateDeviceClientProperty = false;
            AttributeSyntax createDeviceClientMethodAttributeSyntax = null;
            //get the Device attribute
            var deviceAttributes = GetAttributes(nameof(DeviceAttribute)).ToArray();


            if (deviceAttributes.Length == 0) //no device attribute
            {
                _generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(new
                        DiagnosticDescriptor("IoTGen006", "IoT Hub Generator Warning",
                            "No DeviceClient property decorated with [Device] attribute exist, A DeviceClient property with default ConnectionString environment variable has been created!",
                            "Warning", DiagnosticSeverity.Warning, true),
                    Location.None));

                shouldGenerateDeviceClientProperty = true;
                _deviceClientPropertyName = "DeviceClient";
            }
            else
            {
                var deviceAttribute = deviceAttributes[0];
                _deviceClientPropertyName =
                    ((PropertyDeclarationSyntax) (deviceAttribute.Value)).Identifier.ToString();

                createDeviceClientMethodAttributeSyntax = deviceAttribute.Key;
            }

            var createDeviceClientMethodName = $"Create{_deviceClientPropertyName}";
            AppendLine($"{_deviceClientPropertyName} =  {createDeviceClientMethodName}();");
            return () =>
            {
                if (shouldGenerateDeviceClientProperty)
                    CreateDeviceClientProperty();
                CreateDeviceClientMethod(createDeviceClientMethodName, createDeviceClientMethodAttributeSyntax);
            };
        }
        
        private Action CreateDeviceUsingDps()
        {
            AttributeSyntax createDeviceClientMethodAttributeSyntax = null;
            //get the Device attribute
            var dpsAttribute = (from name in new[]
                {
                    nameof(DpsX509CertificateDeviceAttribute),
                    nameof(DpsSymmetricKeyDeviceAttribute), nameof(DpsTpmDeviceAttribute)
                }
                let attributes = GetAttributes(name)
                where attributes.Any()
                select attributes).First().First();
            
                        
            _deviceClientPropertyName =
                    ((PropertyDeclarationSyntax) (dpsAttribute.Value)).Identifier.ToString();

                createDeviceClientMethodAttributeSyntax = dpsAttribute.Key;
            

            var createDeviceClientMethodName = $"Create{_deviceClientPropertyName}";
            AppendLine($"{_deviceClientPropertyName} =  {createDeviceClientMethodName}();");
            return CreateDeviceClientMethodUsingDps(createDeviceClientMethodName, createDeviceClientMethodAttributeSyntax);
        }

        private void CreateDeviceClientProperty()
        {
            AppendLine("[Device(ConnectionString=\"%ConnectionString%\")]");
            AppendLine("private DeviceClient DeviceClient {get; set;}");
            AppendLine();
        }
    }
}