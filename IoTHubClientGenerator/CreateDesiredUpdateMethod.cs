using System.Threading.Tasks;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void CreateDesiredUpdateMethod()
        {
            AppendLine("private async Task HandleDesiredPropertyUpdateAsync(Microsoft.Azure.Devices.Shared.TwinCollection desiredProperties, object context)");
            AppendLine("{");
            using (Indent(this))
            {
                AppendLine("await Task.Yield();");
            }
            AppendLine("");
            AppendLine("}");
        }
    }
}