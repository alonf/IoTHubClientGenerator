using Microsoft.CodeAnalysis;

namespace IoTHubClientGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var source = "class Foo { }";

            if (source != null)
            {
                context.AddSource("IoTHubSupportGenerated.cs", source);
            }
        }
    }
}