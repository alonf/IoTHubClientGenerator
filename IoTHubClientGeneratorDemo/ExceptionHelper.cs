using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using DotNetty.Transport.Channels;

namespace IoTHubClientGeneratorDemo
{
    internal class ExceptionHelper
    {
        private static readonly HashSet<Type> NetworkExceptions = new HashSet<Type>
        {
            typeof(IOException),
            typeof(SocketException),
            typeof(ClosedChannelException),
            typeof(TimeoutException),
            typeof(OperationCanceledException),
            typeof(HttpRequestException),
            typeof(WebException),
            typeof(WebSocketException),
        };

        private static bool IsNetwork(Exception singleException)
        {
            return NetworkExceptions.Any(baseExceptionType => baseExceptionType.IsInstanceOfType(singleException));
        }

        internal static bool IsNetworkExceptionChain(Exception exceptionChain)
        {
            return exceptionChain.Unwind(true).Any(e => IsNetwork(e) && !IsTlsSecurity(e));
        }

        internal static bool IsSecurityExceptionChain(Exception exceptionChain)
        {
            return exceptionChain.Unwind(true).Any(IsTlsSecurity);
        }

        private static bool IsTlsSecurity(Exception singleException)
        {
            if (// WinHttpException (0x80072F8F): A security error occurred.
                (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && (singleException.HResult == unchecked((int)0x80072F8F))) ||
                // CURLE_SSL_CACERT (60): Peer certificate cannot be authenticated with known CA certificates.
                (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && (singleException.HResult == 60)) ||
                singleException is AuthenticationException)
            {
                return true;
            }

            return false;
        }
    }
}
