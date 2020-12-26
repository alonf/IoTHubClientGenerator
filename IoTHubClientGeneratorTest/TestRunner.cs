using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using IoTHubClientGenerator;
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
            string output = GetGeneratedOutput(source);
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

            var compilation = CSharpCompilation.Create("testAssembly", new[] {syntaxTree}, references,
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
