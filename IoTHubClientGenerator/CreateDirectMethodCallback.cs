using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        //register for direct method cloud callback if needed: DeviceClient.SetMethodHandlerAsync() (per method), and DeviceClient.SetMethodDefaultHandlerAsync() for default
        private void CreateDirectMethodCallback()
        {
            var directMethodAttributes = GetAttributes(nameof(DirectMethodAttribute)).ToArray();
            if (directMethodAttributes.Length == 0) //nothing todo
                return;

            foreach (var directMethodAttribute in directMethodAttributes)
            {
                var directMethodAttributePropertyMethodName =
                    directMethodAttribute.Key.ArgumentList?.Arguments.FirstOrDefault()?.Expression.ToString()
                        .TrimStart('"').TrimEnd('"');
                
                var directMethodName =((MethodDeclarationSyntax) directMethodAttribute.Value).Identifier.ToString();


                AppendLine($"await {_deviceClientPropertyName}.SetMethodHandlerAsync(\"{directMethodAttributePropertyMethodName ?? directMethodName}\", async (message, _) =>");
                AppendLine("{");
                using (Indent(this))
                {
                    AppendLine("try", _isErrorHandlerExist);
                    AppendLine("{", _isErrorHandlerExist);
                    using (Indent(this, _isErrorHandlerExist))
                    {
                        Append($"return await {directMethodName}(message);");
                    }

                    AppendLine("}", _isErrorHandlerExist);
                    AppendLine("catch(System.Exception exception)", _isErrorHandlerExist);
                    AppendLine("{", _isErrorHandlerExist);
                    using (Indent(this, _isErrorHandlerExist))
                    {
                        AppendLine(
                            "string errorMessage =\"Error handling cloud to device message. The message has been rejected\";",
                            _isErrorHandlerExist);
                        AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                    }

                    AppendLine("}", _isErrorHandlerExist);
                    AppendLine("return new MethodResponse(new byte[0], 500);", _isErrorHandlerExist);
                }
                AppendLine("}, null);");
                AppendLine();
            }
        }
    }
}