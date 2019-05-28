using System;
using System.Globalization;

namespace IWantTo.Responder.Extensions
{
    public static class DateExtension
    {
        public static string ToPersianDate(this DateTime date)
        {
            var persianCalendar = new PersianCalendar();
            var year = persianCalendar.GetYear(date);
            var month = persianCalendar.GetMonth(date);
            var day = persianCalendar.GetDayOfMonth(date);

            return $"{year}/{month}/{day}";
        }

        public static DateTime ProcessDateFromString(this string stringDate)
        {
            var persianCalendar = new PersianCalendar();
            var det = '/';
            if (stringDate.Contains('/'))
            {
                det = '/';
            }
            else if (stringDate.Contains('\\'))
            {
                det = '\\';
            }
            else if (stringDate.Contains('-'))
            {
                det = '-';
            }

            var parts = stringDate.Split(det);
            int year = 0, month = 0, day = 0;
            year = int.Parse(parts[0]);
            month = int.Parse(parts[1]);
            day = int.Parse(parts[2]);
            if (year < day)
            {
                day = year;
                year = int.Parse(parts[2]);
            }

            return persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
    }
}
