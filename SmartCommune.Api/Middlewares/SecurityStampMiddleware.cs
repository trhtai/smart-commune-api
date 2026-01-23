using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Application.Common.Options;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Api.Middlewares;

public class SecurityStampMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IServiceProvider serviceProvider,
        ICacheService cacheService,
        IOptions<JwtSettings> jwtSettingsOption)
    {
        var jwtSettings = jwtSettingsOption.Value;
        var logger = serviceProvider.GetRequiredService<ILogger<SecurityStampMiddleware>>();

        // 1. Chỉ kiểm tra nếu user đã authenticate (có Access Token hợp lệ về mặt chữ ký)
        // Nếu chưa login thì có thể Client đang gọi các api không cần Authenticate chẳng hạn => cho qua.
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tokenSecurityStamp = context.User.FindFirst(CustomClaims.SecurityStamp)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(tokenSecurityStamp))
            {
                // Lấy SecurityStamp từ Redis trước.
                string cacheKey = $"auth:security_stamp:{userId}";
                string? dbSecurityStamp = null;

                dbSecurityStamp = await cacheService.GetAsync<string>(cacheKey);

                if (dbSecurityStamp is null)
                {
                    // Sử dụng Scope để lấy DbContext (vì Middleware là Singleton/Transient còn DbContext là Scoped).
                    using var scope = serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                    // 2. Lấy SecurityStamp mới nhất từ DB
                    var user = await dbContext.Users
                        .Where(u => u.Id == ApplicationUserId.Create(Guid.Parse(userId)))
                        .Select(u => new { u.SecurityStamp })
                        .FirstOrDefaultAsync();

                    if (user is null)
                    {
                        context.Response.StatusCode = 401;
                        return;
                    }

                    dbSecurityStamp = user.SecurityStamp.ToString();

                    // 3. Có dữ liệu rồi thì thử lưu lại vào Redis (nếu Redis sống lại)
                    // Lưu vào Redis(TTL ngắn, ví dụ 15 phút, trùng với thời gian Access Token).
                    await cacheService.SetAsync(
                        cacheKey,
                        dbSecurityStamp,
                        TimeSpan.FromMinutes(jwtSettings.ExpiryMinutes));
                }

                // 3. So sánh.
                if (dbSecurityStamp != tokenSecurityStamp)
                {
                    // Nếu khác nhau -> Token cũ không còn giá trị.
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Vui lòng đăng nhập lại!" });

                    return; // Ngắt request tại đây.
                }
            }
        }

        await next(context);
    }
}