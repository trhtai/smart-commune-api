using System.Runtime.InteropServices;

using SmartCommune.Application.Common.Interfaces.Services;

namespace SmartCommune.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime VietNamNow
    {
        get
        {
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                                                                ? "SE Asia Standard Time" // Windows
                                                                : "Asia/Ho_Chi_Minh"; // Linux
            var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);

            return vietnamTime;
        }
    }
}