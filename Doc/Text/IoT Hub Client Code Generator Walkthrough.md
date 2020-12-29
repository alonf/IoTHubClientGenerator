# IoT Hub Client C# Code Generator

Using the IoT Hub Client code generator is probably the fastest and easiest way to build a C# based Azure IoT device. I started this project as a demo for a lecture I delivered at the .NET conf 2020 local event in Israel, and I realized that this could be more than just a Demo. I decided to add more features and provide a complete solution for those who need to build a C# based IoT device, whether it is a real device or a simulation one.

This could be the first C# code generator project among many Azure client libraries!

The main benefit of using this library and tool is to save writing boilerplate code. Usually, when you start an IoT Hub client code, you copy an example and change it to your needs. Utilizing this library, you can focus only on your specific code to fulfill your specific tasks.

The easiest way to learn how to use the code generation tool is to try it. The following section demonstrates that. I encourage you to start Visual Studio or JetBrain Rider and ride with me.

1. Make sure you have the latest version of Visual Studio or JetBrain Rider.
1. Start a new project, choose a .NET Core console application. You should give it a name.

![Create new project](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/CreateNewProject.png?raw=true)

![Configure your new project](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/ConfigureYourProject.png?raw=true)

1. Once you have Visual Studio with the standard console app, right-click on the dependency tree item in the Solution Explorer, and choose &quot;Manage Nuget Packages…&quot;
1. Browse for AzureIoTHubClientGenerator and install it:

![Manage NuGet Packages...](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/ManageNugetPackages.png?raw=true)

1. Change the name of the class from Program to ```IoTDemo``` and decorate the class with the ```[IoTHub]``` attribute
  2. Add ```using IoTHubClientGeneratorSDK;```
  2. Make the ```IoTDemo``` class a ```partial``` class
1. Go to the Azure Portal, Create an IoT Hub and a Device and copy the primary connection string of that device
  1. For more information, read: [https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-device-management-get-started](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-device-management-get-started)
1. Right-click on the EasyIoTHubClient project in the Solution Explorer and choose &quot;Properties&quot;
  1. Go to the Debug section and add an environment variable:
    1. Name: ConnectionString
    1. Value: paste the primary key

![Edit environment variables](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/EditEnvironmentVariables.png?raw=true)

1. Delete the ```Console.WriteLine``` line


2. Add: ```using System.Threading.Tasks;```


3. Make Main, an ```async``` method:
```static async Task Main(string[] args)```

4. Add the following lines: to the ```IoTDemo``` class at the beginning:

```[Reported("BatteryLevel")]```

```private string _batteryLevel;```

1. Add to Main the following lines:

```
var iotDemo = new IoTDemo();
await iotDemo.InitIoTHubClientAsync();
iotDemo.BatteryLevel = "100%";
```

1. The program should look like this:

```
using System;
using System.Threading.Tasks;
using IoTHubClientGeneratorSDK;

namespace EasyIoTHubClient
{
    [IoTHub]
    partial class IoTDemo
    {
        [Reported("BatteryLevel")]
        private string _batteryLevel;

        static async Task Main(string[] args)
        {
            var iotDemo = new IoTDemo();
            await iotDemo.InitIoTHubClientAsync();
            iotDemo.BatteryLevel = "100%";
        }
    }
}
```

1. Let&#39;s try the program.
2. After executing the code, go to the Azure Portal and see the device twin. Can you see the Battery Level Property?

![Device Twin](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/DeviceTwinBatteryLevel.png?raw=true)


1. You start to feel how easy it is to use the source code generator. Let&#39;s add some more magic!
2. Add more code and have this program:

```
using System;
using System.Threading;
using System.Threading.Tasks;
using IoTHubClientGeneratorSDK;
namespace EasyIoTHubClient
{
    [IoTHub(GeneratedSendMethodName = "SendAsync")]
    partialclassIoTDemo
    {
        [Reported("BatteryLevel")]
        privatestring _batteryLevel;

        [Desired]
        private int ReportingFrequencyInHz { get; set; } = 1;

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
                BatteryLevel = $"{i%100}%";
                await SendAsync($"{{\"data\":\"{i}\"", i.ToString(), new CancellationToken());

                await Task.Delay(TimeSpan.FromMilliseconds(1000.0 / ReportingFrequencyInHz));
             }
        }
    }
}
```
1. The ```[IoTHub]``` attribute has now a parameter: ```[IoTHub(GeneratedSendMethodName = "SendAsync")]``` The ```GenerateSendMethodName``` instructs the code generator to create a send function us.
2. The ```[Desired] private int ReportingFrequencyInHz { get; set; } = 1;``` is a property of type ```int``` that gets the value from the IoT Hub device twin.
3. ```SendDataAsync``` sets the ```BatteryLevel``` property with a value that is reflected in the device twin in the cloud. The method also sends JSON information to the telemetry channel of the IoT Hub. The cloud controls the sending frequency by setting the ```ReportingFrequencyInHz``` desired property
4. Run the code. You can use the Portal or Azure IoT Explorer to see the results: [https://docs.microsoft.com/en-us/azure/iot-pnp/howto-use-iot-explorer](https://docs.microsoft.com/en-us/azure/iot-pnp/howto-use-iot-explorer)

![Telemetry](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/Telemetry.png?raw=true)

1. Add the ```ReportingFrequencyInHz``` property to the device twin's desired section and set the value to be 10 while the program is running. Change to a higher number and see that the telemetry is coming at a much faster rate.
2. Add the following code:

```
[C2DMessage(AutoComplete = true)]
private void Cloud2DeviceMessage(Message receivedMessage)
{
    string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
    Console.WriteLine($"Received message: [{messageData}]");
}
```

1. Compile and run. Send a command to the device, you can use the Azure IoT Explorer:

![Cloud to device message](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/Cloud2DeviceMessage.png?raw=true)

The result:

![Cloud to device message result](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/Cloud2DeviceMessageResult.png?raw=true)

1. Add the following method:

```
[DirectMethod]
private Task<MethodResponse> EchoAsync(MethodRequest request)
{
    var response = new MethodResponse(request.Data, 200);
    return Task.FromResult(response);
}
```
1. Use the IoT Explorer to call this method:

![Direct Method](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/DirectMethod.png?raw=true)

1. Add the following lines of code:

```
[ConnectionStatus]
private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }

[IoTHubDeviceStatusChangesHandler]
private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
{
    Console.WriteLine($"Connection status changed: status={status}, reason={reason}");
    Console.WriteLine($"Connection status property: status={DeviceConnectionStatus.Status}, reason={DeviceConnectionStatus.Reason}");
}
```

1. Change the ```SendDataAsync``` code to close the connection after some iterations:

```
private async Task SendDataAsync()
{
    for (int i = 1000; I <= 0; --i)
    {
        if (i == 990)
            DeviceClient.Dispose();

        BatteryLevel = $"{i % 100}%";

        await SendAsync($"{{\"data\":\"{i}\"", i.ToString(), new CancellationToken());

        await Task.Delay(TimeSpan.FromMilliseconds(1000.0 / ReportingFrequencyInHz));
    }
}
```
1. Run and see the result:

![Status method and property](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/Status%20method%20and%20property.png?raw=true)

1. To handle the errors, add these lines of code:

```
[IoTHubErrorHandler]
void IoTHubErrorHandler(string errorMessage, Exception exception)
{
    Console.WriteLine($"{errorMessage}, Exception: {exception.Message}");
}
```

1. Run the program

![Handling errors](https://github.com/alonf/IoTHubClientGenerator/blob/master/Doc/Images/Handling%20Errors.png?raw=true)

1. This concludes the basics of the IoT Hub C# Device Client generation. You might want to explore advanced features, such as the ability to define the connection strategy to the IoT Hub using the ```[Device]``` or ```[Dps*]``` attributes.