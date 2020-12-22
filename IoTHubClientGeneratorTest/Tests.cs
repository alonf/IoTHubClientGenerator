using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using IoTHubClientGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace IoTHubClientGeneratorTest
{
    [UseReporter(typeof(DiffReporter))]
    public class Tests
    {
        private readonly ITestOutputHelper _output;

        public Tests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void TestSimplestCase()
        {
            string source = @"
namespace Foo
{
    [IoTHub]
    class MyIoTHubClient
    {
        
    }
}";
            string output = GetGeneratedOutput(source);
            ApprovalTests.Approvals.Verify(output);
        }
        

        [Theory]
        [MemberData(nameof(GetTests), parameters: 2)]
        public void TestCase(string testName, string source)
        {
            string output = GetGeneratedOutput(source);
            using (ApprovalResults.ForScenario(testName))
            {
                ApprovalTests.Approvals.Verify(output);
            }
        }
        
        public static IEnumerable<object[]> GetTests(int numTests)
        {
            var result =
                from field in typeof(Tests).GetFields(
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                let testCaseAttribute = field.GetCustomAttribute<TestCase>()
                where testCaseAttribute != null
                select new object[] {testCaseAttribute.TestName, field.GetValue(null)}.Take(numTests).ToArray();

            return result;
        }

#pragma warning disable 414
        [TestCase("TestC2DeviceMessage")]
        private static string testC2DeviceMessage = @"
namespace Foo
{
    [IoTHub(GeneratedSendMethodName = ""SendTelemetryAsync\"")]
    class MyIoTHubClient
    {
        
    }
}";
        
        [TestCase("TestReportedProperties")]
        private static string testReportedProperties = @"
namespace Foo
{
    [IoTHub()]
    class MyIoTHubClient
    {
        [Reported(""valueFromTheDevice"")] private string _reportedPropertyDemo;

        [Reported(""ReportedPropertyAutoNameDemo"", ""reportedPropertyAutoNameDemo"")] private string _reportedPropertyAutoNameDemo;
    }
}";
    
#pragma warning restore 414       
       

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

            var compilation = CSharpCompilation.Create("foo", new SyntaxTree[] {syntaxTree}, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            // TODO: Uncomment this line if you want to fail tests when the injected program isn't valid _before_ running generators
            // var compileDiagnostics = compilation.GetDiagnostics();
            // Assert.False(compileDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error), "Failed: " + compileDiagnostics.FirstOrDefault()?.GetMessage());

            ISourceGenerator generator = new Generator();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation,
                out var generateDiagnostics);
            Assert.False(generateDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error),
                "Failed: " + generateDiagnostics.FirstOrDefault()?.GetMessage());

            string output = outputCompilation.SyntaxTrees.Last().ToString();

            _output.WriteLine(output);

            return output;
        }
    }
}
