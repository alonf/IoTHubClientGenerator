using System.Linq;
using System.Threading.Tasks;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateReportedProperties()
        {
            var programReportedProperties = GetAttributedMembers(nameof(ReportedAttribute)).ToArray();
            if (programReportedProperties.Length == 0)
                return;
            
            AppendLine("private void ReportProperty<T>(string propertyName, T data)");
            using (Block())
            {
                using (Try(_isErrorHandlerExist))
                {
                    AppendLine("var reportedProperties = new Microsoft.Azure.Devices.Shared.TwinCollection();");
                    AppendLine("reportedProperties[propertyName] = data.ToString();");
                    AppendLine(
                        $"System.Threading.Tasks.Task.Run(async () => await {_deviceClientPropertyName}.UpdateReportedPropertiesAsync(reportedProperties));");
                }
                using (Catch("System.Exception exception", _isErrorHandlerExist))
                {
                    AppendLine("string errorMessage =\"Error updating desired properties\";", _isErrorHandlerExist);
                    AppendLine(_callErrorHandlerPattern, _isErrorHandlerExist);
                }
            }

            AppendLine();

            foreach (var reportedProperty in programReportedProperties)
            {
                var field = ((FieldDeclarationSyntax) reportedProperty.Key).Declaration.Variables.First();
                string fieldName = field.Identifier.ToString();
                var semanticModel = _generatorExecutionContext.Compilation.GetSemanticModel(field.SyntaxTree);
                var clrTypeName = ((IFieldSymbol) semanticModel.GetDeclaredSymbol(field))?.Type.Name;
                var typeName = Util.GetFriendlyNameOfPrimitive(clrTypeName);
                
                var twinPropertyAttribute =
                    reportedProperty.Value.First(v => v.Name.ToString() == nameof(ReportedAttribute).AttName());
                var localPropertyName = twinPropertyAttribute.ArgumentList?.Arguments[0].Expression.ToString()
                    .TrimStart('\"').TrimEnd('\"');

                var twinPropertyName = twinPropertyAttribute.ArgumentList != null &&
                                       twinPropertyAttribute.ArgumentList.Arguments.Count > 1
                    ? twinPropertyAttribute.ArgumentList?.Arguments[1].Expression.ToString().TrimStart('\"')
                        .TrimEnd('\"')
                    : localPropertyName;

                AppendLine($"public {typeName} {localPropertyName}");
                using (Block())
                {
                    AppendLine($"get {{ return {fieldName}; }}");
                    AppendLine($"set {{ {fieldName} = value; ReportProperty(\"{twinPropertyName}\",value);}}");
                }
                AppendLine();
            }
        }
    }
}