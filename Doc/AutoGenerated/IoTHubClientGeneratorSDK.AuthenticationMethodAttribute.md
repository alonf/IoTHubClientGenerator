### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## AuthenticationMethodAttribute Class

Provide an authentication method that is required when creating the device client  
The property should return an [Microsoft.Azure.Devices.Client.IAuthenticationMethod](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Azure.Devices.Client.IAuthenticationMethod 'Microsoft.Azure.Devices.Client.IAuthenticationMethod') instance

```csharp
public class AuthenticationMethodAttribute : System.Attribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; AuthenticationMethodAttribute

### Example
private IAuthenticationMethod _deviceAuthenticationWithRegistrySymmetricKey =  
     new DeviceAuthenticationWithRegistrySymmetricKey("deviceId", "key");  
[AuthenticationMethod]  
public IAuthenticationMethod DeviceAuthenticationWithRegistrySymmetricKey => _deviceAuthenticationWithRegistrySymmetricKey;