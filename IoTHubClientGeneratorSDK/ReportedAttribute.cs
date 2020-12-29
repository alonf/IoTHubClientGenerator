using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Decorate a field to generate device twin reported attribute
    /// </summary>
    /// <example>
    ///[Reported("valueFromTheDevice")] private string _reportedPropertyDemo;
    ///[Reported("ReportedPropertyAutoNameDemo", "reportedPropertyAutoNameDemo")] private string _reportedPropertyAutoNameDemo;
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReportedAttribute : Attribute
    {
        /// <summary>
        /// The local name of the generated proxy
        /// </summary>
        public string LocalPropertyName { get; }
        /// <summary>
        /// The cloud twin property name
        /// </summary>
        public string TwinPropertyName { get; set; }

        /// <summary>
        /// Decorate a field to generate device twin reported attribute
        /// </summary>
        /// <param name="localPropertyName">The local name of the generated proxy</param>
        /// <param name="twinPropertyName">The cloud twin property name. If not provided, the twin property name is the same as the local property name</param>
        public ReportedAttribute(string localPropertyName, string twinPropertyName = "")
        {
            LocalPropertyName = localPropertyName;
            TwinPropertyName = twinPropertyName;
        }
    }
}