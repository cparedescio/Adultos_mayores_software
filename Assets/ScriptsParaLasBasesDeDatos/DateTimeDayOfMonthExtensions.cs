using System;
namespace ScriptsParaLasBasesDeDatos
{
    public static class DateTimeDayOfMonthExtensions
    {
        
        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }
    
        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }
    
        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }

        public static DateTime FirstMomentOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0,0);
        }

        public static DateTime LastMomentOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59,999);
        }

        public static string CreateSqlDate(this DateTime value)
        {
            return $"{value.Year.ToString()}-" +
                   $"{value.Month.ToString()}-" +
                   $"{value.Day.ToString()} " +
                   $"{value.Hour.ToString()}:" +
                   $"{value.Minute.ToString()}:" +
                   $"{value.Second.ToString()}";
        }
    }
}