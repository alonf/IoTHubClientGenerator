using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace IoTHubClientGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var iotHubAttributeName = nameof(IoTHubAttribute).AttName();
            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;
            var iotHubSyntaxNodes = receiver.CandidateMembers
                .Where(m => m.Value.Any(att => att.Name.ToString() == iotHubAttributeName))
                .Select(t => t.Key).ToArray();

            if (!ValidateIoTHubAttribute(context, iotHubSyntaxNodes)) 
                return;
            
            foreach (var iotHubSyntaxNode in iotHubSyntaxNodes)
            {
                var iotHubNodeSemanticModel = context.Compilation.GetSemanticModel(iotHubSyntaxNode.SyntaxTree);
                var iotHubSymbol = iotHubNodeSemanticModel.GetDeclaredSymbol(iotHubSyntaxNode);
                
                //filter attribute members by the IoTHub class
                var candidateMembers = receiver.CandidateMembers
                    .Where(e => e.Key == iotHubSyntaxNode || e.Key.Parent == iotHubSyntaxNode).ToDictionary(e=>e.Key, e=>e.Value);
                var candidateAttributes = receiver.CandidateAttributes
                    .Where(e => e.Value == iotHubSyntaxNode || e.Value.Parent == iotHubSyntaxNode).ToDictionary(e=>e.Key, e=>e.Value);

                if (!Validate(context, candidateAttributes)) 
                    return;
                
                AddIoTHubGeneratedClass(context, iotHubSymbol, candidateMembers, candidateAttributes);
            }
        }

         private static bool ValidateIoTHubAttribute(GeneratorExecutionContext context, SyntaxNode[] iotHubSyntaxNodes)
        {
            if (iotHubSyntaxNodes.Length == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(new
                        DiagnosticDescriptor("IoTGen001", "IoT Hub Generator Error",
                            "At least one class should be decorated with [IoTHub]", "Warning", DiagnosticSeverity.Warning, true),
                    Location.None));
                return false;
            }
            return true;
        }
         
        private static bool Validate(GeneratorExecutionContext context, Dictionary<AttributeSyntax, SyntaxNode> receiverCandidateAttributes)
        {
            bool ValidateAttributeCount(string attributeName, int min, int max, string codeElement = "property")
            {
                
                var attributesNodes = receiverCandidateAttributes
                    .Where(a => a.Key.Name.ToString() == attributeName.AttName()).Select(e => e.Value)
                    .ToArray();
                var attributeCount = attributesNodes.Length;
                
                if (attributeCount < min)
                {
                    context.ReportDiagnostic(Diagnostic.Create(new
                            DiagnosticDescriptor("IoTGen002", "IoT Hub Generator Error",
                                $"At least {min} {codeElement} should be decorated with [{attributeName.AttName()}]", "Error", DiagnosticSeverity.Error, true),
                        Location.None));
                    return false;
                }
                    
                if (attributeCount > max)
                {
                    foreach (var syntaxNode in attributesNodes)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(new
                                DiagnosticDescriptor("IoTGen003", "IoT Hub Generator Error",
                                    $"No more then {max} {codeElement} should be decorated with [{attributeName.AttName()}], however, you may have more than one [IoTHub] decorated class", "Error",
                                    DiagnosticSeverity.Error, true),
                            Location.Create(syntaxNode.SyntaxTree, syntaxNode.Span)));
                    }
                    return false;
                }
                return true;
            }
            
            //validate the existence of attribute's properties 
            bool ValidateAttributeProperties(string attributeName, params string[] properties)
            {
                bool result = true;
                var attributesNodes = receiverCandidateAttributes
                    .Where(a => a.Key.Name.ToString() == attributeName.AttName()).Select(e => e.Key).ToArray();

                foreach (var attributesNode in attributesNodes)
                {
                    var missingProperties = properties.Where(p =>
                            attributesNode.ArgumentList != null &&
                            attributesNode.ArgumentList.Arguments.All(a => a.NameEquals?.ToString().TrimEnd('=') != p))
                        .ToArray();

                    if (missingProperties.Length != 0)
                    {
                        var missingParamsText = String.Join(" ", missingProperties);
                        context.ReportDiagnostic(Diagnostic.Create(new
                                DiagnosticDescriptor("IoTGen008", "IoT Hub Generator Error",
                                    $@"[{attributeName.AttName()}] must define these missing properties: {missingParamsText}",
                                    "Error",
                                    DiagnosticSeverity.Error, true),
                            Location.Create(attributesNode.SyntaxTree, attributesNode.Span)));
                        result = false;
                    }
                }
                return result;
            }

            //check if the device client is decorated with [Device] or one of the [DPS*] attributes
            var deviceAttributesNodes = receiverCandidateAttributes
                .Where(a => a.Key.Name.ToString().StartsWith("Dps") ||  a.Key.Name.ToString() == nameof(DeviceAttribute).AttName())
                .Select(e => e.Key)
                .ToArray();

            bool isValid = true;

            if (deviceAttributesNodes.Length > 1)
            {
                foreach (var syntaxNode in deviceAttributesNodes)
                {
                    context.ReportDiagnostic(Diagnostic.Create(new
                            DiagnosticDescriptor("IoTGen009", "IoT Hub Generator Error",
                                "No more then one [Device] or [Dps*] attributes are allowed per [IoTHub] class, however, you may have more than one [IoTHub] decorated class",
                                "Error",
                                DiagnosticSeverity.Error, true),
                        Location.Create(syntaxNode.SyntaxTree, syntaxNode.Span)));
                }
                isValid = false;
            }
            else if (deviceAttributesNodes.Length > 0 &&  deviceAttributesNodes.First().Name.ToString().StartsWith("Dps")) //[Dps*] attribute
            {
                var properties = new List<string>
                {
                    nameof(DpsDeviceAttribute.DPSIdScope),
                    nameof(DpsDeviceAttribute.Id),
                    nameof(DpsDeviceAttribute.EnrollmentGroupId),
                    nameof(DpsDeviceAttribute.EnrollmentType),
                    nameof(DpsDeviceAttribute.DPSTransportType),
                };
                var attributeName = deviceAttributesNodes.First().Name.ToString();
                switch (attributeName)
                {
                    case { } attName when attName == nameof(DpsSymmetricKeyDeviceAttribute).AttName():
                        properties.Add(nameof(DpsSymmetricKeyDeviceAttribute.PrimarySymmetricKey));
                        break;
                    case { } attName when attName == nameof(DpsX509CertificateDeviceAttribute).AttName():
                        properties.Add(nameof(DpsX509CertificateDeviceAttribute.CertificatePath));
                        properties.Add(nameof(DpsX509CertificateDeviceAttribute.CertificatePassword));
                        break;
                }
                if (receiverCandidateAttributes.Any(att=>att.Key.Name.ToString() == nameof(TransportSettingAttribute).AttName()) == false) //o TransportSettingAttribute
                    properties.Add(nameof(DpsX509CertificateDeviceAttribute.TransportType));
                
                if (ValidateAttributeProperties(deviceAttributesNodes.First().Name + "Attribute", properties.ToArray()) == false)
                    isValid = false;
            }

            if (ValidateAttributeCount(nameof(ClientOptionsAttribute), 0, 1) == false)
                isValid = false;
            
            if (ValidateAttributeCount(nameof(AuthenticationMethodAttribute), 0, 1) == false)
                isValid = false;
            
            if (ValidateAttributeCount(nameof(ConnectionStatusAttribute), 0, 1) == false)
                isValid = false;
            
            if (ValidateAttributeCount(nameof(C2DMessageAttribute), 0, 1, "method") == false)
                isValid = false;
            
            if (ValidateAttributeCount(nameof(IoTHubErrorHandlerAttribute), 0, 1, "method") == false)
                isValid = false;
            
            //todo: add validation for DPS attributes, only one of the DPS or Device should be define
            return isValid;
        }

        private static void AddIoTHubGeneratedClass(GeneratorExecutionContext context, ISymbol iotHubSymbol,
            Dictionary<SyntaxNode, AttributeSyntax[]> receiverCandidateMembers,
            Dictionary<AttributeSyntax, SyntaxNode> receiverCandidateAttributes)
        {
            AddSource($"{iotHubSymbol.Name}Extension.IoTHub.g.cs",
                IoTHubPartialClassBuilder.Build(context, iotHubSymbol as INamedTypeSymbol, receiverCandidateMembers, receiverCandidateAttributes));

            void AddSource(string sourceFileName, string sourceCode)
            {
                var sourceText = CSharpSyntaxTree
                    .ParseText(SourceText.From(sourceCode, Encoding.UTF8))
                    .GetRoot().NormalizeWhitespace()
                    .GetText(Encoding.UTF8);
                context.AddSource(sourceFileName, sourceText);
            }
        }
    }

    internal class SyntaxReceiver : ISyntaxReceiver
    {
        private static readonly string[] AttributeNames =
        {
            nameof(IoTHubAttribute).AttName(),
            nameof(C2DMessageAttribute).AttName(),
            nameof(DirectMethodAttribute).AttName(),
            //nameof(AlternateConnectionStringAttribute).AttName(),
            nameof(ConnectionStatusAttribute).AttName(),
            nameof(DesiredAttribute).AttName(),
            nameof(ReportedAttribute).AttName(),
            nameof(DeviceAttribute).AttName(),
            nameof(IoTHubDeviceStatusChangesHandlerAttribute).AttName(),
            nameof(ClientOptionsAttribute).AttName(),
            nameof(AuthenticationMethodAttribute).AttName(),
            nameof(TransportSettingAttribute).AttName(),
            nameof(IoTHubErrorHandlerAttribute).AttName(),
            nameof(DpsX509CertificateDeviceAttribute).AttName(),
            nameof(DpsSymmetricKeyDeviceAttribute).AttName(),
            nameof(DpsTpmDeviceAttribute).AttName()
        };

        public Dictionary<SyntaxNode, AttributeSyntax[]> CandidateMembers { get; } = new();

        public Dictionary<AttributeSyntax, SyntaxNode> CandidateAttributes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // if (!Debugger.IsAttached)
            // {
            //     Debugger.Launch();
            // }
            if (syntaxNode is not MemberDeclarationSyntax memberDeclarationSyntax) return;
            
            var iotAttributes = memberDeclarationSyntax.AttributeLists
                .SelectMany(attributeList => attributeList.Attributes, (_, attribute) => attribute)
                .Where(a => AttributeNames.Contains(a.Name.ToString()))
                .Select(a => a).ToArray();

            if (iotAttributes.Length > 0)
            {
                CandidateMembers[syntaxNode] = iotAttributes;

                foreach (var attributeSyntax in iotAttributes)
                {
                    CandidateAttributes[attributeSyntax] = syntaxNode;
                }
            }
        }
    }
}
