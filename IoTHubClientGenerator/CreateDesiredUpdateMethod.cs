using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateDesiredUpdateMethod()
        {
            AppendLine("private async Task HandleDesiredPropertyUpdateAsync(Microsoft.Azure.Devices.Shared.TwinCollection desiredProperties, object context)");
            using (Block())
            {
                using(Try(_isErrorHandlerExist))
                {
                    var programDesiredProperties = GetAttributedMembers(nameof(DesiredAttribute)).ToArray();

                    foreach (var desiredProperty in programDesiredProperties)
                    {
                        string propertyName = ((PropertyDeclarationSyntax) desiredProperty.Key).Identifier.ToString();
                        string desiredPropertyName = propertyName;

                        var twinPropertyAttribute = desiredProperty.Value.First(v =>
                            v.Name.ToString() == nameof(DesiredAttribute).AttName());

                        var twinPropertyNameProperty = twinPropertyAttribute.ArgumentList?.Arguments.FirstOrDefault();

                        if (twinPropertyNameProperty != null)
                            desiredPropertyName = twinPropertyNameProperty.Expression.ToString().TrimStart('\"')
                                .TrimEnd('\"');

                        using(If($"desiredProperties.Contains(\"{desiredPropertyName}\")"))
                        {
                            var semanticModel =
                                _generatorExecutionContext.Compilation.GetSemanticModel(desiredProperty.Key
                                    .SyntaxTree);
                            var typeInfo =
                                semanticModel.GetTypeInfo(((PropertyDeclarationSyntax) desiredProperty.Key).Type);

                            if (typeInfo.Type?.Name == nameof(System.String))
                            {
                                AppendLine($"{propertyName} = desiredProperties[\"{desiredPropertyName}\"];");
                            }
                            else
                            {
                                AppendLine($"string textData = desiredProperties[\"{desiredPropertyName}\"];");
                                AppendLine(
                                    $"{propertyName} = {typeInfo.Type!.Name}.Parse(textData);");
                            }
                        }
                    }
                }
                using (Catch("System.Exception exception", _isErrorHandlerExist))
                {
                    AppendLine("string errorMessage =\"Error updating desired properties\";", _isErrorHandlerExist);
                    AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                }
                AppendLine("await Task.CompletedTask;");
            }
        }
    }
}