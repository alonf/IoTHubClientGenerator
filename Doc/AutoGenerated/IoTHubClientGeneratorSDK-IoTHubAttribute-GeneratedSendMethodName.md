### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK').[IoTHubAttribute](./IoTHubClientGeneratorSDK-IoTHubAttribute.md 'IoTHubClientGeneratorSDK.IoTHubAttribute')
## IoTHubAttribute.GeneratedSendMethodName Property
if used, a send method with the prototype:  private async Task MethodNameAsync(string jsonPayload, string messageId, CancellationToken cancellationToken, IDictionary<string, string> properties = null) is created  
call this method to send the telemetry data to the IoT Hub  
```csharp
public string GeneratedSendMethodName { get; set; }
```
#### Property Value
[System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
