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
            if (args.Length == 0 || (args[0] != "lock" && args[0] != "unlock"))
            {
                Console.Error.WriteLine("Usage: WorkstationLogger.exe lock|unlock");
                Environment.Exit(1);
            }

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string logFile = args[0] == "lock"
                ? Path.Combine(baseDir, "logoff.txt")
                : Path.Combine(baseDir, "logon.txt");

            Log(logFile);
        }

        public static void Log(string logFile)
        {
            string timestamp = DateTime.Now.ToString(TimestampFormat, CultureInfo.InvariantCulture);

            if (!File.Exists(logFile) || new FileInfo(logFile).Length == 0)
            {
                File.AppendAllText(logFile, timestamp + Environment.NewLine);
                return;
            }

            DateTime? lastDate = GetLastDate(logFile);
            using StreamWriter w = File.AppendText(logFile);
            if (lastDate == null || lastDate.Value.Date != DateTime.Now.Date)
                w.WriteLine("---");
            w.WriteLine(timestamp);
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
