# IoT Hub Client C# code generator.

Installation: use the [NuGet package](https://www.nuget.org/packages/IoTHubClientGenerator/)  
To get started, follow this [walk-through](Doc/Text/Walkthrough.md)


This project takes advantage of the new C# 9.0 ability to have a code generation as part of the C# code compilation process. With this code generator, you can build an IoT Device client program in seconds. For example, the following code creates a device client that can send telemetry, receive commands, update twin reported property, get desired twin properties updates, get the current connection state, and handle direct method calls:

```
namespace EasyIoTHubClient
{
    [IoTHub(GeneratedSendMethodName = "SendAsync")]
    partial class IoTDemo
    {
        [Reported("BatteryLevel")] private string _batteryLevel;

        [Desired] private int ReportingFrequencyInHz { get; set; } = 1;

        static async Task Main(string[] args)
        {
            var iotDemo = new IoTDemo();
            await iotDemo.InitIoTHubClientAsync();
            iotDemo.BatteryLevel = "100%";
            await iotDemo.SendDataAsync();
        }

        private async Task SendDataAsync()
        {
            for (int i = 1000; i >= 0; --i)
            {
                BatteryLevel = $"{i % 100}%";
                await SendAsync($"{{\"data\":\"{i}\"", i.ToString(), new CancellationToken());
                await Task.Delay(TimeSpan.FromMilliseconds(1000.0 / ReportingFrequencyInHz));
            }
        }

        [C2DMessage(AutoComplete = true)]
        private void Cloud2DeviceMessage(Message receivedMessage)
        {
            string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
            Console.WriteLine($"Received message: [{messageData}]");
        }

        [DirectMethod]
        private Task<MethodResponse> EchoAsync(MethodRequest request)
        {
            var response = new MethodResponse(request.Data, 200);
            return Task.FromResult(response);
        }


        [ConnectionStatus]
        private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }
        
        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
            Console.WriteLine($"{errorMessage}, Exception: {exception.Message}");
        }
    }
}
```

## Advanced Features
You may use one of the ```[Device]``` or the ```[DPS*]``` attributes to decorate a ```DeviceClient``` property. With these attributes and their properties, we can manipulate the IoT Hub device client creation parameters.
Each property value can be set as a text or can be set as an environment variable value by wrapping the value with the `````%````` character, for example:
```
[DeviceClient(ConnectionString="%ConStr%)]
DeviceClient MyClient {get;set;}
```
The ```[Device]``` attribute has a long list of properties and a set of other attributes (```[ClientOptions]```, ```[TransportSetting]```, and ```[AuthenticationMethod]```)  that creates the parameter of the IoT device client Create method. The code generator chooses the correct overload version of the device client ```Create()``` function by collecting all these parameters and selecting the suitable function version. If there is a missing parameter or a collision between parameters, the code generator emits an error.
Example of non-trivial device creation:

```
    [Device(ConnectionString = "%conString%", DeviceId = "%deviceId%")]
    public DeviceClient DeviceClient { get; set; }

    [TransportSetting]
    public ITransportSettings AmqpTransportSettings { get; } = new AmqpTransportSettings(TransportType.Amqp)
    {
        AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings {MaxPoolSize = 5}, IdleTimeout = TimeSpan.FromMinutes(1)
    };

    [TransportSetting]
    public ITransportSettings MqttTransportSetting { get; } = new MqttTransportSettings(TransportType.Mqtt)
    {
        DefaultReceiveTimeout = TimeSpan.FromMinutes(2)
    };

    [ClientOptions]
    public ClientOptions ClientOptions { get; } = new();
```
The following code is the result of the example:
```
private Microsoft.Azure.Devices.Client.DeviceClient CreateDeviceClient()
{
    var theConnectionString = System.Environment.GetEnvironmentVariable("conString");
    var theDeviceId = System.Environment.GetEnvironmentVariable("deviceId");
    ITransportSettings[] transportSettings = new[]{AmqpTransportSettings, MqttTransportSetting};
    var deviceClient = DeviceClient.CreateFromConnectionString(theConnectionString, theDeviceId, transportSettings, ClientOptions);
    return deviceClient;
}
```
Or if you want to set the authentication method:
```
    [Device(Hostname = "%hostName%", TransportType = TransportType.Mqtt)]
    public DeviceClient DeviceClient { get; set; }

    [AuthenticationMethod]
    public IAuthenticationMethod DeviceAuthenticationWithRegistrySymmetricKey { get; } = new DeviceAuthenticationWithRegistrySymmetricKey("deviceId", "key");
```
And the result generated code is:
```
private Microsoft.Azure.Devices.Client.DeviceClient CreateDeviceClient()
{
    var theHostname = System.Environment.GetEnvironmentVariable("hostName");
    var theTransportType = TransportType.Mqtt;
    var deviceClient = DeviceClient.Create(theHostname, DeviceAuthenticationWithRegistrySymmetricKey, theTransportType);
    return deviceClient;
}
```
To use the Device Provisioning Service of Azure IoT, decorate the Device Client property with one of ```[DpsSymmetricKeyDevice]```, ```[DpsTpmDevice]```, ```[DpsX509CertificateDevice]```

Example:
```
        [DpsSymmetricKeyDevice(DPSIdScope="scope", DPSTransportType=TransportType.Mqtt, TransportType=TransportType.Mqtt,
            EnrollmentGroupId="%EnrollmentGroupId%", EnrollmentType=DPSEnrollmentType.Group, 
            Id="%RegistrationId%", PrimarySymmetricKey="%SymKey%")]
        public DeviceClient DeviceClient { get; set; }
```
To compile the code, we need to add the NuGet Packages: ```Microsoft.Azure.Devices.Provisioning.Client``` and ```Microsoft.Azure.Devices.Provisioning.Transport.Mqtt```

The resulting generated code:
```
    private async Task<Microsoft.Azure.Devices.Client.DeviceClient> CreateDeviceClientAsync()
    {
        var theDPSIdScope = "scope";
        var theDPSTransportType = TransportType.Mqtt;
        var theTransportType = TransportType.Mqtt;
        var theEnrollmentGroupId = System.Environment.GetEnvironmentVariable("EnrollmentGroupId");
        var theEnrollmentType = DPSEnrollmentType.Group;
        var theId = System.Environment.GetEnvironmentVariable("RegistrationId");
        var thePrimarySymmetricKey = System.Environment.GetEnvironmentVariable("SymKey");
        using var security = new SecurityProviderSymmetricKey(theId, thePrimarySymmetricKey, null);
        using var transport = new ProvisioningTransportHandlerMqtt();
        var theGlobalDeviceEndpoint = "global.azure-devices-provisioning.net";
        ProvisioningDeviceClient provClient = ProvisioningDeviceClient.Create(theGlobalDeviceEndpoint, theDPSIdScope, security, transport);
        DeviceRegistrationResult result = await provClient.RegisterAsync();
        if (result.Status != ProvisioningRegistrationStatusType.Assigned)
        {
            throw new Exception($"Registration status did not assign a hub, status: {result.Status}");
        }

        IAuthenticationMethod auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, security.GetPrimaryKey());
        var deviceClient = DeviceClient.Create(result.AssignedHub, auth, theTransportType);
        return deviceClient;
    }
```

## IoTHubClientGeneratorSDK Namespace
### Classes
- [AuthenticationMethodAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-AuthenticationMethodAttribute.md 'IoTHubClientGeneratorSDK.AuthenticationMethodAttribute')
- [C2DMessageAttribute](/Doc/AutoGenerated/IotHubClientGeneratorSDK/Doc/AutoGenerated/IoTHubClientGeneratorSDK-C2DMessageAttribute.md 'IoTHubClientGeneratorSDK.C2DMessageAttribute')
- [ClientOptionsAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-ClientOptionsAttribute.md 'IoTHubClientGeneratorSDK.ClientOptionsAttribute')
- [ConnectionStatusAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-ConnectionStatusAttribute.md 'IoTHubClientGeneratorSDK.ConnectionStatusAttribute')
- [DesiredAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DesiredAttribute.md 'IoTHubClientGeneratorSDK.DesiredAttribute')
- [DeviceAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DeviceAttribute.md 'IoTHubClientGeneratorSDK.DeviceAttribute')
- [DirectMethodAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DirectMethodAttribute.md 'IoTHubClientGeneratorSDK.DirectMethodAttribute')
- [DpsDeviceAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DpsDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsDeviceAttribute')
- [DpsSymmetricKeyDeviceAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DpsSymmetricKeyDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsSymmetricKeyDeviceAttribute')
- [DpsTpmDeviceAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DpsTpmDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsTpmDeviceAttribute')
- [DpsX509CertificateDeviceAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DpsX509CertificateDeviceAttribute.md 'IoTHubClientGeneratorSDK.DpsX509CertificateDeviceAttribute')
- [IoTHubAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-IoTHubAttribute.md 'IoTHubClientGeneratorSDK.IoTHubAttribute')
- [IoTHubDeviceStatusChangesHandlerAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-IoTHubDeviceStatusChangesHandlerAttribute.md 'IoTHubClientGeneratorSDK.IoTHubDeviceStatusChangesHandlerAttribute')
- [IoTHubErrorHandlerAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-IoTHubErrorHandlerAttribute.md 'IoTHubClientGeneratorSDK.IoTHubErrorHandlerAttribute')
- [ReportedAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-ReportedAttribute.md 'IoTHubClientGeneratorSDK.ReportedAttribute')
- [TransportSettingAttribute](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-TransportSettingAttribute.md 'IoTHubClientGeneratorSDK.TransportSettingAttribute')
### Enums
- [DPSEnrollmentType](/Doc/AutoGenerated/IoTHubClientGeneratorSDK-DPSEnrollmentType.md 'IoTHubClientGeneratorSDK.DPSEnrollmentType')



## Missing features and known issues:
- Move some of the implementation to rely on the semantic data and not the syntax tree, it will solve several issues such as spaces around =
- Have a separate error handler for each attribute instead (or as override rule) of one global handler
- Add the ability for the user to provide the ```MessageSchema``` ```ContentType``` and ```ContentEncoding``` to the send-telemetry method in compile and at runtime
- Have the ability to postponed update of reported properties, and only after setting a group of them, ask for a batch update of all
- Add the ability to provide the connection string at runtime, something like: ```[Device(ConnectionString={variableName})]```. On device creation, the value of the variable is used. This can be also relevant to all other places that has the ability to use ```%envName%```
- Add support for:
  - File Uploads
  - Device Modules
  - Device Streams  

