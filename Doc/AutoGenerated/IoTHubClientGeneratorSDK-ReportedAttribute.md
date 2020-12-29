### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')
## ReportedAttribute Class
Decorate a field to generate device twin reported attribute  
```csharp
public class ReportedAttribute : Attribute
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; ReportedAttribute  
### Example
[Reported("valueFromTheDevice")] private string _reportedPropertyDemo;  
[Reported("ReportedPropertyAutoNameDemo", "reportedPropertyAutoNameDemo")] private string _reportedPropertyAutoNameDemo;  
### Constructors
- [ReportedAttribute(string, string)](./IoTHubClientGeneratorSDK-ReportedAttribute-ReportedAttribute(string_string).md 'IoTHubClientGeneratorSDK.ReportedAttribute.ReportedAttribute(string, string)')
### Properties
- [LocalPropertyName](./IoTHubClientGeneratorSDK-ReportedAttribute-LocalPropertyName.md 'IoTHubClientGeneratorSDK.ReportedAttribute.LocalPropertyName')
- [TwinPropertyName](./IoTHubClientGeneratorSDK-ReportedAttribute-TwinPropertyName.md 'IoTHubClientGeneratorSDK.ReportedAttribute.TwinPropertyName')
