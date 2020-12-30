using System;
using System.Collections.Generic;
using System.Linq;
using IoTHubClientGeneratorSDK;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder : TextGenerator
    {
        private bool _isErrorHandlerExist;
        private string _callErrorHandlerPattern; //ErrorHandler(errorMessage, exception);
        private bool _isConnectionStatusExist;
        private string _connectionStatusAccessText;
        private string _deviceClientPropertyName;
        
        private readonly GeneratorExecutionContext _generatorExecutionContext;
        private readonly Dictionary<SyntaxNode, AttributeSyntax[]> _receiverCandidateMembers;
        private readonly Dictionary<AttributeSyntax, SyntaxNode> _receiverCandidateAttributes;
        
        private INamedTypeSymbol Class { get; }

        public IoTHubPartialClassBuilder(GeneratorExecutionContext generatorExecutionContext,
            INamedTypeSymbol classSymbol,
            Dictionary<SyntaxNode, AttributeSyntax[]> receiverCandidateMembers,
            Dictionary<AttributeSyntax, SyntaxNode> receiverCandidateAttributes)
        {
            _generatorExecutionContext = generatorExecutionContext;
            _receiverCandidateMembers = receiverCandidateMembers;
            _receiverCandidateAttributes = receiverCandidateAttributes;
            Class = classSymbol;
            var @namespace = Class.ContainingNamespace.ToDisplayString();
            try
            {
                BuildPartialClass(@namespace, Class.Name);
            }
            catch (Exception e)
            {
                AppendLine($"#error in IoTHub Client generator, error {e}");
            }

        }

        public static string Build(GeneratorExecutionContext generatorExecutionContext, INamedTypeSymbol classSymbol,
            Dictionary<SyntaxNode, AttributeSyntax[]> receiverCandidateMembers,
            Dictionary<AttributeSyntax, SyntaxNode> receiverCandidateAttributes)
        {
            return new IoTHubPartialClassBuilder(generatorExecutionContext, classSymbol, receiverCandidateMembers,
                receiverCandidateAttributes).Result;
        }

        private IEnumerable<KeyValuePair<AttributeSyntax, SyntaxNode>> GetAttributes(string attributeName) =>
            _receiverCandidateAttributes.Where(att =>
                att.Key.Name.ToString() == attributeName.AttName());
        
        private bool IsAttributeExist(params string [] attributeNames) =>
            _receiverCandidateAttributes.Any(att =>
                attributeNames.Any(n=>n.AttName() == att.Key.Name.ToString()));

        private IEnumerable<KeyValuePair<SyntaxNode, AttributeSyntax[]>> GetAttributedMembers(string attributeName) =>
            _receiverCandidateMembers.Where(att =>
                att.Value.Any(a=>a.Name.ToString() == attributeName.AttName()));
        
        private string GetAttributePropertyValue(AttributeSyntax attributeSyntax, string propertyName) =>
            attributeSyntax.ArgumentList!.Arguments.Where(a =>
                    a.NameEquals!.ToString().TrimEnd('=').Trim() == propertyName)
                .Select(a => a.Expression.ToString()).FirstOrDefault();

        private void BuildPartialClass(string namespaceName, string className)
        {
            CreateErrorHandlerMethodCallPattern();

            AppendLine("using System;");
            AppendLine("using System.Diagnostics;");
            AppendLine("using System.Threading.Tasks;");
            AppendLine("using System.Threading;");
            AppendLine("using Microsoft.Azure.Devices.Client;");
            AppendLine("using Microsoft.Azure.Devices.Shared;");
            if (IsAttributeExist(nameof(DpsX509CertificateDeviceAttribute)) ||
                IsAttributeExist(nameof(DpsSymmetricKeyDeviceAttribute)) ||
                IsAttributeExist(nameof(DpsTpmDeviceAttribute)))
            {
                AppendLine("using Microsoft.Azure.Devices.Provisioning.Client.Transport;");
                AppendLine("using System.Security.Cryptography;");
                AppendLine("using Microsoft.Azure.Devices.Provisioning.Client;");
            }

            if (IsAttributeExist(nameof(DpsX509CertificateDeviceAttribute)))
            {
                AppendLine("using System.Security.Cryptography.X509Certificates;");
            }

            AppendLine("using IoTHubClientGeneratorSDK;");
            AppendLine();
            AppendLine($"namespace {namespaceName}");
            using (Block())
            {
                CreateClass(className);
            }
        }

        private void CreateClass(string className)
        {
            AppendLine($"public partial class {className}");
            using (Block())
            {
                CreateDeviceClientInitialization();
                CreateReportedProperties();
                CreateSendMethod();
                if (IsAttributeExist(nameof(DesiredAttribute)))
                    CreateDesiredUpdateMethod();
            }
        }
    }
}