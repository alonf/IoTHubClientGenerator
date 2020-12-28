using System;
using System.Collections.Generic;

namespace IoTHubClientGenerator
{
    public static class Util
    {
        private static Dictionary<string, string> _typesToFriendlyNames = new ()
        {
            {typeof(bool).ToString(), "bool"},
            {typeof(byte).ToString(), "byte"},
            {typeof(sbyte).ToString(), "sbyte"},
            {typeof(char).ToString(), "char"},
            {typeof(decimal).ToString(), "decimal"},
            {typeof(double).ToString(), "double"},
            {typeof(float).ToString(), "float"},
            {typeof(int).ToString(), "int"},
            {typeof(uint).ToString(), "uint"},
            {typeof(long).ToString(), "long"},
            {typeof(ulong).ToString(), "ulong"},
            {typeof(object).ToString(), "object"},
            {typeof(short).ToString(), "short"},
            {typeof(ushort).ToString(), "ushort"},
            {typeof(string).ToString(), "string"}
        };
        
        public static string AttName(this string fullAttributeName) => fullAttributeName.Replace("Attribute", "");

        public static string GetFriendlyNameOfPrimitive(string clrTypeName)
        {
            if (_typesToFriendlyNames.TryGetValue("System."+ clrTypeName, out string typeName))
            {
                return typeName;
            }

            //else
            return clrTypeName;
        }
    }
}