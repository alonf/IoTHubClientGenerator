using System;
using Microsoft.Azure.Devices.Client;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Provide an authentication method that is required when creating the device client
    /// The property should return an <see cref="IAuthenticationMethod"/> instance
    /// </summary>
    /// <example>
    /// private IAuthenticationMethod _deviceAuthenticationWithRegistrySymmetricKey =
    ///      new DeviceAuthenticationWithRegistrySymmetricKey("deviceId", "key");
    /// [AuthenticationMethod]
    /// public IAuthenticationMethod DeviceAuthenticationWithRegistrySymmetricKey => _deviceAuthenticationWithRegistrySymmetricKey;
    /// </example>
   
    [AttributeUsage(AttributeTargets.Property)]
    public class AuthenticationMethodAttribute : Attribute
    {
    }
}