using System.Runtime.InteropServices;

using SmartCommune.Application.Common.Interfaces.Services;

namespace SmartCommune.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Chỉ dùng khi muốn hiển thị ngày tháng cho người Việt Nam hoặc lưu log.
    /// Với các logic hệ thống, token, database timestamp,... hãy dùng DateTime.UtcNow.
    /// </summary>
    public DateTime VietNamNow
    {
        get
        {
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                                                ? "SE Asia Standard Time" // Windows.
                                                                : "Asia/Ho_Chi_Minh"; // Linux.
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            return vietnamTime;
        }
    }
}