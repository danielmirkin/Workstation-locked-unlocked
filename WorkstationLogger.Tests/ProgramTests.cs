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
}
