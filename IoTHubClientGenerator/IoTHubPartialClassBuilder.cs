using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _nestingLevel;
        private bool _isErrorHandlerExist = false;
        private string _callErrorHandlerPattern; //ErrorHandler(errorMessage, exception);
        private bool _isConnectionStatusExist = false;
        private string _connectionStatusAccessText;
        private string _deviceClientPropertyName;
        
        private readonly GeneratorExecutionContext _generatorExecutionContext;
        private readonly Dictionary<SyntaxNode, AttributeSyntax[]> _receiverCandidateMembers;
        private readonly Dictionary<AttributeSyntax, SyntaxNode> _receiverCandidateAttributes;
        
        private INamedTypeSymbol Class { get; }

        private static IDisposable Indent(IoTHubPartialClassBuilder @this, bool condition = true) => new IndentClass(@this, condition);

        class IndentClass : IDisposable
        {
            private readonly IoTHubPartialClassBuilder _this;
            private readonly bool _condition;

            public IndentClass(IoTHubPartialClassBuilder @this, bool condition = true)
            {
                _this = @this;
                _condition = condition;
                if (condition)
                    ++_this._nestingLevel;
            }

            public void Dispose()
            {
                if (_condition)
                    --_this._nestingLevel;
            }
        }

        private void AppendLine(string line = "", bool condition = true)
        {
            if (!condition)
                return;
            
            _sb.Append('\t', _nestingLevel);
            _sb.AppendLine(line);
        }

        private void Append(string line = "", bool isIndented = false, bool condition = true)
        {
            if (!condition)
                return;

            if (isIndented)
                _sb.Append('\t', _nestingLevel);
            _sb.Append(line);
        }

        private void TrimEnd(int n = 1)
        {
            _sb.Remove(_sb.Length - n, n);
        }

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

        private string Result => _sb.ToString();

        private IEnumerable<KeyValuePair<AttributeSyntax, SyntaxNode>> GetAttributes(string attributeName) =>
            _receiverCandidateAttributes.Where(a =>
                a.Key.Name.ToString() == attributeName.AttName());
        
        private IEnumerable<KeyValuePair<SyntaxNode, AttributeSyntax[]>> GetAttributedMembers(string attributeName) =>
            _receiverCandidateMembers.Where(a =>
                a.Value.Any(a=>a.Name.ToString() == attributeName.AttName()));

        private void BuildPartialClass(string namespaceName, string className)
        {
            CreateErrorHandlerMethoCallPattern(); 
            
            AppendLine("using System;");
            AppendLine("using System.Diagnostics;");
            AppendLine("using System.Threading.Tasks;");
            AppendLine("using System.Threading;");
            AppendLine("using Microsoft.Azure.Devices.Client;");
            AppendLine("using IoTHubClientGeneratorSDK;");
            AppendLine();
            AppendLine($"namespace {namespaceName}");
            AppendLine("{");
            using (Indent(this))
            {
                CreateClass(className);
            }

            AppendLine("}");
        }

        private void CreateClass(string className)
        {
            AppendLine($"public partial class {className}");
            AppendLine("{");
            using (Indent(this))
            {
                CreateDeviceClientInitialization();
                CreateReportedProperties();
                CreateSendMethod();
                CreateDesiredUpdateMethod();
                CreateDirectMethodCallback();
            }

            AppendLine("}");
        }
    }
}