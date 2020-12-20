using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IoTHubClientGeneratorSDK;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Client.Transport.Mqtt;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IoTHubClientGeneratorDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            await IoTHubClientManager.RunAsync();
            IoTHubClient iotHubClient = new IoTHubClient();
            IoTHubClientAuto iotHubClientAuto = new IoTHubClientAuto();
           // iotHubClient.InitIoTHubClient();
            await iotHubClient.RunSampleAsync(TimeSpan.FromMinutes(5));
        }
    }

    [IoTHub(GeneratedSendMethodName = "SendTelemetry")]
    public partial class IoTHubClientAuto
    {
        [Device(ConnectionString = "%connectionString%")]
        public DeviceClient DeviceClient { get; set; }
       
        [Desired] private string _desiredProperty;

        [Reported()] 
        private string ReportedProper { get; set; }
        
        [C2DMessage(AutoComplete = true)]
        private void OnC2dMessageReceived(Message receivedMessage)
        {
            Console.WriteLine(
                $"{DateTime.Now}> C2D message callback - message received with Id={receivedMessage.MessageId}.");

            //do something with the message

        }
    }

    [IoTHub(GeneratedSendMethodName = "SendTelemetryAsync")]
    public partial class IoTHubClient
    {
        private static readonly Random RandomGenerator = new Random();
        private const int TemperatureThreshold = 30;
        private static readonly TimeSpan SleepDuration = TimeSpan.FromSeconds(5);

        //[Device(Hostname = "%hostname%", GatewayHostname = "%GatewayHostname%", DeviceId = "123", AutoReconnect = true)]
        [Device(ConnectionString = "%alon%", TransportType = TransportType.Http1)]
        public DeviceClient DeviceClient { get; set; }
        
        /*
         todo: handle DPS
        [DpsDevice( DPSEnrollmentType = DPSEnrollmentType.Group, DPSIdScope = "scope")]
        public DeviceClient DeviceClientFull { get; set; }
        */

        private readonly ITransportSettings _amqpTransportSettings =
            new AmqpTransportSettings(TransportType.Amqp)
            {
                AmqpConnectionPoolSettings = new AmqpConnectionPoolSettings {MaxPoolSize = 5},
                IdleTimeout = TimeSpan.FromMinutes(1)
            };

        private readonly ITransportSettings _mqttTransportSettings =
            new MqttTransportSettings(TransportType.Mqtt)
            {
                DefaultReceiveTimeout = TimeSpan.FromMinutes(2)
            };

        //[TransportSetting]
        public ITransportSettings AmqpTransportSettings => _amqpTransportSettings;
        
        //[TransportSetting]
        public ITransportSettings MqttTransportSetting => _mqttTransportSettings;


        //If exist, overrides DeviceAttribute properties
        private IAuthenticationMethod _deviceAuthenticationWithRegistrySymmetricKey =
            new DeviceAuthenticationWithRegistrySymmetricKey("deviceId", "key");
        
        //[AuthenticationMethod]
        public IAuthenticationMethod DeviceAuthenticationWithRegistrySymmetricKey => _deviceAuthenticationWithRegistrySymmetricKey;

        private ClientOptions _clientOptions = new ClientOptions();

        [ClientOptions]
        public ClientOptions ClientOptions => _clientOptions;
        

        //if exist, provide a second means for device creation in case of a failure
        [AlternateConnectionString("%alternateConnectionString%")]
        private string AlternateConnectionString { get; set; }
        
        //desired property are created and managed by the source generator
        [Desired("valueFromTheCloud")] private string _desiredPropertyDemo;

        [Desired] private string _desiredPropertyAutoNameDemo;

        [Reported("valueFromTheDevice")] 
        private string ReportedPropertyDemo { get; set; }

        [Reported] 
        private string ReportedPropertyAutoNameDemo { get; set; }

        [ConnectionStatus] 
        private (ConnectionStatus Status, ConnectionStatusChangeReason Reason) DeviceConnectionStatus { get; set; }
        
        [C2DMessage(AutoComplete = false)]
        private void OnC2dMessageReceived(Message receivedMessage)
        {
            Console.WriteLine(
                $"{DateTime.Now}> C2D message callback - message received with Id={receivedMessage.MessageId}.");

            //do something with the message
        }

       

        [IoTHubErrorHandler]
        void IoTHubErrorHandler(string errorMessage, Exception exception)
        {
           if(exception is IotHubException iotHubException && iotHubException.IsTransient)
           {
                Console.WriteLine($"An IotHubException was caught, but will try to recover and retry: {exception}");
           }

           if (ExceptionHelper.IsNetworkExceptionChain(exception))
           {
               Console.WriteLine(
                   $"A network related exception was caught, but will try to recover and retry: {exception}");
           }
           
           Console.WriteLine($"{errorMessage}, Exception: {exception.Message}");
        }
        
        //[C2DMessage(AutoComplete = false)]
        private async Task OnC2dMessageReceived2(Message receivedMessage, object userContext)
        {
            Console.WriteLine(
                $"{DateTime.Now}> C2D message callback - message received with Id={receivedMessage.MessageId}.");

            //do something with the message

            await DeviceClient.CompleteAsync(receivedMessage);
            Console.WriteLine($"{DateTime.Now}> Completed C2D message with Id={receivedMessage.MessageId}.");

            receivedMessage.Dispose();

        }

        [C2DeviceCallback]
        private Task<MethodResponse> WriteToConsoleAsync(MethodRequest methodRequest)
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
                ++messageCount;
                var temperature = RandomGenerator.Next(20, 35);
                var humidity = RandomGenerator.Next(60, 80);
                string messagePayload = $"{{\"temperature\":{temperature},\"humidity\":{humidity}}}";
                var properties = new Dictionary<string, string>()
                {
                    {
                        "temperatureAlert", (temperature > TemperatureThreshold) ? "true" : "false"
                    }
                };

                while (true)
                {
                    var succeeded = await SendTelemetryAsync(messagePayload, messageCount.ToString(), cancellationToken, properties);
                    if (succeeded)
                        break;
                    // wait and retry
                    await Task.Delay(SleepDuration, cancellationToken);
                }
                await Task.Delay(SleepDuration, cancellationToken);
            }
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


        [IoTHubDeviceStatusChangesHandler]
        private void StatusChangesHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            Console.WriteLine($"Connection status changed: status={status}, reason={reason}");
            
            switch (status)
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
                    switch (reason)
                    {
                        case ConnectionStatusChangeReason.Bad_Credential:
                            // When getting this reason, the current connection string being used is not valid.
                            // If we had a backup, we can try using that.
                            Console.WriteLine("The current connection string is invalid. Trying another.");
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

                            break;

                        case ConnectionStatusChangeReason.Communication_Error:
                            Console.WriteLine(
                                "### The DeviceClient has been disconnected due to a non-retry-able exception. Inspect the exception for details." +
                                "\nIf you want to perform more operations on the device client, you should dispose (DisposeAsync()) and then open (OpenAsync()) the client.");

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
