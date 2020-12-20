using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReportedAttribute : Attribute
    {
        public string TwinPropertyName { get; set; }

        public ReportedAttribute(string twinPropertyName = "")
        {
            TwinPropertyName = twinPropertyName;
        }
    }
}