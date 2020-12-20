using System;

namespace IoTHubClientGeneratorSDK
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AlternateConnectionStringAttribute : Attribute
    {
        private readonly string _connectionString;

        public AlternateConnectionStringAttribute(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}