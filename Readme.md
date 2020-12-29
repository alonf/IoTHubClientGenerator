# IoT Hub Client C# code generator.

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

To get started, follow this [walk-through](Doc/Text/Walkthrough.md)


## The code generation Attributes and Features:

### ```[Device]```
