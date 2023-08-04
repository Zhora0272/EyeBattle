using System;
using JetBrains.Annotations;

namespace Vengadores.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [PublicAPI] public static long ToTimestamp(this DateTime d) 
        {
            return (long)(d.ToUniversalTime() - Jan1St1970).TotalSeconds;
        }
        
        [PublicAPI] public static DateTime TimestampToDateTime(this double unixTimestamp)
        {
            return Jan1St1970.AddSeconds(unixTimestamp);
        }
    }
}