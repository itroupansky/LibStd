using System;

namespace LibStd.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime date)
        {
            var firstDayOfMonth = date.FirstDayOfMonth();
            return firstDayOfMonth.AddMonths(1).AddDays(-1);
        }

        public static DateTime LastCalendarYearStart(this DateTime date)
        {
            return date.AddYears(-1).AddDays(1);
        }

        public static string FirstDayOfMonthStr(this DateTime date)
        {
            return date.FirstDayOfMonth().ToString("MM/dd/yyyy");
        }

        public static string LastDayOfMonthStr(this DateTime date)
        {
            return date.LastDayOfMonth().ToString("MM/dd/yyyy");
        }
    }
}
