using System.Linq;
using IoTHubClientGeneratorSDK;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateSendMethod()
        {
            var iotHubAttributes = GetAttributes(nameof(IoTHubAttribute)).ToArray();
            if (iotHubAttributes.Length == 0)
                return;

            var iotHubAttribute = iotHubAttributes[0];

            var sendMethodName = iotHubAttribute.Key.ArgumentList?.Arguments.Where(p =>
                    p.NameEquals != null && p.NameEquals.ToString().StartsWith(nameof(IoTHubAttribute.GeneratedSendMethodName)))
                .Select(a => a.Expression.ToString()).FirstOrDefault();
            
            if (string.IsNullOrWhiteSpace(sendMethodName))
                return;
            sendMethodName = sendMethodName.TrimStart('"').TrimEnd('"');   
            AppendLine($"private async Task<bool> {sendMethodName}(string jsonPayload, string messageId, System.Threading.CancellationToken cancellationToken, System.Collections.Generic.IDictionary<string, string> messageProperties = null)");
            AppendLine("{");
            using (Indent(this))
            {
                AppendLine("try", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this, _isErrorHandlerExist))
                {
                    if (_isConnectionStatusExist)
                    {
                        AppendLine($"if ({_connectionStatusAccessText} != Microsoft.Azure.Devices.Client.ConnectionStatus.Connected)");
                        AppendLine("{");
                        using (Indent(this))
                        {
                            AppendLine("string errorMessage = \"Error sending message to the IoT Hub, the device is not connected\";", _isErrorHandlerExist);
                            AppendLine("var exception = new System.Exception(errorMessage);", _isErrorHandlerExist);
                            AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                            AppendLine("return false;");
                        }

                        AppendLine("}");
                    }

                    AppendLine("var iotMessage = new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.UTF8.GetBytes(jsonPayload))");
                    AppendLine("{");
                    using (Indent(this))
                    {
                        AppendLine("MessageId = messageId,");
                        AppendLine("ContentEncoding = System.Text.Encoding.UTF8.ToString(),");
                        AppendLine("ContentType = \"application/json\"");
                    }
                    AppendLine("};");
                    AppendLine($"await {_deviceClientPropertyName}.SendEventAsync(iotMessage, cancellationToken);");
                    AppendLine("iotMessage.Dispose();");
                    AppendLine("}", _isErrorHandlerExist);
                }

                AppendLine("catch (System.Exception exception)", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this, _isErrorHandlerExist))
                {
                    AppendLine("string errorMessage = \"Error sending message to the IoT Hub\";", _isErrorHandlerExist);
                    AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                    AppendLine("return false;",_isErrorHandlerExist);
                }

                AppendLine("}", _isErrorHandlerExist);
                AppendLine("return true;");
            }
            AppendLine("}");
        }
    }
}