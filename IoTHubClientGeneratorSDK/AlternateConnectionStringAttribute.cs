using System;

namespace IoTHubClientGeneratorSDK
{
    /// <summary>
    /// Supply an alternative ConnectionString to be use if the device client can't
    /// establish communication using the provided connection parameters on the [Device] attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AlternateConnectionStringAttribute : Attribute
    {
        private readonly string _connectionString;

        /// <summary>
        /// Supply an alternative ConnectionString to be use if the device client can't
        /// establish communication using the provided connection parameters on the [Device] attribute
        /// </summary>
        /// <param name="connectionString">The connection string. It can be %connectionString% for using environment variable</param>
        public AlternateConnectionStringAttribute(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}