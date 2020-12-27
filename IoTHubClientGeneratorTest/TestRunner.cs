using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using IoTHubClientGenerator;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Provisioning.Security;
using Microsoft.Azure.Devices.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace IoTHubClientGeneratorTest
{
    [UseReporter(typeof(DiffReporter))]
    public class TestRunner
    {
        private readonly ITestOutputHelper _output;

        public TestRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [MemberData(nameof(GetTests), parameters: 2)]
        public void TestCase(string testName, string source)
        {
            string output;
            try
            {
                output = GetGeneratedOutput(source);
            }
            catch (Exception e)
            {
                output = e.ToString();
            }
            
            using (ApprovalResults.ForScenario(testName))
            {
                ApprovalTests.Approvals.Verify(output);
            }
        }

        public static IEnumerable<object[]> GetTests(int numTests)
        {
            var result = 
                from type in Assembly.GetAssembly(typeof(TestRunner))!.GetTypes()
                from property in type.GetProperties(
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                let testCaseAttribute = property.GetCustomAttribute<TestCase>()
                where testCaseAttribute != null
                select new[] {testCaseAttribute.TestName, property.GetValue(null)}.Take(numTests).ToArray();
            return result;
        }
        
        //make sure the needed types are referenced
        [IoTHub]
        // ReSharper disable once UnusedType.Local
        private class Dummy
        {
#pragma warning disable 169
            private DeviceClient _dc;
#pragma warning restore 169

            public static void Foo()
            {
                // ReSharper disable once CA1806
                // ReSharper disable once ObjectCreationAsStatement
                new SecurityProviderX509Certificate(null);
                ProvisioningDeviceClient.Create(null, null, null, null);
            }
        }

        private string GetGeneratedOutput(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            references.Add(MetadataReference.CreateFromFile(typeof(IoTHubAttribute).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(DeviceClient).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ProvisioningDeviceClient).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(SecurityProviderX509Certificate).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ProvisioningTransportHandlerAmqp).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ProvisioningTransportHandlerMqtt).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(ProvisioningTransportHandlerHttp).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(HMACSHA256).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(SecurityProviderTpmHsm).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(X509Certificate2).Assembly.Location));
            
            var compilation = CSharpCompilation.Create("testAssembly", new[] {syntaxTree}, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


            var compileDiagnostics = compilation.GetDiagnostics();
            Assert.False(compileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                "Failed: " + compileDiagnostics.FirstOrDefault()?.GetMessage());

            ISourceGenerator generator = new Generator();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation,
                out var generateDiagnostics);
            Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                "Failed: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

            string output = outputCompilation.SyntaxTrees
                .Aggregate(new StringBuilder(),
                    (sb, st) =>
                    {
                        sb.AppendLine(new string('*', 80));
                        sb.AppendLine(st.ToString());
                        sb.AppendLine();
                        sb.AppendLine();
                        return sb;
                    }, sb => sb.ToString());

            _output.WriteLine(output);
            var allClasses = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            var theIoTHubDecoratedClass = allClasses.FirstOrDefault(c =>
                Enumerable.Any<AttributeListSyntax>(c.AttributeLists,
                    a => a.Attributes.Any(a => a.Name.ToString() + "Attribute" == nameof(IoTHubAttribute))));
            if (theIoTHubDecoratedClass != null)
            {
                var iotClassName = theIoTHubDecoratedClass.Identifier.ToString();

                var @namespace = ((NamespaceDeclarationSyntax) theIoTHubDecoratedClass.Parent)?.Name.ToString() ?? "";
                var main =
                    @$"
using System;
using System.Threading.Tasks;

namespace {@namespace}
{{
    public class Console
    {{
        static async Task Main()
        {{
            var iotHubClient = new {iotClassName}();
            await iotHubClient.InitIoTHubClientAsync();
        }}
    }}
}}";
                var mainSyntaxTree = CSharpSyntaxTree.ParseText(main);

                var syntaxTreeList = new List<SyntaxTree>(outputCompilation.SyntaxTrees) {mainSyntaxTree};

                var consoleCompilation = CSharpCompilation.Create("testExecutable", syntaxTreeList.ToArray(),
                    references, new CSharpCompilationOptions(OutputKind.ConsoleApplication));
                var consoleCompileDiagnostics = consoleCompilation.GetDiagnostics();
                if (consoleCompileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    output = consoleCompileDiagnostics.Aggregate(new StringBuilder(),
                        (s, d) => s.AppendLine(d.ToString()), s => s.ToString()) + Environment.NewLine + output;
                }
            }
            return output;
        }
    }
}
