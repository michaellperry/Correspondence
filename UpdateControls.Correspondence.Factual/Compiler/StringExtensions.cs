using System;

namespace UpdateControls.Correspondence.Factual.Compiler
{
    public static class StringExtensions
    {
        public static string PascalCase(this string name)
        {
            return name.Length > 0 ? name.Substring(0, 1).ToUpper() + name.Substring(1) : name;
        }

        public static string CamelCase(this string name)
        {
            return name.Length > 0 ? name.Substring(0, 1).ToLower() + name.Substring(1) : name;
        }
    }
}
