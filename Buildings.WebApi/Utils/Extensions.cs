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
        // 使用当前本地时间（自动处理时区偏移），也可改用 UtcNow
        var now = DateTimeOffset.Now;
        var diff = now - target;

        // 处理未来时间（如果目标时间在未来）
        if (diff.TotalSeconds < 0)
            return "即将到来"; // 或按需处理为“未来X秒/分/小时...”，这里简单处理

        // 小于 1 分钟
        if (diff.TotalSeconds < 60)
            return "刚刚";

        // 小于 1 小时
        if (diff.TotalMinutes < 60)
            return $"{diff.Minutes}分钟前";

        // 小于 1 天
        if (diff.TotalHours < 24)
            return $"{diff.Hours}小时前";

        // 小于 30 天：尝试混合显示“天+小时”
        if (diff.TotalDays < 30)
        {
            var days = diff.Days;
            var hours = diff.Hours; // diff.Hours 范围 0-23
            if (hours > 0)
                return $"{days}天{hours}小时前";
            return $"{days}天前";
        }

        // 30 天及以上：只显示天数
        return $"{diff.Days}天前";
    }
}