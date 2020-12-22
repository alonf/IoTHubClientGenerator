using System;

namespace IoTHubClientGeneratorTest
{
    public class TestCase : Attribute
    {
        public string TestName { get; }

        public TestCase(string testName)
        {
            TestName = testName;
        }
    }
}