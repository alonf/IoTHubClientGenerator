### [IoTHubClientGeneratorSDK](IoTHubClientGeneratorSDK.md 'IoTHubClientGeneratorSDK').[ReportedAttribute](IoTHubClientGeneratorSDK.ReportedAttribute.md 'IoTHubClientGeneratorSDK.ReportedAttribute')

## ReportedAttribute(string, string) Constructor

Decorate a field to generate device twin reported attribute

```csharp
public ReportedAttribute(string localPropertyName, string twinPropertyName="");
```
#### Parameters

<a name='IoTHubClientGeneratorSDK.ReportedAttribute.ReportedAttribute(string,string).localPropertyName'></a>

`localPropertyName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The local name of the generated proxy

<a name='IoTHubClientGeneratorSDK.ReportedAttribute.ReportedAttribute(string,string).twinPropertyName'></a>

`twinPropertyName` [System.String](https://docs.microsoft.com/en-us/dotnet/api/System.String 'System.String')

The cloud twin property name. If not provided, the twin property name is the same as the local property name