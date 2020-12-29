### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')
## DesiredAttribute Class
Decorate a property to be automatically updated from the device twin properties  
```csharp
public class DesiredAttribute : Attribute
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; DesiredAttribute  
### Example
[Desired("valueFromTheCloud")] private string DesiredPropertyDemo { get; set; }  
[Desired] private string DesiredPropertyAutoNameDemo { get; set; }  
### Constructors
- [DesiredAttribute(string)](./IoTHubClientGeneratorSDK-DesiredAttribute-DesiredAttribute(string).md 'IoTHubClientGeneratorSDK.DesiredAttribute.DesiredAttribute(string)')
### Properties
- [TwinPropertyName](./IoTHubClientGeneratorSDK-DesiredAttribute-TwinPropertyName.md 'IoTHubClientGeneratorSDK.DesiredAttribute.TwinPropertyName')
