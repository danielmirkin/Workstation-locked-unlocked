using System;
using System.IO;
using Xunit;
using WorkstationLogger;

public class ProgramTests
{
    private static string TempFile() =>
        Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");

    // --- GetLastDate ---

    [Fact]
    public void GetLastDate_ReturnsNull_WhenFileHasNoTimestamps()
    {
        string file = TempFile();
        File.WriteAllLines(file, new[] { "---", "---", "" });
        try
        {
            DateTime? result = Program.GetLastDate(file);
            Assert.Null(result);
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public void GetLastDate_ParsesLastTimestampLine()
    {
        string file = TempFile();
        File.WriteAllLines(file, new[]
        {
            "Monday 04-05-2026 09:00:00",
            "Monday 04-05-2026 17:00:00"
        });
        try
        {
            DateTime? result = Program.GetLastDate(file);
            Assert.NotNull(result);
            Assert.Equal(new DateTime(2026, 5, 4, 17, 0, 0), result!.Value);
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public void GetLastDate_SkipsSeparatorLines_ReturnsLastTimestamp()
    {
        string file = TempFile();
        File.WriteAllLines(file, new[]
        {
            "Monday 04-05-2026 09:00:00",
            "---",
            "Tuesday 05-05-2026 08:30:00",
            "---"
        });
        try
        {
            DateTime? result = Program.GetLastDate(file);
            Assert.NotNull(result);
            Assert.Equal(new DateTime(2026, 5, 5, 8, 30, 0), result!.Value);
        }
        finally { File.Delete(file); }
    }

    // --- Log ---

    [Fact]
    public void Log_CreatesFileWithSingleTimestamp_WhenFileDoesNotExist()
    {
        string file = TempFile();
        try
        {
            Program.Log(file);
            Assert.True(File.Exists(file));
            string[] lines = File.ReadAllLines(file);
            Assert.Single(lines);
            Assert.True(DateTime.TryParseExact(lines[0], "dddd dd-MM-yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _));
        }
        finally { if (File.Exists(file)) File.Delete(file); }
    }

    [Fact]
    public void Log_AppendsTimestampOnly_WhenSameDay()
    {
        string file = TempFile();
        string todayEntry = DateTime.Now.ToString("dddd dd-MM-yyyy HH:mm:ss",
            System.Globalization.CultureInfo.InvariantCulture);
        File.WriteAllLines(file, new[] { todayEntry });
        try
        {
            Program.Log(file);
            string[] lines = File.ReadAllLines(file);
            Assert.Equal(2, lines.Length);
            Assert.DoesNotContain("---", lines);
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public void Log_WritesSeparatorThenTimestamp_WhenDifferentDay()
    {
        string file = TempFile();
        string yesterdayEntry = DateTime.Now.AddDays(-1).ToString("dddd dd-MM-yyyy HH:mm:ss",
            System.Globalization.CultureInfo.InvariantCulture);
        File.WriteAllLines(file, new[] { yesterdayEntry });
        try
        {
            Program.Log(file);
            string[] lines = File.ReadAllLines(file);
            Assert.Equal(3, lines.Length);
            Assert.Equal("---", lines[1]);
            Assert.True(DateTime.TryParseExact(lines[2], "dddd dd-MM-yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _));
        }
        finally { File.Delete(file); }
    }

    [Fact]
    public void Log_WritesTimestamp_WhenFileExistsButIsEmpty()
    {
        string file = TempFile();
        File.WriteAllText(file, "");
        try
        {
            Program.Log(file);
            string[] lines = File.ReadAllLines(file);
            Assert.Single(lines);
            Assert.True(DateTime.TryParseExact(lines[0], "dddd dd-MM-yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out _));
        }
        finally { File.Delete(file); }
    }
}
