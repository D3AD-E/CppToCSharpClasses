using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPPtoCSharpClasses.Utils
{
    internal static class FileService
    {
        public static List<string> ReadFile(string filePath)
        {
            var logFile = File.ReadAllLines(filePath);
            return new List<string>(logFile);
        }

        public static void CreateAndWriteToFile(string filePath, string contents)
        {
            using StreamWriter sw = File.CreateText(filePath);
            sw.Write(contents);
            sw.Flush();
            sw.Close();
        }
    }
}
