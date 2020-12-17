using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReportedAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public ReportedAttribute(string propertyName = "")
        {
            PropertyName = propertyName;
        }
    }
}