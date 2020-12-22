using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a property to be automatically updated from the device twin properties
    /// Example:
    /// <example>
    /// [Desired("valueFromTheCloud")] private string DesiredPropertyDemo { get; set; }
    /// [Desired] private string DesiredPropertyAutoNameDemo { get; set; }
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DesiredAttribute : Attribute
    {
        /// <summary>
        /// The name of the cloud device property.
        /// It will be derived from the property name, if not provided
        /// </summary>
        public string TwinPropertyName { get; set; }

        /// <summary>
        /// Decorate a property to be automatically updated from the device twin properties
        /// </summary>
        /// <param name="twinPropertyName">The name of the cloud device property. It will be derived from the property name, if not provided.</param>
        public DesiredAttribute(string twinPropertyName = "")
        {
            TwinPropertyName = twinPropertyName;
        }
    }
}