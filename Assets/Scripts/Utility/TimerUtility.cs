using System;

namespace Utility.Utility
{
    public static class TimerUtility
    {
        public static string FormatTime(long timeInSeconds)
        {
            var ts = ToTimeSpan((int) timeInSeconds);
            return GetFormattedTime(ts.Hours, ts.Minutes, ts.Seconds);
        }

        public static string GetDateFromTimestamp(long timestamp)
        {
            var dateoffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
            return GetDateFormat(dateoffset.UtcDateTime);
        }

        public static string GetTimeFormat(DateTime date) =>
            date.ToLocalTime().ToString("%h:mm tt");

        public static string GetDateFormat(DateTime date)
        {
            switch (date.Day) {
                case 1:
                    return date.ToString("MMMM dd") + "st";
                case 2:
                    return date.ToString("MMMM dd") + "nd";
                case 3:
                    return date.ToString("MMMM dd") + "rd";
                default:
                    return date.ToString("MMMM dd") + "th";
            }
        }

        private static TimeSpan ToTimeSpan(int time)
        {
            var hours = (int) Math.Max(0, Math.Floor(time / 3600d));
            var minutes = (int) Math.Max(0, Math.Floor(time / 60d) - hours * 60);
            var seconds = Math.Max(0, time % 60);

            return new TimeSpan(hours, minutes, seconds);
        }

        private static string GetFormattedTime(int hours, int minutes, int seconds) =>
            hours > 0
                ? $"{hours}:{minutes:00}:{seconds:00}"
                : $"{minutes:00}:{seconds:00}";
    }
}