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

            bool isValid = ValidateAttributeCount(nameof(DeviceAttribute), 0, 1);

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
            nameof(C2DeviceCallbackAttribute).AttName(),
            nameof(AlternateConnectionStringAttribute).AttName(),
            nameof(ConnectionStatusAttribute).AttName(),
            nameof(DesiredAttribute).AttName(),
            nameof(ReportedAttribute).AttName(),
            nameof(DeviceAttribute).AttName(),
            nameof(IoTHubDeviceStatusChangesHandlerAttribute).AttName(),
            nameof(ClientOptionsAttribute).AttName(),
            nameof(AuthenticationMethodAttribute).AttName(),
            nameof(DpsDeviceAttribute).AttName(),
            nameof(TransportSettingAttribute).AttName(),
            nameof(IoTHubErrorHandlerAttribute).AttName(),
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
