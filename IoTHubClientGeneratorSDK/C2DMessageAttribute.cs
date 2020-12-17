using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class C2DMessageAttribute : Attribute
    {
        /// <summary>
        /// If set to true, the message is auto completed if there is no exception thrown
        /// </summary>
        public bool AutoComplete { get; set; } = true;
        /*
          await DeviceClient.CompleteAsync(receivedMessage);
            Console.WriteLine($"{DateTime.Now}> Completed C2D message with Id={receivedMessage.MessageId}.");

            receivedMessage.Dispose();
         */
    }
}