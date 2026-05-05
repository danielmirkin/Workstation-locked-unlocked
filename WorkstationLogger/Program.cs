using System;
using System.Globalization;
using System.IO;

namespace WorkstationLogger
{
    class Program
    {
        const string TimestampFormat = "dddd dd-MM-yyyy HH:mm:ss";

        static void Main(string[] args)
        {
            // TODO: implement in Task 4
        }

        public static void Log(string logFile)
        {
            // TODO: implement in Task 3
            throw new NotImplementedException();
        }

        public static DateTime? GetLastDate(string logFile)
        {
            string[] lines = File.ReadAllLines(logFile);
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (DateTime.TryParseExact(line, TimestampFormat, CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime dt))
                    return dt;
            }
            return null;
        }
    }
}
