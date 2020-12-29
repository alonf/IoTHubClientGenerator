### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')
## IoTHubDeviceStatusChangesHandlerAttribute Class
decorate a function with a prototype such as: private async Task StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)  
It will be called whenever there is a change in the connectivity state of the device client  
```csharp
public class IoTHubDeviceStatusChangesHandlerAttribute : Attribute
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; IoTHubDeviceStatusChangesHandlerAttribute  
