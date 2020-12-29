### [IoTHubClientGeneratorSDK](./IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK').[DesiredAttribute](./IoTHubClientGeneratorSDK-DesiredAttribute.md 'IoTHubClientGeneratorSDK.DesiredAttribute')
## DesiredAttribute(string) Constructor
Decorate a property to be automatically updated from the device twin properties  
```csharp
public DesiredAttribute(string twinPropertyName="");
```
#### Parameters
<a name='IoTHubClientGeneratorSDK-DesiredAttribute-DesiredAttribute(string)-twinPropertyName'></a>
`twinPropertyName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')  
The name of the cloud device property. It will be derived from the property name, if not provided.  
  
