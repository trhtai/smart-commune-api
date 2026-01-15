using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Common.Interfaces.Persistence;

namespace SmartCommune.Api.Middlewares;

public class SecurityStampMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        // 1. Chỉ kiểm tra nếu user đã authenticate (có Access Token hợp lệ về mặt chữ ký)
        // Nếu chưa login thì có thể Client đang gọi các api không cần Authenticate chẳng hạn => cho qua.
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tokenSecurityStamp = context.User.FindFirst(CustomClaims.SecurityStamp)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(tokenSecurityStamp))
            {
                // Sử dụng Scope để lấy DbContext (vì Middleware là Singleton/Transient còn DbContext là Scoped).
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                // 2. Lấy SecurityStamp mới nhất từ DB
                // LƯU Ý: Đoạn này sẽ query DB mỗi request.
                // Sau này áp dụng Redis, bạn sẽ lấy từ Redis thay vì query DB -> Hiệu năng cực nhanh.
                var userSecurityStamp = await dbContext.Users
                    .Where(u => u.Id.Value == Guid.Parse(userId))
                    .Select(u => u.SecurityStamp)
                    .FirstOrDefaultAsync();

                // 3. So sánh
                if (userSecurityStamp.ToString() != tokenSecurityStamp)
                {
                    // Nếu khác nhau -> Token cũ không còn giá trị -> Trả về 401.
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { error = "Vui lòng đăng nhập lại!" });

                    return; // Ngắt request tại đây.
                }
            }
        }

        await next(context);
    }
}