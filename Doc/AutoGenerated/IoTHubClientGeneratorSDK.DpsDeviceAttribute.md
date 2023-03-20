### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## DpsDeviceAttribute Class

Base class for DPS based device creation  
It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();

```csharp
public abstract class DpsDeviceAttribute : System.Attribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; DpsDeviceAttribute

Derived  
&#8627; [DpsSymmetricKeyDeviceAttribute](IoTHubClientGeneratorSDK.DpsSymmetricKeyDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsSymmetricKeyDeviceAttribute')  
&#8627; [DpsTpmDeviceAttribute](IoTHubClientGeneratorSDK.DpsTpmDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsTpmDeviceAttribute')  
&#8627; [DpsX509CertificateDeviceAttribute](IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute')

| Properties | |
| :--- | :--- |
| [DPSIdScope](IoTHubClientGeneratorSDK.DpsDeviceAttribute.DPSIdScope.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.DPSIdScope') | The Id Scope of the DPS instance |
| [DPSTransportType](IoTHubClientGeneratorSDK.DpsDeviceAttribute.DPSTransportType.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.DPSTransportType') | The transport to use to communicate with the device provisioning instance. Possible values include Mqtt, Mqtt_WebSocket_Only, Mqtt_Tcp_Only, Amqp, Amqp_WebSocket_Only, Amqp_Tcp_only, and Http1 |
| [EnrollmentType](IoTHubClientGeneratorSDK.DpsDeviceAttribute.EnrollmentType.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.EnrollmentType') | The type of enrollment: Individual or Group |
| [GlobalDeviceEndpoint](IoTHubClientGeneratorSDK.DpsDeviceAttribute.GlobalDeviceEndpoint.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.GlobalDeviceEndpoint') | The global endpoint for devices to connect to |
| [Id](IoTHubClientGeneratorSDK.DpsDeviceAttribute.Id.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.Id') | TThe registration Id when using individual enrollment, or the desired device Id when using group enrollment |
| [OptionalDeviceId](IoTHubClientGeneratorSDK.DpsDeviceAttribute.OptionalDeviceId.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.OptionalDeviceId') | An optional device identifier |
| [TransportType](IoTHubClientGeneratorSDK.DpsDeviceAttribute.TransportType.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.TransportType') | The transport to use to communicate with the IoTHub. Possible values include Mqtt, Mqtt_WebSocket_Only, Mqtt_Tcp_Only, Amqp, Amqp_WebSocket_Only, Amqp_Tcp_only, and Http1 |
