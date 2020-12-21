﻿using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateReportedProperties()
        {
            //todo: Add error handling if exist
            AppendLine("private void ReportProperty(string propertyName, string data)");
            AppendLine("{");
            using (Indent(this))
            {
                AppendLine("var reportedProperties = new Microsoft.Azure.Devices.Shared.TwinCollection();");
                AppendLine("reportedProperties[propertyName] = data;");
                AppendLine($"{_deviceClientPropertyName}.UpdateReportedPropertiesAsync(reportedProperties).Wait();");
            }

            AppendLine("}");
            AppendLine();

            var programReportedProperties = GetAttributedMembers(nameof(ReportedAttribute)).ToArray();

            foreach (var reportedProperty in programReportedProperties)
            {
                string fieldName = ((FieldDeclarationSyntax) reportedProperty.Key).Declaration.Variables.First()
                    .Identifier.ToString();
                var twinPropertyAttribute =
                    reportedProperty.Value.First(v => v.Name.ToString() == nameof(ReportedAttribute).AttName());
                var localPropertyName = twinPropertyAttribute.ArgumentList?.Arguments[0].Expression.ToString()
                    .TrimStart('\"').TrimEnd('\"');
                
                var twinPropertyName = twinPropertyAttribute.ArgumentList != null && twinPropertyAttribute.ArgumentList.Arguments.Count > 1 
                    ? twinPropertyAttribute.ArgumentList?.Arguments[1].Expression.ToString().TrimStart('\"').TrimEnd('\"') : localPropertyName;

                AppendLine($"public string {localPropertyName}");
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