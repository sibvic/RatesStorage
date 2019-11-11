using System;

namespace ProfitRobots.RatesStorage
{
    public static class DateTimeUtils
    {
        public static DateTime ScrollTo(this DateTime date, DayOfWeek dayOfWeek)
        {
            var day = date;
            while (day.DayOfWeek != dayOfWeek)
                day = day.AddDays(1);

            return day;
        }

        public static DateTime RollBackTo(this DateTime date, DayOfWeek dayOfWeek)
        {
            var day = date;
            while (day.DayOfWeek != dayOfWeek)
                day = day.AddDays(-1);

            return day;
        }

        public static int WeekNumber(this DateTime date)
        {
            var firstDay = new DateTime(date.Year, 1, 1);
            var firstMonday = firstDay.ScrollTo(DayOfWeek.Monday);
            int shift = (firstMonday - firstDay).Days;

            return (date.DayOfYear - shift) / 7 + 1;
        }
    }
}
