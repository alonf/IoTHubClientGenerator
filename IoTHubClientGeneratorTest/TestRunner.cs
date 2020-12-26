using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using IoTHubClientGenerator;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

            var compilation = CSharpCompilation.Create("testAssembly", new[] {syntaxTree}, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            
            var compileDiagnostics = compilation.GetDiagnostics();
            Assert.False(compileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "Failed: " + compileDiagnostics.FirstOrDefault()?.GetMessage());

            ISourceGenerator generator = new Generator();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation,
                out var generateDiagnostics);
            Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                "Failed: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

            string output = outputCompilation.SyntaxTrees
                .Aggregate(new StringBuilder(), 
                    (sb,st)=>
                    {
                        sb.AppendLine(new string('*', 80));
                        sb.AppendLine(st.ToString());
                        sb.AppendLine();
                        sb.AppendLine();
                        return sb;
                    }, sb=>sb.ToString());
            
            _output.WriteLine(output);

            return output;
        }
    }
}
