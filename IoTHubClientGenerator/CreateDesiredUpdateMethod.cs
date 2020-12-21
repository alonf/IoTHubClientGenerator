using System.Linq;
using System.Threading.Tasks;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateDesiredUpdateMethod()
        {
            AppendLine("private async Task HandleDesiredPropertyUpdateAsync(Microsoft.Azure.Devices.Shared.TwinCollection desiredProperties, object context)");
            AppendLine("{");
            using (Indent(this))
            {
                var programDesiredProperties = GetAttributedMembers(nameof(DesiredAttribute)).ToArray();
                
                foreach (var desiredProperty in programDesiredProperties)
                {
                    string propertyName = ((PropertyDeclarationSyntax) desiredProperty.Key).Identifier.ToString();
                    string desiredPropertyName = propertyName;

                    var twinPropertyAttribute = desiredProperty.Value.First(v => v.Name.ToString() == nameof(DesiredAttribute).AttName());

                    var twinPropertyNameProperty = twinPropertyAttribute.ArgumentList?.Arguments.FirstOrDefault();
                    
                    if (twinPropertyNameProperty != null)
                        desiredPropertyName = twinPropertyNameProperty.Expression.ToString().TrimStart('\"').TrimEnd('\"');
                    
                    AppendLine($"if (desiredProperties.Contains(\"{desiredPropertyName}\"))");
                    AppendLine("{");
                    using (Indent(this))
                    {
                        AppendLine($"{propertyName} = desiredProperties[\"{desiredPropertyName}\"];");
                    }
                    AppendLine("}");
                }
            }
            AppendLine("await Task.CompletedTask;");
            AppendLine("}");
            
        }
    }
}