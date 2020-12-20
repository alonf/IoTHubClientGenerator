using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DesiredAttribute : Attribute
    {
        public string TwinPropertyName { get; set; }

        public DesiredAttribute(string twinPropertyName = "")
        {
            TwinPropertyName = twinPropertyName;
        }
    }
}