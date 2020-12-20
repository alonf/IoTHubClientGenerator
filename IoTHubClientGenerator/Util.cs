namespace IoTHubClientGenerator
{
    public static class Util
    {
        public static string AttName(this string fullAttributeName) => fullAttributeName.Replace("Attribute", "");
    }
}