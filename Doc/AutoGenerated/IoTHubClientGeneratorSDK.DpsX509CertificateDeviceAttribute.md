### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## DpsX509CertificateDeviceAttribute Class

Decorate a device client property with DPS settings.  
It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();

```csharp
public class DpsX509CertificateDeviceAttribute : IoTHubClientGeneratorSDK.DpsDeviceAttribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; [DpsDeviceAttribute](IoTHubClientGeneratorSDK.DpsDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute') &#129106; DpsX509CertificateDeviceAttribute

| Properties | |
| :--- | :--- |
| [CertificatePassword](IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute.CertificatePassword.md 'IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute.CertificatePassword') | The password of the PFX certificate file |
| [CertificatePath](IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute.CertificatePath.md 'IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute.CertificatePath') | The PFX certificate to load for device provisioning authentication |
