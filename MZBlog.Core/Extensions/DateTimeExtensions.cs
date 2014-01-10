using System;

namespace MZBlog.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime CloneToUtc(this DateTime dt)//修复ModelBind时将期望的UTC时间绑定为Local
        {
            if (dt.Kind == DateTimeKind.Utc)
                return dt;
            var utc = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
            return utc;
        }

        public static DateTime ToChineseTime(this DateTime dt)
        {
            //var cnOffset = TimeZoneInfo.FindSystemTimeZoneById("Asia/Shanghai").BaseUtcOffset;
            //var cnUTC = dt.Add(cnOffset);
            var cnUTC = dt.AddHours(8);
            var cnDT = new DateTime(cnUTC.Year, cnUTC.Month, cnUTC.Day, cnUTC.Hour, cnUTC.Minute, cnUTC.Second, cnUTC.Millisecond, DateTimeKind.Unspecified);
            return cnDT;
        }
    }
}
