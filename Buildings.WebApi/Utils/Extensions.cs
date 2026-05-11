namespace Buildings.Utils;

public static class Extensions
{
    /// <summary>
    ///     将 DateTimeOffset 转换为相对时间字符串（中文）。
    /// </summary>
    /// <param name="target">目标时间</param>
    /// <returns>例如 “4分钟前” “2天2小时前” “35天前”</returns>
    public static string ToRelativeTimeString(this DateTimeOffset target)
    {
        var now = DateTimeOffset.Now;
        var diff = now - target;

        // 未来时间
        if (diff.TotalSeconds < 0)
            return "即将到来";

        // 小于 1 分钟
        if (diff.TotalSeconds < 60)
            return "刚刚";

        // 小于 1 小时
        if (diff.TotalMinutes < 60)
            return $"{diff.Minutes}分钟前";

        // 小于 1 天
        if (diff.TotalHours < 24)
            return $"{diff.Hours}小时前";

        // 大于等于 3 天 → 显示日期
        if (diff.TotalDays >= 3)
        {
            // 当年：显示 "MM-dd"，跨年：显示 "yyyy-MM-dd"
            var format = target.Year == now.Year ? "MM-dd" : "yyyy-MM-dd";
            return target.ToString(format);
        }

        // 1 ~ 2 天：显示“X天前”或“X天X小时前”
        var days = diff.Days;
        var hours = diff.Hours;
        if (hours > 0)
            return $"{days}天{hours}小时前";
        return $"{days}天前";
    }
}