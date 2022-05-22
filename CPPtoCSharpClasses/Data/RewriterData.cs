using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPtoCSharpClasses
{
    internal static class RewriterData
    {
        public static string ConvertToPascalCase(string name)
        {
            return name.Split(new[] { "_" }, 
                StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2).Replace("()", "");
        }
    }
}
