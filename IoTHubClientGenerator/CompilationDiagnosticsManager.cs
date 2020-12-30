using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace IoTHubClientGenerator
{
    static class Diagnostics
    {
        public static IDictionary<DiagnosticId, DiagnosticCase> Cases = new Dictionary<DiagnosticId, DiagnosticCase>
        {
            {
                DiagnosticId.MissingIoTHubAttribute,
                new DiagnosticCase(DiagnosticId.MissingIoTHubAttribute, "IoT Hub Device Client Generator", DiagnosticsCategory.IoTHub,
                    "At least one class should be decorated with [IoTHub]", DiagnosticSeverity.Warning)
            },
            {
                DiagnosticId.MinElementsShouldBeDecorated,
                new DiagnosticCase(DiagnosticId.MinElementsShouldBeDecorated, "Missing Attributes", DiagnosticsCategory.Attributes,
                    "At least {0} {1} should be decorated with [{2}]", DiagnosticSeverity.Error)
            },
            {
                DiagnosticId.MaxElementsShouldBeDecorated,
                new DiagnosticCase(DiagnosticId.MaxElementsShouldBeDecorated, "Too Many Attributes", DiagnosticsCategory.Attributes,
                    "No more then {0} {1} should be decorated with [{2}], however, you may have more than one [IoTHub] decorated class", DiagnosticSeverity.Error)
            },
            {
                DiagnosticId.DeviceParametersError,
                new DiagnosticCase(DiagnosticId.DeviceParametersError, "Device attribute parameters error", DiagnosticsCategory.Attributes,
                    "Can't generate DeviceClient creation code, check the supplied [Device] parameters", DiagnosticSeverity.Error)
            },
            {
                DiagnosticId.ParametersMismatch,
                new DiagnosticCase(DiagnosticId.ParametersMismatch, "Device attribute parameters error", DiagnosticsCategory.Attributes,
                    "Can't generate DeviceClient creation code, check the supplied [Device] parameters", DiagnosticSeverity.Error)
            },
            {
                DiagnosticId.MissingDeviceAttribute,
                new DiagnosticCase(DiagnosticId.MissingDeviceAttribute, "Missing Device Attribute", DiagnosticsCategory.Attributes,
                    "Generating default DeviceClient property with a default ConnectionString environment variable since no [Device] or one of the [DPS*] can be found.", DiagnosticSeverity.Warning)
            },
            {
                DiagnosticId.PropertiesMustExist,
                new DiagnosticCase(DiagnosticId.PropertiesMustExist, "Missing Properties", DiagnosticsCategory.Parameters,
                    "[{0}] must define these missing properties: {1}", DiagnosticSeverity.Error)
            },
            {
                DiagnosticId.DpsAttributeMissingProperties,
                new DiagnosticCase(DiagnosticId.DpsAttributeMissingProperties, "Missing Properties for DPS attribute", DiagnosticsCategory.Parameters,
                    "Dps attribute must define properties", DiagnosticSeverity.Error)
            },
            {
                DiagnosticId.SingleDeviceOrDPSAttribute,
                new DiagnosticCase(DiagnosticId.SingleDeviceOrDPSAttribute, "Too many [DPS*] or [Device] exist", DiagnosticsCategory.Attributes,
                    "No more then one [Device] or [Dps*] attributes are allowed per [IoTHub] class, however, you may have more than one [IoTHub] decorated class", DiagnosticSeverity.Error)
            }

        };
    }

    public enum DiagnosticId
    {
        MissingIoTHubAttribute = 1,
        MinElementsShouldBeDecorated = 2,
        MaxElementsShouldBeDecorated = 3,
        DeviceParametersError = 4,
        ParametersMismatch = 5,
        MissingDeviceAttribute = 6,
        DpsAttributeMissingProperties = 7,
        PropertiesMustExist = 8,
        SingleDeviceOrDPSAttribute = 9,
        
        
    }

    public enum DiagnosticsCategory
    {
        Parameters,
        IoTHub,
        Attributes
    }

    public class DiagnosticCase
    {
        private readonly DiagnosticId _diagnosticId;
        private readonly string _title;
        private readonly DiagnosticsCategory _category;
        private readonly string _template;
        private readonly DiagnosticSeverity _severity;

        public DiagnosticCase(DiagnosticId diagnosticId, string title, DiagnosticsCategory category, string template, DiagnosticSeverity severity)
        {
            _diagnosticId = diagnosticId;
            _title = title;
            _category = category;
            _template = template;
            _severity = severity;
        }

        private string Id => $"IoTGen{(int)_diagnosticId:D5}";

        public DiagnosticDescriptor Create(params object[] messageParameters)
        {
            return new DiagnosticDescriptor(Id, _title, string.Format(_template, messageParameters), _category.ToString(), _severity, true, null, null);
        }
    }
    
    class CompilationDiagnosticsManager
    {
        private readonly GeneratorExecutionContext _context;
        
        public CompilationDiagnosticsManager(GeneratorExecutionContext context)
        {
            _context = context;
        }

        public void Report(DiagnosticId diagnosticsId, Location location, params string [] messageParameters)
        {
            var diagnosticsCase = Diagnostics.Cases[diagnosticsId];
            
            _context.ReportDiagnostic(Diagnostic.Create(diagnosticsCase.Create(messageParameters), location));
        }
    }
}