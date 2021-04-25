using System;

public static class TimespanExtensions
{
    public static string ToHumanReadableString (this TimeSpan t)
    {
        if (t.TotalMinutes <= 1) {
            return $@"{t:%s} seconds";
        }
        if (t.TotalHours <= 1) {
            return $@"{t:%m} minutes {t:%s} seconds";
        }
        if (t.TotalDays <= 1) {
            return $@"{t:%h} hours {t:%m} minutes";
        }

        return $@"{t:%d} days {t:%h} hours {t:%m} minutes";
    }
}