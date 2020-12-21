using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReportedAttribute : Attribute
    {
        public string LocalPropertyName { get; }
        public string TwinPropertyName { get; set; }

        public ReportedAttribute(string localPropertyName, string twinPropertyName = "")
        {
            LocalPropertyName = localPropertyName;
            TwinPropertyName = twinPropertyName;
        }
    }
}