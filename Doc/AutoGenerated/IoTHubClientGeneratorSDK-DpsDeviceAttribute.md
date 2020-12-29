### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')
## DpsDeviceAttribute Class
Base class for DPS based device creation  
It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();  
```csharp
public abstract class DpsDeviceAttribute : Attribute
```
Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; DpsDeviceAttribute  

Derived  
&#8627; [DpsSymmetricKeyDeviceAttribute](./IoTHubClientGeneratorSDK-DpsSymmetricKeyDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsSymmetricKeyDeviceAttribute')  
&#8627; [DpsTpmDeviceAttribute](./IoTHubClientGeneratorSDK-DpsTpmDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsTpmDeviceAttribute')  
&#8627; [DpsX509CertificateDeviceAttribute](./IoTHubClientGeneratorSDK-DpsX509CertificateDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute')  
### Properties
- [DPSIdScope](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-DPSIdScope.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.DPSIdScope')
- [DPSTransportType](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-DPSTransportType.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.DPSTransportType')
- [EnrollmentGroupId](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-EnrollmentGroupId.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.EnrollmentGroupId')
- [EnrollmentType](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-EnrollmentType.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.EnrollmentType')
- [GlobalDeviceEndpoint](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-GlobalDeviceEndpoint.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.GlobalDeviceEndpoint')
- [Id](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-Id.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.Id')
- [OptionalDeviceId](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-OptionalDeviceId.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.OptionalDeviceId')
- [TransportType](./IoTHubClientGeneratorSDK-DpsDeviceAttribute-TransportType.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute.TransportType')
