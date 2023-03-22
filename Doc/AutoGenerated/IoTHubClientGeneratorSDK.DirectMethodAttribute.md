### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## DirectMethodAttribute Class

register a method with a prototype such as: private async Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)

```csharp
public class DirectMethodAttribute : System.Attribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; DirectMethodAttribute

| Properties | |
| :--- | :--- |
| [CloudMethodName](IoTHubClientGeneratorSDK.DirectMethodAttribute.CloudMethodName.md 'IoTHubClientGeneratorSDK.DirectMethodAttribute.CloudMethodName') | The name that the cloud uses to call this method<br/>If not set, the name is the decorated C# method name |
