### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## DpsTpmDeviceAttribute Class

Decorate a device client property with DPS settings.  
It will be set when the device client is initiated in: await iotHubInstance.InitIoTHubClientAsync();

```csharp
public class DpsTpmDeviceAttribute : IoTHubClientGeneratorSDK.DpsDeviceAttribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; [DpsDeviceAttribute](IoTHubClientGeneratorSDK.DpsDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute') &#129106; DpsTpmDeviceAttribute

| Properties | |
| :--- | :--- |
| [GetTpmEndorsementKey](IoTHubClientGeneratorSDK.DpsTpmDeviceAttribute.GetTpmEndorsementKey.md 'IoTHubClientGeneratorSDK.DpsTpmDeviceAttribute.GetTpmEndorsementKey') | Gets the TPM endorsement key. Use this option by itself to get the EK needed to create a DPS individual enrollment |
