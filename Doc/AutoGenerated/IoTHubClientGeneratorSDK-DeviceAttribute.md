### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')
## DeviceAttribute Class
Decorate a property that will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();  
If not provided, a default Device property is created  
```csharp
public class DeviceAttribute : Attribute
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; DeviceAttribute  
### Example
[Device(DeviceId = "%deviceId%", ConnectionString = "%connectionString%", TransportType = TransportType.Amqp)]  
public DeviceClient Client { get; set; }  
### Properties
- [AutoReconnect](./IoTHubClientGeneratorSDK-DeviceAttribute-AutoReconnect.md 'IoTHubClientGeneratorSDK.DeviceAttribute.AutoReconnect')
- [ConnectionString](./IoTHubClientGeneratorSDK-DeviceAttribute-ConnectionString.md 'IoTHubClientGeneratorSDK.DeviceAttribute.ConnectionString')
- [DeviceId](./IoTHubClientGeneratorSDK-DeviceAttribute-DeviceId.md 'IoTHubClientGeneratorSDK.DeviceAttribute.DeviceId')
- [GatewayHostname](./IoTHubClientGeneratorSDK-DeviceAttribute-GatewayHostname.md 'IoTHubClientGeneratorSDK.DeviceAttribute.GatewayHostname')
- [Hostname](./IoTHubClientGeneratorSDK-DeviceAttribute-Hostname.md 'IoTHubClientGeneratorSDK.DeviceAttribute.Hostname')
- [TransportType](./IoTHubClientGeneratorSDK-DeviceAttribute-TransportType.md 'IoTHubClientGeneratorSDK.DeviceAttribute.TransportType')
