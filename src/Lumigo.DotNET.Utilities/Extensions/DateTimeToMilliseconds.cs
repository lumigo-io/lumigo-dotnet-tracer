using System;

namespace Lumigo.DotNET.Utilities.Extensions
{
    public static class DateTimeToMilliseconds
    {
        public static long ToMilliseconds(this DateTime dateTime)
        {
            return (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}
