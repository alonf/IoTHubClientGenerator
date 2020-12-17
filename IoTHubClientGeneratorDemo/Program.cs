using System;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Newtonsoft.Json;

namespace IoTHubClientGeneratorDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await IoTHubClientManager.RunAsync();
            IoTHubClient iotHubClient = new IoTHubClient();
            await iotHubClient.RunSampleAsync(TimeSpan.FromMinutes(5));
        }
    }


    [IoTHub]
    public class IoTHubClient
    {
        private static readonly Random RandomGenerator = new Random();
        private const int TemperatureThreshold = 30;
        private static readonly TimeSpan SleepDuration = TimeSpan.FromSeconds(5);

        [Device(Hostname = "%hostname%", GatewayHostname = "%GatewayHostname%", AutoReconnect = true)]
        public DeviceClient DeviceClient { get; set; }

        private readonly ClientProperties _clientProperties = new ClientProperties();

        [ClientProperties]
        public ClientProperties ClientProperties => _clientProperties;

        [Desired("valueFromTheCloud")] 
        public string DesiredPropertyDemo { get; set; }

        [Desired] 
        public string DesiredPropertyNameDemo { get; set; }

        [Reported("valueFromTheDevice")] 
        public string ReportedPropertyDemo { get; set; }

        [Reported] 
        public string ReportedPropertyNameDemo { get; set; }

        [ConnectionStatus] 
        public (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }

        [C2DMessage(AutoComplete = true)]
        public void OnC2dMessageReceived(Message receivedMessage, object context)
        {
            Console.WriteLine(
                $"{DateTime.Now}> C2D message callback - message received with Id={receivedMessage.MessageId}.");

            //do something with the message


        }

        //[C2DMessage(AutoComplete = false)]
        public async Task OnC2dMessageReceived2(Message receivedMessage, object userContext)
        {
            Console.WriteLine(
                $"{DateTime.Now}> C2D message callback - message received with Id={receivedMessage.MessageId}.");

            //do something with the message

            await DeviceClient.CompleteAsync(receivedMessage);
            Console.WriteLine($"{DateTime.Now}> Completed C2D message with Id={receivedMessage.MessageId}.");

            receivedMessage.Dispose();

        }

        [C2DeviceCallback]
        public Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)
        {
            Console.WriteLine($"\t *** {methodRequest.Name} was called.");
            Console.WriteLine($"\t{methodRequest.DataAsJson}\n");

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        private async Task SendMessagesAsync(CancellationToken cancellationToken)
        {
            int messageCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                if (DeviceConnectionStatus.Status == ConnectionStatus.Connected)
                {
                    Console.WriteLine($"Device sending message {++messageCount} to IoT Hub...");

                    (Message message, string payload) = PrepareMessage(messageCount);
                    while (true)
                    {
                        try
                        {
                            await DeviceClient.SendEventAsync(message, cancellationToken);
                            Console.WriteLine($"Sent message {messageCount} of {payload}");
                            message.Dispose();
                            break;
                        }
                        catch (IotHubException ex) when (ex.IsTransient)
                        {
                            // Inspect the exception to figure out if operation should be retried, or if user-input is required.
                            Console.WriteLine(
                                $"An IotHubException was caught, but will try to recover and retry: {ex}");
                        }
                        catch (Exception ex) when (ExceptionHelper.IsNetworkExceptionChain(ex))
                        {
                            Console.WriteLine(
                                $"A network related exception was caught, but will try to recover and retry: {ex}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Unexpected error {ex}");
                        }

                        // wait and retry
                        await Task.Delay(SleepDuration, cancellationToken);
                    }
                }

                await Task.Delay(SleepDuration, cancellationToken);
            }
        }

        private (Message, string) PrepareMessage(int messageId)
        {
            var temperature = RandomGenerator.Next(20, 35);
            var humidity = RandomGenerator.Next(60, 80);
            string messagePayload = $"{{\"temperature\":{temperature},\"humidity\":{humidity}}}";

            var eventMessage = new Message(Encoding.UTF8.GetBytes(messagePayload))
            {
                MessageId = messageId.ToString(),
                ContentEncoding = Encoding.UTF8.ToString(),
                ContentType = "application/json",
            };
            eventMessage.Properties.Add("temperatureAlert", (temperature > TemperatureThreshold) ? "true" : "false");

            return (eventMessage, messagePayload);
        }

        public async Task RunSampleAsync(TimeSpan sampleRunningTime)
        {
            var cts = new CancellationTokenSource(sampleRunningTime);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Sample execution cancellation requested; will exit.");
            };

            Console.WriteLine("Sample execution started, press Control+C to quit the sample.");

            try
            {
                await SendMessagesAsync(cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unrecoverable exception caught, user action is required, so exiting...: \n{ex}");
            }
        }


        [StatusChangesHandler]
        public async Task StatusChangesHandler()
        {
            Console.WriteLine($"Connection status changed: status={DeviceConnectionStatus.Status}, reason={DeviceConnectionStatus.Reason}");


            switch (DeviceConnectionStatus.Status)
            {
                case ConnectionStatus.Connected:
                    Console.WriteLine(
                        "### The DeviceClient is CONNECTED; all operations will be carried out as normal.");
                    break;

                case ConnectionStatus.Disconnected_Retrying:
                    Console.WriteLine(
                        "### The DeviceClient is retrying based on the retry policy. Do NOT close or open the DeviceClient instance");
                    break;

                case ConnectionStatus.Disabled:
                    Console.WriteLine("### The DeviceClient has been closed gracefully." +
                                      "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");
                    break;

                case ConnectionStatus.Disconnected:
                    switch (DeviceConnectionStatus.Reason)
                    {
                        case ConnectionStatusChangeReason.Bad_Credential:
                            // When getting this reason, the current connection string being used is not valid.
                            // If we had a backup, we can try using that.
                            Console.WriteLine("The current connection string is invalid. Trying another.");
                            _clientProperties.AlternateConnectionString = "..."; //set alternate connection string
                            await IoTHubClientManager.ReconnectAsync();
                            break;

                        case ConnectionStatusChangeReason.Device_Disabled:
                            Console.WriteLine(
                                "### The device has been deleted or marked as disabled (on your hub instance)." +
                                "\nFix the device status in Azure and then create a new device client instance.");
                            break;

                        case ConnectionStatusChangeReason.Retry_Expired:
                            Console.WriteLine(
                                "### The DeviceClient has been disconnected because the retry policy expired." +
                                "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");

                            await IoTHubClientManager.ReconnectAsync();
                            break;

                        case ConnectionStatusChangeReason.Communication_Error:
                            Console.WriteLine(
                                "### The DeviceClient has been disconnected due to a non-retry-able exception. Inspect the exception for details." +
                                "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");

                            await IoTHubClientManager.ReconnectAsync();
                            break;

                        default:
                            Console.WriteLine(
                                "### This combination of ConnectionStatus and ConnectionStatusChangeReason is not expected, contact the client library team with logs.");
                            break;

                    }

                    break;

                default:
                    Console.WriteLine(
                        "### This combination of ConnectionStatus and ConnectionStatusChangeReason is not expected, contact the client library team with logs.");
                    break;
            }
        }
    }
}
