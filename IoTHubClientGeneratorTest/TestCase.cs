using System;

namespace IoTHubClientGeneratorTest
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TestCase : Attribute
    {
        public string TestName { get; }

        public TestCase(string testName)
        {
            TestName = testName;
        }
    }
}