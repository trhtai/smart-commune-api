using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Authentication;
using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Application.Services.Identity.MenuItems;
using SmartCommune.Application.Services.Identity.Permissions;
using SmartCommune.Application.Services.User.Authentication.Common;
using SmartCommune.Domain.Common.Errors;

namespace SmartCommune.Application.Services.User.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    IJwtTokenGenerator jwtTokenGenerator,
    IDateTimeProvider dateTimeProvider,
    IPermissionService permissionService,
    IMenuItemService menuItemService)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly IPermissionService _permissionService = permissionService;
    private readonly IMenuItemService _menuItemService = menuItemService;

    public async Task<ErrorOr<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm User sở hữu Refresh Token.
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.RefreshToken), cancellationToken);

        if (user is null)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        var existingRefreshToken = user.RefreshTokens.Single(t => t.Token == request.RefreshToken);

        // 2. (Quan trọng) Kiểm tra token có active không, nếu không => nghi vấn hack.
        if (!existingRefreshToken.IsActive())
        {
            // Revoke toàn bộ token của user này để bảo mật.
            user.RevokeAllRefreshTokens(_dateTimeProvider.VietNamNow);

            // Kết hợp thêm SecurityStamp.
            user.RefreshSecurityStamp();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Errors.Authentication.InvalidCredentials;
        }

        // 3. Kiểm tra token hết hạn.
        // Ở flow bình thường thì việc refresh token hết hạn mà vẫn gửi lên là bình thường,
        // Vì hết hạn nên mới xin cái mới.
        // Đơn giản trả về 401 và bắt User login lại.
        // Nếu token hết hạn thì sẽ bị Revoke ngay, nếu Refresh Token lại được gửi lên để
        // xin Access Token 1 lần nữa thì sẽ bị chặn ở bước 2.
        if (existingRefreshToken.IsExpired(_dateTimeProvider.VietNamNow))
        {
            user.RevokeRefreshToken(existingRefreshToken.Token, _dateTimeProvider.VietNamNow);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Errors.Authentication.InvalidCredentials;
        }

        // 4. Generate Token mới.
        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);

        // 5. Lấy Permissions mới nhất
        var permissions = await _permissionService.GetPermissionsAsync(user.RoleId, cancellationToken);

        // 6. Lấy Menu mới nhất (đã cắt tỉa)
        var menu = await _menuItemService.GetMenuAsync(user.RoleId, cancellationToken);

        return new AuthenticationResult(
            user.Id.Value,
            user.FullName,
            newAccessToken,
            request.RefreshToken,
            permissions.ToList(),
            menu);
    }
}