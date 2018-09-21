using System.Globalization;
using System.Windows;
using Calendar = System.Globalization.Calendar;

namespace System
{
    public static class DateTimeHelper
    {
        private static readonly Calendar Cal = new GregorianCalendar();

        public static DateTime? AddDays(DateTime time, int days)
        {
            try
            {
                return Cal.AddDays(time, days);
            }
            catch (ArgumentException)
            {
                return new DateTime?();
            }
        }

        public static DateTime? AddMonths(DateTime time, int months)
        {
            try
            {
                return Cal.AddMonths(time, months);
            }
            catch (ArgumentException)
            {
                return new DateTime?();
            }
        }

        public static DateTime? AddYears(DateTime time, int years)
        {
            try
            {
                return Cal.AddYears(time, years);
            }
            catch (ArgumentException)
            {
                return new DateTime?();
            }
        }

        public static DateTime? SetYear(DateTime date, int year)
        {
            return AddYears(date, year - date.Year);
        }

        public static DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
        {
            var dt = SetYear(date, yearMonth.Year);
            if (dt.HasValue)
                dt = AddMonths(dt.Value, yearMonth.Month - date.Month);
            return dt;
        }

        public static int CompareDays(DateTime dt1, DateTime dt2)
        {
            var dt11 = DiscardTime(dt1);
            var dt22 = DiscardTime(dt2);

            return DateTime.Compare(dt11, dt22);
        }

        public static int CompareYearMonth(DateTime dt1, DateTime dt2)
        {
            return (dt1.Year - dt2.Year) * 12 + (dt1.Month - dt2.Month);
        }

        public static int DecadeOfDate(DateTime date)
        {
            return date.Year - date.Year % 10;
        }

        public static int DaysInYear(DateTime date)
        {
            return DateTime.IsLeapYear(date.Year) ? 366 : 365;
        }

        public static DateTime DiscardDayMonthTime(DateTime d)
        {
            return new DateTime(d.Year, 1, 1, 0, 0, 0);
        }

        public static DateTime? DiscardDayMonthTime(DateTime? d)
        {
            return d.HasValue ? DiscardDayMonthTime(d.Value) : new DateTime?();
        }

        public static DateTime DiscardDayTime(DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1, 0, 0, 0);
        }

        public static DateTime? DiscardTime(DateTime? d)
        {
            return d?.Date;
        }

        public static DateTime DiscardTime(DateTime d)
        {
            return d.Date;
        }

        public static DateTime? DiscardDate(DateTime? d)
        {
            return d.HasValue ? DiscardDate(d.Value) : new DateTime?();
        }

        public static DateTime? DiscardDate(DateTime? d, DateTime defaultDate)
        {
            return d.HasValue ? DiscardDate(d.Value, defaultDate) : new DateTime?();
        }

        public static DateTime DiscardDate(DateTime d)
        {
            return DiscardDate(d, DateTime.MinValue);
        }

        public static DateTime DiscardDate(DateTime d, DateTime defaultDate)
        {
            return new DateTime(
                defaultDate.Year,
                defaultDate.Month,
                defaultDate.Day,
                d.Hour,
                d.Minute,
                d.Second,
                d.Millisecond);
        }

        public static int EndOfDecade(DateTime date)
        {
            return DecadeOfDate(date) + 9;
        }

        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            return GetDateFormat(CultureInfo.CurrentCulture);
        }

        private static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            if (culture.Calendar is GregorianCalendar)
                return culture.DateTimeFormat;

            var gregorianCalendar = (GregorianCalendar)null;
            foreach (Calendar calendar in culture.OptionalCalendars)
            {
                if (calendar is GregorianCalendar)
                {
                    if (gregorianCalendar == null)
                        gregorianCalendar = (GregorianCalendar)calendar;

                    if (((GregorianCalendar)calendar).CalendarType == GregorianCalendarTypes.Localized)
                    {
                        gregorianCalendar = calendar as GregorianCalendar;
                        break;
                    }
                }
            }

            DateTimeFormatInfo dateTimeFormat;
            if (gregorianCalendar == null)
            {
                dateTimeFormat = ((CultureInfo)CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
                dateTimeFormat.Calendar = new GregorianCalendar();
            }
            else
            {
                dateTimeFormat = ((CultureInfo)culture.Clone()).DateTimeFormat;
                dateTimeFormat.Calendar = gregorianCalendar;
            }

            return dateTimeFormat;
        }

        public static bool InRange(DateTime date, DateTime start, DateTime end)
        {
            return CompareDays(date, start) > -1 && CompareDays(date, end) < 1;
        }

        public static bool Intersects(DateTime? start1, DateTime? end1, DateTime? start2, DateTime? end2)
        {
            if (start1 == null)
                start1 = DateTime.MinValue;

            if (end1 == null)
                end1 = DateTime.MaxValue;

            if (start2 == null)
                start2 = DateTime.MinValue;

            if (end2 == null)
                end2 = DateTime.MaxValue;

            return Intersects(start1.Value, end1.Value, start2.Value, end2.Value);
        }

        public static bool Intersects(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            if (start1 > end1)
                (end1, start1) = (start1, end1);

            if (start2 > end2)
                (end2, start2) = (start2, end2);

            return start2 <= end1 && start1 <= end2;
        }

        public static DateTime GetFirstDateOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static DateTime GetFistDateOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetFirstDateOfWeek(DateTime date, DayOfWeek firstDayOfWeek)
        {
            var weekOffset = GetWeekOffset(date, firstDayOfWeek);
            return date.AddDays(-weekOffset);
        }

        public static int GetWeekOffset(DateTime date, DayOfWeek firstDayOfWeek)
        {
            date = DiscardTime(date);
            return GetWeekOffset(date.DayOfWeek, firstDayOfWeek);
        }

        public static int GetWeekOffset(DayOfWeek dayOfWeek, DayOfWeek firstDayOfWeek)
        {
            var firstDayOfWeekIndex = (int)firstDayOfWeek;
            var index = (int)dayOfWeek;

            if (index < firstDayOfWeekIndex)
                index += 7;

            return index - firstDayOfWeekIndex;
        }

        public static DateTime? GetDateTimeFrom(DateTime? possibleDate, DateTime? possibleTime)
        {
            if (!possibleDate.HasValue)
                return null;

            var date = possibleDate.Value.Date;
            var time = possibleTime ?? DateTime.Now;
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hour,
                time.Minute,
                time.Second);
        }

        public static DateTime? GetDateTimeFrom(DateTime? possibleDate, TimeSpan? possibleTime)
        {
            if (!possibleDate.HasValue)
                return null;

            var date = possibleDate.Value.Date;
            var time = possibleTime ?? TimeSpan.Zero;
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hours,
                time.Minutes,
                time.Seconds);
        }

        public static DateTime GetDateTimeFrom(DateTime possibleDate, DateTime possibleTime)
        {
            var date = possibleDate.Date;
            var time = possibleTime;
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hour,
                time.Minute,
                time.Second);
        }

        public static DateTime GetDateTimeFrom(DateTime possibleDate, TimeSpan possibleTime)
        {
            var date = possibleDate.Date;
            var time = possibleTime;
            return new DateTime(
                date.Year,
                date.Month,
                date.Day,
                time.Hours,
                time.Minutes,
                time.Seconds);
        }

        public static DateTime GetUtcToday()
        {
            return DiscardTime(DateTime.UtcNow);
        }

        public static string ToWeekRangeString(DateTime date, DayOfWeek firstDayOfWeek, CultureInfo culture)
        {
            var firstDateOfWeek = GetFirstDateOfWeek(date, firstDayOfWeek);
            var lastDateOfWeek = firstDateOfWeek.AddDays(7);
            return $"{firstDateOfWeek.ToString("M", culture)} - {lastDateOfWeek.ToString("M", culture)}, {lastDateOfWeek.Year}";
        }

        public static string ToDayString(DateTime? date, CultureInfo culture)
        {
            var str = string.Empty;
            var dateFormat = GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                str = date.Value.Day.ToString(dateFormat);
            return str;
        }

        public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
        {
            var str = string.Empty;
            var dateFormat = GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                str = date.Value.ToString(dateFormat.YearMonthPattern, dateFormat);
            return str;
        }

        public static string ToYearString(DateTime? date, CultureInfo culture)
        {
            var str = string.Empty;
            var dateFormat = GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                str = date.Value.Year.ToString(dateFormat);
            return str;
        }

        public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
        {
            var str = string.Empty;
            var dateFormat = GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
            {
                var abbreviatedMonthNames = dateFormat.AbbreviatedMonthNames;
                if (abbreviatedMonthNames != null && abbreviatedMonthNames.Length > 0)
                    str = abbreviatedMonthNames[(date.Value.Month - 1) % abbreviatedMonthNames.Length];
            }
            return str;
        }

        public static string ToLongDateString(DateTime? date, CultureInfo culture)
        {
            var str = string.Empty;
            var dateFormat = GetDateFormat(culture);
            if (date.HasValue && dateFormat != null)
                str = date.Value.Date.ToString(dateFormat.LongDatePattern, dateFormat);
            return str;
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return DateTime.Compare(date1, date2) >= 0 ? date1 : date2;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return DateTime.Compare(date1, date2) >= 0 ? date2 : date1;
        }

        public static int DaysInYear(int year)
        {
            return 365 + Convert.ToInt32(DateTime.IsLeapYear(year));
        }

        public static DateTime GetDate(int year, int month, DayOfWeek dayOfWeek, int ordinal)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);

            var dayOfMonth = (int)dayOfWeek - (int)firstDayOfMonth.DayOfWeek + 1;
            if (dayOfMonth <= 0)
                dayOfMonth += 7;

            var daysInMonth = DateTime.DaysInMonth(year, month);
            for (var j = 0; j < ordinal; j++)
            {
                if (dayOfMonth + 7 > daysInMonth)
                    break;

                dayOfMonth += 7;
            }
            return firstDayOfMonth.AddDays(dayOfMonth - 1);
        }

        public static int GetMonthByDayOfYear(int year, int daysFromFirstDateOfYear)
        {
            if (daysFromFirstDateOfYear < 0 || daysFromFirstDateOfYear > 366)
                throw new ArgumentException("The param must be between 0 and 366", nameof(daysFromFirstDateOfYear));

            for (var i = 1; i <= 12; i++)
            {
                daysFromFirstDateOfYear -= DateTime.DaysInMonth(year, i);
                if (daysFromFirstDateOfYear <= 0)
                    return i;
            }

            return -1;
        }

        public static int GetOffsetInMonthByDayOfYear(int year, int daysFromFirstDateOfYear)
        {
            if (daysFromFirstDateOfYear < 0 || daysFromFirstDateOfYear > 366)
                throw new ArgumentException("The param must be between 0 and 366", nameof(daysFromFirstDateOfYear));

            for (var i = 1; i <= 12; i++)
            {
                var daysInMonth = DateTime.DaysInMonth(year, i);
                if (daysFromFirstDateOfYear <= daysInMonth)
                    return daysFromFirstDateOfYear;

                daysFromFirstDateOfYear -= daysInMonth;
            }

            return -1;
        }
    }
}
