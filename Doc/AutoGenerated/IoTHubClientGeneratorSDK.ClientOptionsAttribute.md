### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK')

## ClientOptionsAttribute Class

Provide a client option instance that is used when creating the device client  
The property should return an [Microsoft.Azure.Devices.Client.ClientOptions](https://docs.microsoft.com/en-us/dotnet/api/Microsoft.Azure.Devices.Client.ClientOptions 'Microsoft.Azure.Devices.Client.ClientOptions') instance

```csharp
public class ClientOptionsAttribute : System.Attribute
```

Inheritance [System.Object](https://docs.microsoft.com/en-us/dotnet/api/System.Object 'System.Object') &#129106; [System.Attribute](https://docs.microsoft.com/en-us/dotnet/api/System.Attribute 'System.Attribute') &#129106; ClientOptionsAttribute

### Example
private ClientOptions _clientOptions = new ClientOptions() {FileUploadTransportSettings = new Http1TransportSettings(){Proxy = new System.Net.WebProxy("https://myproxy")}};