### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## DeviceAttribute Class

Decorate a property that will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();  
If not provided, a default Device property is created

```csharp
public class DeviceAttribute : System.Attribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; DeviceAttribute

### Example
[Device(DeviceId = "%deviceId%", ConnectionString = "%connectionString%", TransportType = TransportType.Amqp)]  
public DeviceClient Client { get; set; }

| Properties | |
| :--- | :--- |
| [AutoReconnect](IoTHubClientGeneratorSDK.DeviceAttribute.AutoReconnect.md 'IoTHubClientGeneratorSDK.DeviceAttribute.AutoReconnect') | Try to reconnect if the connection dropped |
| [ConnectionString](IoTHubClientGeneratorSDK.DeviceAttribute.ConnectionString.md 'IoTHubClientGeneratorSDK.DeviceAttribute.ConnectionString') | Connection string for the IoT hub (with DeviceId) |
| [DeviceId](IoTHubClientGeneratorSDK.DeviceAttribute.DeviceId.md 'IoTHubClientGeneratorSDK.DeviceAttribute.DeviceId') | Device Identifier |
| [GatewayHostname](IoTHubClientGeneratorSDK.DeviceAttribute.GatewayHostname.md 'IoTHubClientGeneratorSDK.DeviceAttribute.GatewayHostname') | The fully-qualified DNS host name of Gateway |
| [Hostname](IoTHubClientGeneratorSDK.DeviceAttribute.Hostname.md 'IoTHubClientGeneratorSDK.DeviceAttribute.Hostname') | The fully-qualified DNS host name of IoT Hub |
| [TransportType](IoTHubClientGeneratorSDK.DeviceAttribute.TransportType.md 'IoTHubClientGeneratorSDK.DeviceAttribute.TransportType') | The default transport type (Http1, AMQP or MQTT) if not at least one TransportAttribute property exist |
