using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using IoTHubClientGeneratorSDK;

namespace IoTHubClientGeneratorDemo
{
    public partial class IoTHubClient
    {
        public async Task InitIoTHubClientAsync()
        {
            await System.Threading.Tasks.Task.CompletedTask; //suppress async warning in case we don't generate any async call
            try
            {
                DeviceClient = CreateDeviceClient();
                DeviceClient.SetConnectionStatusChangesHandler((status, reason) =>
                {
                    DeviceConnectionStatus = (status, reason);
                    StatusChangesHandler(status, reason);
                });
                await DeviceClient.SetDesiredPropertyUpdateCallbackAsync(HandleDesiredPropertyUpdateAsync, null);
                var twin = await DeviceClient.GetTwinAsync();
                await HandleDesiredPropertyUpdateAsync(twin.Properties.Desired, null);
                await DeviceClient.SetReceiveMessageHandlerAsync(async (message, userContext) => await OnC2dMessageReceived2(message), null);
                await DeviceClient.SetMethodHandlerAsync("WriteToConsoleAsync", async (message, _) =>
                {
                    try
                    {
                        return await WriteToConsoleAsync(message);
                    }
                    catch (System.Exception exception)
                    {
                        string errorMessage = "Error handling cloud to device message. The message has been rejected";
                        IoTHubErrorHandler(errorMessage, exception);
                    }

                    return new MethodResponse(new byte[0], 500);
                }, null);
                await DeviceClient.SetMethodHandlerAsync("TestMethod", async (message, _) =>
                {
                    try
                    {
                        return await WriteToConsole2Async(message);
                    }
                    catch (System.Exception exception)
                    {
                        string errorMessage = "Error handling cloud to device message. The message has been rejected";
                        IoTHubErrorHandler(errorMessage, exception);
                    }

                    return new MethodResponse(new byte[0], 500);
                }, null);
            }
            catch (System.Exception exception)
            {
                string errorMessage = "Error initiating device client";
                IoTHubErrorHandler(errorMessage, exception);
            }
        }

        private Microsoft.Azure.Devices.Client.DeviceClient CreateDeviceClient()
        {
#pragma warning disable CS4014
            var theConnectionString = System.Environment.GetEnvironmentVariable("conString");
#pragma warning restore CS4014
            ITransportSettings[] transportSettings = new[]
            {
                AmqpTransportSettings,
                MqttTransportSetting
            };
            var deviceClient = DeviceClient.CreateFromConnectionString(theConnectionString, transportSettings, ClientOptions);
            return deviceClient;
        }

        private void ReportProperty<T>(string propertyName, T data)
        {
            try
            {
                var reportedProperties = new Microsoft.Azure.Devices.Shared.TwinCollection();
                reportedProperties[propertyName] = data.ToString();
                System.Threading.Tasks.Task.Run(async () => await DeviceClient.UpdateReportedPropertiesAsync(reportedProperties));
            }
            catch (System.Exception exception)
            {
                string errorMessage = "Error updating desired properties";
                IoTHubErrorHandler(errorMessage, exception);
            }
        }

        public string valueFromTheDevice
        {
            get
            {
                return _reportedPropertyDemo;
            }

            set
            {
                _reportedPropertyDemo = value;
                ReportProperty("valueFromTheDevice", value);
            }
        }

        public string ReportedPropertyAutoNameDemo
        {
            get
            {
                return _reportedPropertyAutoNameDemo;
            }

            set
            {
                _reportedPropertyAutoNameDemo = value;
                ReportProperty("reportedPropertyAutoNameDemo", value);
            }
        }

        private async Task<bool> SendTelemetryAsync(string jsonPayload, string messageId, System.Threading.CancellationToken cancellationToken, System.Collections.Generic.IDictionary<string, string> messageProperties = null)
        {
            try
            {
                if (DeviceConnectionStatus.ToTuple().Item1 != Microsoft.Azure.Devices.Client.ConnectionStatus.Connected)
                {
                    string errorMessage = "Error sending message to the IoT Hub, the device is not connected";
                    var exception = new System.Exception(errorMessage);
                    IoTHubErrorHandler(errorMessage, exception);
                    return false;
                }

                var iotMessage = new Microsoft.Azure.Devices.Client.Message(System.Text.Encoding.UTF8.GetBytes(jsonPayload))
                {
                    MessageId = messageId,
                    ContentEncoding = System.Text.Encoding.UTF8.ToString(),
                    ContentType = "application/json"
                };
                await DeviceClient.SendEventAsync(iotMessage, cancellationToken);
                iotMessage.Dispose();
            }
            catch (System.Exception exception)
            {
                string errorMessage = "Error sending message to the IoT Hub";
                IoTHubErrorHandler(errorMessage, exception);
                return false;
            }

            return true;
        }

        private async Task HandleDesiredPropertyUpdateAsync(Microsoft.Azure.Devices.Shared.TwinCollection desiredProperties, object context)
        {
            try
            {
                if (desiredProperties.Contains("valueFromTheCloud"))
                {
                    DesiredPropertyDemo = desiredProperties["valueFromTheCloud"];
                }

                if (desiredProperties.Contains("DesiredPropertyAutoNameDemo"))
                {
                    DesiredPropertyAutoNameDemo = desiredProperties["DesiredPropertyAutoNameDemo"];
                }
            }
            catch (System.Exception exception)
            {
                string errorMessage = "Error updating desired properties";
                IoTHubErrorHandler(errorMessage, exception);
            }

            await Task.CompletedTask;
        }
    }
}