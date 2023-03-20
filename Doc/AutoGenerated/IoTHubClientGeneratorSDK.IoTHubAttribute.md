### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## IoTHubAttribute Class

Decorate a class to serves as an IoTHub management class  
At least one class must be decorated as IoTHub to activate the source code generation.  
There can be more then one IoTHub class in case we need to be connected with more than one Device Client

```csharp
public class IoTHubAttribute : System.Attribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; IoTHubAttribute

| Properties | |
| :--- | :--- |
| [GeneratedSendMethodName](IoTHubClientGeneratorSDK.IoTHubAttribute.GeneratedSendMethodName.md 'IoTHubClientGeneratorSDK.IoTHubAttribute.GeneratedSendMethodName') | if used, a send method with the prototype:  private async Task MethodNameAsync(string jsonPayload, string messageId, CancellationToken cancellationToken, IDictionary<string, string> properties = null) is created<br/>call this method to send the telemetry data to the IoT Hub |
