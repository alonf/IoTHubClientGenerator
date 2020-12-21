using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DesiredAttribute : Attribute
    {
        public string TwinPropertyName { get; set; }

        public DesiredAttribute(string twinPropertyName = "")
        {
            TwinPropertyName = twinPropertyName;
        }
    }
}