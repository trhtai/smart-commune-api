using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

using SmartCommune.Api.Common.Constants;
using SmartCommune.Api.Common.Mapping;
using SmartCommune.Api.Configurations;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartCommune.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        ConfigurationManager configuration,
        IWebHostEnvironment env)
    {
        // SWAGGER.
        if (env.IsDevelopment())
        {
            services.AddSwaggerGen();
            services.AddTransient<
                IConfigureOptions<SwaggerGenOptions>,
                ConfigureSwaggerOptions>();
        }

        // CONTROLLERS.
        services.AddControllers(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        });

        // CORS.
        services.AddCorsPolicies(configuration);

        // AUTHORIZATION.
        // Các dịch vụ cần thiết để hệ thống có thể thực hiện việc Phân quyền.
        // Cho phép bạn xác định xem một người dùng (đã đăng nhập) có quyền truy cập vào
        // một tài nguyên cụ thể hay không (ví dụ: Admin mới được xóa bài viết).
        // Sau khi thêm dòng này, thường sẽ sử dụng Middleware app.UseAuthorization();
        // và các Attribute như [Authorize] hoặc [Authorize(Roles = "Admin")] trên các Controller/Endpoint.
        services.AddAuthorization();

        // HTTP CONTEXT ACCESSOR.
        // HttpContext: chứa thông tin về request hiện tại như Header, User, Query string,...
        // HttpContextAccessor cho phép truy cập thẳng HttpContext từ các class nằm ngoài Controller.
        // Ví dụ: CurrentUserProvider hiện có trong hệ thống.
        services.AddHttpContextAccessor();

        // Mapping.
        services.AddMappings();

        return services;
    }

    public static IServiceCollection AddCorsPolicies(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        if (origins is null || origins.Length == 0)
        {
            throw new InvalidOperationException("Thiếu danh sách Cors Origin.");
        }

        services.AddCors(options =>
        {
            // Lưu ý về AllowCredentials:
            // - Do đã cấu hình danh sách domain rõ ràng nên việc sử dụng AllowCredentials là an toàn.
            // - AllowCredentials cho phép trình duyệt đính kèm cookie chứa refresh token và gửi đi.
            // - Nếu không có AllowCredentials Backend sẽ từ chối nhận Cookie từ 1 nguồn lạ.
            // - Nguồn lạ ở đây là từ 1 domain/sub-domain khác.
            options.AddPolicy(CorsPolicyNames.App, policy =>
            {
                policy.WithOrigins(origins!);
                policy.AllowCredentials();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod(); // Cho phép mọi phương thức HTTP (GET, POST, PUT, DELETE, PATCH, OPTIONS...).
                policy.WithExposedHeaders("Content-Disposition"); // Giúp frontend đọc được file từ header.
            });
        });

        return services;
    }
}