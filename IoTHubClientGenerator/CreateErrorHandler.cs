using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateErrorHandlerMethodCallPattern()
        {
            var errorHandlerAttributes = GetAttributes(nameof(IoTHubErrorHandlerAttribute)).ToArray();
            if (errorHandlerAttributes.Length == 0)
                return;

            var errorHandlerAttribute = errorHandlerAttributes[0];

            _isErrorHandlerExist = true;
            var errorHandlerMethodName =
                ((MethodDeclarationSyntax) (errorHandlerAttribute.Value)).Identifier.ToString();
            _callErrorHandlerPattern = $"{errorHandlerMethodName}(errorMessage, exception);";
        }
    }
}