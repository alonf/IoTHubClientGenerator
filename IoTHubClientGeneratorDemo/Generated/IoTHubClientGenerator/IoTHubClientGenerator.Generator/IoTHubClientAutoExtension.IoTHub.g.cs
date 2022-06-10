using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using IoTHubClientGeneratorSDK;

namespace IoTHubClientGeneratorDemo
{
    public partial class IoTHubClientAuto
    {
        public async Task InitIoTHubClientAsync()
        {
            await System.Threading.Tasks.Task.CompletedTask; //suppress async warning in case we don't generate any async call
            try
            {
                DeviceClient = CreateDeviceClient();
                await DeviceClient.SetDesiredPropertyUpdateCallbackAsync(HandleDesiredPropertyUpdateAsync, null);
                var twin = await DeviceClient.GetTwinAsync();
                await HandleDesiredPropertyUpdateAsync(twin.Properties.Desired, null);
                await DeviceClient.SetReceiveMessageHandlerAsync(async (message, _) =>
                {
                    try
                    {
                        OnC2dMessageReceived(message);
                        await DeviceClient.CompleteAsync(message);
                        message.Dispose();
                        message = null;
                    }
                    catch (System.Exception exception)
                    {
                        await DeviceClient.RejectAsync(message);
                        if (message != null)
                            message.Dispose();
                        string errorMessage = "Error handling cloud to device message. The message has been rejected";
                        IoTHubErrorHandler(errorMessage, exception);
                    }
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
            var theConnectionString = System.Environment.GetEnvironmentVariable("ConnectionString");
            var deviceClient = DeviceClient.CreateFromConnectionString(theConnectionString);
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

        public string ReportedProperty
        {
            get
            {
                return _reportedProperty;
            }

            set
            {
                _reportedProperty = value;
                ReportProperty("reported", value);
            }
        }

        private async Task HandleDesiredPropertyUpdateAsync(Microsoft.Azure.Devices.Shared.TwinCollection desiredProperties, object context)
        {
            try
            {
                if (desiredProperties.Contains("DesiredProperty"))
                {
                    string textData = desiredProperties["DesiredProperty"];
                    DesiredProperty = Int32.Parse(textData);
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