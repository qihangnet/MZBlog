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

        /// <summary>
        /// 获取指定时间的时间戳
        /// </summary>
        /// <remarks>如果指定时间不是UTC时间，会先转换为对应的UTC时间，然后再计算</remarks>
        /// <returns>时间戳</returns>
        /// <param name="dt">时间</param>
        public static int Timestamp(this DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
                return ObjectId.GetTimestampFromDateTime(dt.ToUniversalTime());
            return ObjectId.GetTimestampFromDateTime(dt);
        }

        public static DateTime TimeExaminationLegality(this DateTime dt)
        {
            var NullTime = new DateTime(1753, 1, 1, 12, 0, 0);
            if (dt < NullTime)
                return NullTime;
            return dt;
        }
    }
}