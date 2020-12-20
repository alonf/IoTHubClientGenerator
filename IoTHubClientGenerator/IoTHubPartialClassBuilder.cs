using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Serialization;

namespace IoTHubClientGenerator
{
    class IoTHubPartialClassBuilder
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private int _nestingLevel;
        private readonly GeneratorExecutionContext _generatorExecutionContext;
        private readonly Dictionary<SyntaxNode, AttributeSyntax[]> _receiverCandidateMembers;
        private readonly Dictionary<AttributeSyntax, SyntaxNode> _receiverCandidateAttributes;
        private INamedTypeSymbol Class { get; }

        private static IDisposable Indent(IoTHubPartialClassBuilder @this) => new IndentClass(@this);
        
        class IndentClass : IDisposable
        {
            private readonly IoTHubPartialClassBuilder _this;

            public IndentClass(IoTHubPartialClassBuilder @this)
            {
                _this = @this;
                ++_this._nestingLevel;
            }

            public void Dispose()
            {
                --_this._nestingLevel;
            }
        }

        private void AppendLine(string line = "")
        {
            _sb.Append('\t', _nestingLevel);
            _sb.AppendLine(line);
        }

        private void Append(string line = "", bool isIndented = false)
        {
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
            return new IoTHubPartialClassBuilder(generatorExecutionContext, classSymbol, receiverCandidateMembers, receiverCandidateAttributes).Result;
        }

        private string Result => _sb.ToString();

        private IEnumerable<KeyValuePair<AttributeSyntax, SyntaxNode>> GetAttributes(string attributeName) => _receiverCandidateAttributes.Where(a =>
            a.Key.Name.ToString() == attributeName.AttName());

        private void BuildPartialClass(string namespaceName, string className)
        {
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
                CreateConnectionStatusFunction();
                CreateReportedProperties();
                CreateDesiredProperties();
                CreateReportedCloudUpdateMethod();
                CreateDesiredUpdateMethod();
                CreateDirectMethodCallback();
                CreateMessageHandling();
            }
            AppendLine("}");
        }

        private void CreateDeviceClientInitialization()
        {
            var createDeviceClientMethods = new List<(string methodName, AttributeSyntax deviceAtributeSyntax)>();
            _sb.AppendLine("private void InitIoTHubClient()");
            _sb.AppendLine("{");
            using (Indent(this))
            {
                //get the Device attribute
                var deviceAttributes = GetAttributes(nameof(DeviceAttribute)).ToArray();
               
                if (!deviceAttributes.Any())
                {
                    _sb.AppendLine("#error Missing [Device] attributes on a DeviceClient property;");
                    return;
                }

                foreach (var deviceAttribute in deviceAttributes)
                {
                    var deviceProperty = ((PropertyDeclarationSyntax)(deviceAttribute.Value)).Identifier;
                    var createDeviceClientMethodName = $"Create{deviceProperty}";
                    _sb.AppendLine($"{deviceProperty} =  {createDeviceClientMethodName}();");
                    
                    createDeviceClientMethods.Add((createDeviceClientMethodName, deviceAttribute.Key));
                }
            }
            _sb.AppendLine("}");
            _sb.AppendLine();
            foreach (var deviceClientMethod in createDeviceClientMethods)
            {
                CreateDeviceClientMethod(deviceClientMethod.methodName, deviceClientMethod.deviceAtributeSyntax);
            }
        }

        private void CreateDeviceClientMethod(string methodName, AttributeSyntax attributeSyntax)
        {
            _sb.AppendLine($"private Microsoft.Azure.Devices.Client.DeviceClient {methodName}()");
            _sb.AppendLine("{");
            using (Indent(this))
            {
                var parameterNameList = new List<string>();
                if (attributeSyntax.ArgumentList != null)
                {
                    
                    foreach (var argument in attributeSyntax.ArgumentList.Arguments)
                    {
                        var attAssignment = $"the{argument.NameEquals}";
                        parameterNameList.Add(argument.NameEquals?.ToString().TrimEnd('=').Trim());
                        var attExpression = argument.Expression.ToString();
                        if (attExpression.StartsWith("\"%") && attExpression.EndsWith("%\""))
                        {
                            attExpression = $"System.Environment.GetEnvironmentVariable(\"{attExpression.TrimStart('%','"').TrimEnd('%','"')}\")";
                        }
                        _sb.AppendLine($"var {attAssignment}{attExpression};");
                    }
                }

                var createDeviceError = new StringBuilder();
                
                string clientOptionsPropertyName = null;
                var clientOptionAttribute = GetAttributes(nameof(ClientOptionsAttribute)).ToArray();
                if (clientOptionAttribute.Length > 0)
                {
                    clientOptionsPropertyName = ((PropertyDeclarationSyntax) clientOptionAttribute[0].Value).Identifier.ToString();
                }

                var transportSettingsAttributes = GetAttributes(nameof(TransportSettingAttribute)).ToArray();
                if (transportSettingsAttributes.Length > 0)
                {
                    AppendLine("ITransportSettings [] transportSettings = new [] {");
                    foreach (var transportSettingsAttribute in transportSettingsAttributes)
                    {
                        Append(((PropertyDeclarationSyntax)transportSettingsAttribute.Value).Identifier.ToString());
                        Append(", ");
                    }

                    TrimEnd(2);
                    AppendLine("};");
                }
                
                string authenticationMethoPropertyName = null;
                var authenticationMethodAttributes = GetAttributes(nameof(AuthenticationMethodAttribute)).ToArray();
                if (authenticationMethodAttributes.Length > 0)
                {
                    authenticationMethoPropertyName = ((PropertyDeclarationSyntax) authenticationMethodAttributes[0].Value).Identifier.ToString();
                }
                
                var creationFunctionEntry = new StringBuilder();
                if (parameterNameList.Contains(nameof(DeviceAttribute.ConnectionString)))
                {
                    createDeviceError.Append("ConnectionString ");
                    creationFunctionEntry.Append("cs_");
                }
                if (parameterNameList.Contains(nameof(DeviceAttribute.Hostname)))
                {
                    createDeviceError.Append("Hostname ");
                    creationFunctionEntry.Append("hn_");
                }
                if (parameterNameList.Contains(nameof(DeviceAttribute.GatewayHostname)))
                {
                    createDeviceError.Append("GatewayHostname ");
                    creationFunctionEntry.Append("gw_");
                }
                if (parameterNameList.Contains(nameof(DeviceAttribute.TransportType)))
                {
                    createDeviceError.Append("TransportType ");
                    creationFunctionEntry.Append("tt_");
                }
                if (parameterNameList.Contains(nameof(DeviceAttribute.DeviceId)))
                {
                    createDeviceError.Append("DeviceId ");
                    creationFunctionEntry.Append("did_");
                }
                if (transportSettingsAttributes.Length > 0)
                {
                    createDeviceError.Append("ITransportSettings[] ");
                    creationFunctionEntry.Append("ts_");
                }
                if (authenticationMethodAttributes.Length > 0) //0 or 1
                {
                    createDeviceError.Append("AuthenticationMethod ");
                    creationFunctionEntry.Append("am_");
                }
                if (clientOptionAttribute.Length > 0) //0 or 1
                {
                    createDeviceError.Append("ClientOptions ");
                    creationFunctionEntry.Append("co_");
                }

                if (creationFunctionEntry.Length == 0) //no paramters
                {
                    _generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(new
                            DiagnosticDescriptor("IoTGen004", "IoT Hub Generator Error",
                                $"Can't generate DeviceClient creation code, check the supplied [Device] parameters", "Error",
                                DiagnosticSeverity.Error, true),
                        Location.Create(attributeSyntax.SyntaxTree, attributeSyntax.Span)));
                    return;
                }
                AppendLine($"//{creationFunctionEntry}");
                Append("var deviceClient = ");

                creationFunctionEntry.Remove(creationFunctionEntry.Length - 1, 1); //remove the last _

                switch (creationFunctionEntry.ToString())
                {
                    case "cs":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)});");
                        break;
                    
                    case "cs_co":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, {clientOptionsPropertyName});");
                        break;
                    
                    case "cs_ts":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, transportSettings);");
                        break;
                    
                    case "cs_ts_co":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, transportSettings, {clientOptionsPropertyName});");
                        break;
                    
                    case "cs_tt":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.TransportType)});");
                        break;
                    
                    case "cs_tt_co":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.TransportType)}, {clientOptionsPropertyName});");
                        break;
                    
                    case "cs_did":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)});");
                        break;
                    
                    case "cs_did_co":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, {clientOptionsPropertyName});");
                        break;
                    
                    case "cs_tt_did":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, the{nameof(DeviceAttribute.TransportType)});");
                        break;
                    
                    case "cs_did_ts":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, transportSettings);");
                        break;
                    
                    case "cs_did_ts_co":
                        AppendLine($"DeviceClient.CreateFromConnectionString(the{nameof(DeviceAttribute.ConnectionString)}, the{nameof(DeviceAttribute.DeviceId)}, transportSettings, {clientOptionsPropertyName});");
                        break;
                    
                    case "hn_gw_tt_am":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethoPropertyName}, the{nameof(DeviceAttribute.TransportType)});");
                        break;
                    
                    case "hn_gw_tt_am_co":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethoPropertyName}, the{nameof(DeviceAttribute.TransportType)}, {clientOptionsPropertyName});");
                        break;

                    case "hn_gw_ts_am":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethoPropertyName}, transportSettings);");
                        break;
                    
                    case "hn_gw_ts_am_co":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethoPropertyName}, transportSettings, {clientOptionsPropertyName});");
                        break;
                    
                    case "hn_gw_am":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethoPropertyName});");
                        break;
                    
                    case "hn_gw_am_co":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, the{nameof(DeviceAttribute.GatewayHostname)}, {authenticationMethoPropertyName}, {clientOptionsPropertyName});");
                        break;
                    
                    case "hn_tt_am":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethoPropertyName}, the{nameof(DeviceAttribute.TransportType)});");
                        break;
                    
                    case "hn_tt_am_co":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethoPropertyName}, the{nameof(DeviceAttribute.TransportType)}, {clientOptionsPropertyName});");
                        break;

                    case "hn_ts_am":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethoPropertyName}, transportSettings);");
                        break;
                    
                    case "hn_ts_am_co":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethoPropertyName}, transportSettings, {clientOptionsPropertyName});");
                        break;
                    
                    case "hn_am":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethoPropertyName});");
                        break;
                    
                    case "hn_am_co":
                        AppendLine($"DeviceClient.Create(the{nameof(DeviceAttribute.Hostname)}, {authenticationMethoPropertyName}, {clientOptionsPropertyName});");
                        break;
                    
                    default:
                        _generatorExecutionContext.ReportDiagnostic(Diagnostic.Create(new
                                DiagnosticDescriptor("IoTGen005", "IoT Hub Generator Error",
                                    $"Can't generate DeviceClient creation code, no DeviceClient Create method takes the combination of {createDeviceError} parameters. check [Device] parameters and other attributes ([TransportSetting], [AuthenticationMethod], [ClientOptions]) ", "Error",
                                    DiagnosticSeverity.Error, true),
                            Location.Create(attributeSyntax.SyntaxTree, attributeSyntax.Span)));
                        AppendLine(" null;");
                        break;
                }
                    
                _sb.AppendLine("return deviceClient;");
            }
            _sb.AppendLine("}");
        }

        private void CreateConnectionStatusFunction()
        {
            
        }
        
        private void CreateDesiredUpdateMethod()
        {
            
        }

        private void CreateDesiredProperties()
        {
        }

        private void CreateReportedCloudUpdateMethod()
        {
            
        }

        private void CreateReportedProperties()
        {
            
        }


        

        private void CreateDirectMethodCallback()
        {
        }

        private void CreateMessageHandling()
        {
        }

        /*

        AppendLine("private static void OutputText(string text)");
                AppendLine("{");
                using (new Indent(this))
                {
                    AppendLine("Console.WriteLine($\" ==>{text}<==\");");
                }

                AppendLine("}");
                AppendLine();
                AppendLine("public void Stop() {_cts.Cancel(); }");
                AppendLine();
                if (!_programBuilderParameters.UseBenchmarkDotnet)
                {
                    AppendLine("public void Start()");
                    AppendLine("{");
                    using (new Indent(this))
                    {
                        AppendLine("var stopwatch = new System.Diagnostics.Stopwatch();");
                        AppendLine("stopwatch.Start();");
                        AppendLine(
                            $"var numberOfCalls = Start({_programBuilderParameters.DurationInSeconds}, {_programBuilderParameters.NumberOfExceptionPerSecond});");
                        AppendLine("OutputText(\"Test results:\");");
                        AppendLine("OutputText($\"Total number of Calls: {numberOfCalls:N}\");");
                        AppendLine("stopwatch.Stop();");
                        AppendLine("OutputText($\"Total number of exceptions: {_numberOfExceptions}\");");
                        AppendLine();
                        AppendLine("OutputText($\"Test duration: {stopwatch.Elapsed.TotalSeconds} sec\");");
                    }

                    AppendLine("}");
                    AppendLine();
                }
                else
                {
                    AppendLine("[Benchmark]");
                    AppendLine(
                        $"[Arguments({_programBuilderParameters.DurationInSeconds}, {_programBuilderParameters.NumberOfExceptionPerSecond})]");
                }

                AppendLine("public long Start(int duration, double exceptionRate)");
                AppendLine("{");
                using (new Indent(this))
                {
                    AppendLine("long totalNumberOfCalls = 0;");
                    AppendLine("try");
                    AppendLine("{");
                    using (new Indent(this))
                    {
                        AppendLine("_exceptionRate = exceptionRate;");
                        AppendLine("var stopThread = new Thread(() =>");
                        AppendLine("{");
                        using (new Indent(this))
                        {
                            AppendLine("Thread.Sleep(duration * 1000);");
                            AppendLine("Stop();");
                        }

                        AppendLine("}");
                        AppendLine(") {Priority = ThreadPriority.Highest};");
                        AppendLine("stopThread.Start();");
                        AppendLine();
                        AppendLine("ParallelOptions po = new ParallelOptions();");
                        AppendLine("po.CancellationToken = _cts.Token;");
                        AppendLine("_clock.Start();");
                        AppendLine("_rateFormClock.Start();");
                        AppendLine("while(true)");
                        AppendLine("{");
                        using (new Indent(this))
                        {
                            AppendLine("Parallel.Invoke(po, ");
                            using (new Indent(this))
                            {
                                for (int i = 0; i < _programBuilderParameters.ConcurrentLevel; ++i)
                                {
                                    AppendLine("()=>");
                                    AppendLine("{");
                                    using (new Indent(this))
                                    {
                                        AppendLine("long numberOfCalls = 0;");
                                        AppendLine("try");
                                        AppendLine("{");
                                        using (new Indent(this))
                                        {
                                            AppendLine("Run(ref numberOfCalls);");
                                        }
                                        AppendLine("}");
                                        AppendLine("catch(Exception ex)");
                                        AppendLine("{");
                                        using (new Indent(this))
                                        {
                                            AppendLine("_ozCodeLogger.LogError(ex, \"error...\");");
                                        }
                                        AppendLine("}");
                                        AppendLine("finally");
                                        AppendLine("{");
                                        using (new Indent(this))
                                        {
                                            AppendLine("Interlocked.Add(ref totalNumberOfCalls, numberOfCalls);");
                                        }
                                        AppendLine("}");
                                    }
                                    AppendLine(i < _programBuilderParameters.ConcurrentLevel - 1 ? "}," : "}");
                                }
                                AppendLine(");");
                            }

                            AppendLine("if (_cts.IsCancellationRequested)");
                            AppendLine("{");
                            using (new Indent(this))
                            {
                                AppendLine("break;");
                            }
                            AppendLine("}");
                        }
                        AppendLine("}");
                    }

                    AppendLine("}");
                    AppendLine("catch");
                    AppendLine("{");
                    AppendLine("}");
                }

                AppendLine("return totalNumberOfCalls;");
                AppendLine("}");
                AppendLine();
                CreateMethods();
                AppendLine();
            }

        

        AppendLine("}");
            AppendLine();
        }
        _sb.Append($@"
                using System;

                namespace {namespaceName} {{
                    public partial class {className} {{
                        {AppendClassMembers()}
                    }}
                }}
            ");
        }

        public void AppendClassMembers()
        {
        }
        */
    }

    //public class IoTHubPartialClassBuilder
    //{
    //    private readonly Dictionary<SyntaxNode, AttributeSyntax[]> _receiverCandidateMembers;
    //    private INamedTypeSymbol Class { get; }
    //    private ImmutableList<IPropertySymbol> Properties { get; }

    //    protected IoTHubPartialClassBuilder(INamedTypeSymbol classSymbol,
    //        Dictionary<SyntaxNode, AttributeSyntax[]> receiverCandidateMembers)
    //    {
    //        _receiverCandidateMembers = receiverCandidateMembers;
            
    //        //Class = classSymbol;
    //        //Properties = Class.GetMembers()
    //        //    .OfType<IPropertySymbol>()
    //        //    .Where(x => !x.IsStatic && x.SetMethod != null)
    //        //    .ToImmutableList();
    //        //Value = BuildValue();
    //    }

    //    //public string Value { get; set; }

        //public static string Build(INamedTypeSymbol classSymbol,
        //    Dictionary<SyntaxNode, AttributeSyntax[]> receiverCandidateMembers)
        //{
        //    return new IoTHubPartialClassBuilder(classSymbol, receiverCandidateMembers).Value;
        //}

        //private string BuildValue()
        //{
        //    var @namespace = Class.ContainingNamespace.ToDisplayString();
        //    var showAll = BuildShowAll();

        //    return $@"
        //        using System;

        //        namespace {@namespace} {{
        //            public partial class {Class.Name} {{
        //                {showAll}
        //            }}
        //        }}
        //    ";
        //}

        //public string BuildShowAll()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var receiverCandidateMember in _receiverCandidateMembers)
        //    {
        //        sb.AppendLine($"Console.WriteLine(\"{receiverCandidateMember.Key.GetFirstToken().Value}:\");");
        //        foreach (AttributeSyntax attributeSyntax in receiverCandidateMember.Value)
        //        {
        //            var argumentsText = attributeSyntax.ArgumentList?.Arguments.Aggregate(new StringBuilder(),
        //                (s, v) => sb.Append($"{v.NameColon}:{v.Expression}  "), s => s.ToString());
        //            sb.AppendLine($"Console.WriteLine(\"{attributeSyntax.Name}: {argumentsText}\");");
        //        }

        //        sb.AppendLine();
        //    }
        //    return $@"
        //        public static ShowAll()
        //        {{
        //            {sb}
        //        }}
        //    ";
        //}

        //private string BuildConstructorPropertyAssignments()
        //{
        //    return string.Join(Environment.NewLine, Properties
        //        .Select(property => $"{property.Name} = other.{property.Name};"));
        //}
    //}
}