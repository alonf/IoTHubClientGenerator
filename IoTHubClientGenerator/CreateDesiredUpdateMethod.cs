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
            AppendLine(
                "private async Task HandleDesiredPropertyUpdateAsync(Microsoft.Azure.Devices.Shared.TwinCollection desiredProperties, object context)");
            AppendLine("{");
            using (Indent(this, _isErrorHandlerExist))
            {
                AppendLine("try", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);

                using (Indent(this))
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

                        AppendLine($"if (desiredProperties.Contains(\"{desiredPropertyName}\"))");
                        AppendLine("{");
                        using (Indent(this))
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
                                AppendLine(
                                    $"{propertyName} = {typeInfo.Type!.Name}.Parse(desiredProperties[\"{desiredPropertyName}\"]);");
                            }
                        }

                        AppendLine("}");
                    }
                }

                AppendLine("}", _isErrorHandlerExist);
                AppendLine("catch(System.Exception exception)", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this, _isErrorHandlerExist))
                {
                    AppendLine("string errorMessage =\"Error updating desired properties\";", _isErrorHandlerExist);
                    AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                }

                AppendLine("}", _isErrorHandlerExist);
            }

            AppendLine("await Task.CompletedTask;");
            AppendLine("}");
        }
    }
}