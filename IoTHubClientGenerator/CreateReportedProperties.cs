using System.Linq;
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
            AppendLine("{");
            using (Indent(this, _isErrorHandlerExist))
            {
                AppendLine("try", _isErrorHandlerExist);
                AppendLine("{", _isErrorHandlerExist);
                using (Indent(this))
                {
                    AppendLine("var reportedProperties = new Microsoft.Azure.Devices.Shared.TwinCollection();");
                    AppendLine("reportedProperties[propertyName] = data.ToString();");
                    AppendLine(
                        $"{_deviceClientPropertyName}.UpdateReportedPropertiesAsync(reportedProperties).Wait();");
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
            AppendLine("}");
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
                AppendLine("{");
                using (Indent(this))
                {
                    AppendLine($"get {{ return {fieldName}; }}");
                    AppendLine($"set {{ {fieldName} = value; ReportProperty(\"{twinPropertyName}\",value);}}");
                }

                AppendLine("}");
                AppendLine();
            }
        }
    }
}