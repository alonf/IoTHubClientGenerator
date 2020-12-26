using System.Diagnostics;

namespace IoTHubClientGenerator
{
    partial class IoTHubPartialClassBuilder
    {
        private void GenerateProvisioningTransportHandler(string transportType)
        {
            Append("using var transport = new ", true);
            switch (transportType)
            {
                case "TransportType.Mqtt":
                    AppendLine("ProvisioningTransportHandlerMqtt();");
                    break;
                case "TransportType.Mqtt_Tcp_Only":
                    AppendLine("ProvisioningTransportHandlerMqtt(TransportFallbackType.WebSocketOnly);");
                    break;
                case "TransportType.Mqtt_WebSocket_Only":
                    AppendLine("ProvisioningTransportHandlerMqtt(TransportFallbackType.TcpOnly);");
                    break;
                case "TransportType.Amqp":
                    AppendLine("ProvisioningTransportHandlerAmqp();");
                    break;
                case "TransportType.Amqp_Tcp_Only":
                    AppendLine("ProvisioningTransportHandlerAmqp(TransportFallbackType.WebSocketOnly);");
                    break;
                case "TransportType.Amqp_WebSocket_Only":
                    AppendLine("ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);");
                    break;
                case "TransportType.Http1":
                    AppendLine("ProvisioningTransportHandlerHttp();");
                    break;
            }
        }
    }
}