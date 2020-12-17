using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DesiredAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public DesiredAttribute(string propertyName = "")
        {
            PropertyName = propertyName;
        }
    }
}