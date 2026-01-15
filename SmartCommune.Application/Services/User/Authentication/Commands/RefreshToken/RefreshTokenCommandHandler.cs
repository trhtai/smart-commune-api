using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SmartCommune.Application.Common.Interfaces.Authentication;
using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Application.Common.Options;
using SmartCommune.Application.Services.User.Authentication.Common;
using SmartCommune.Domain.Common.Errors;

namespace SmartCommune.Application.Services.User.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    IJwtTokenGenerator jwtTokenGenerator,
    IDateTimeProvider dateTimeProvider,
    IOptions<JwtSettings> jwtSettingsOption)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly JwtSettings _jwtSettings = jwtSettingsOption.Value;

    public async Task<ErrorOr<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm User sở hữu Refresh Token.
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.Permissions)
                .ThenInclude(p => p.Permission) // Load permission để gen token mới (sẽ xóa khi áp dụng redis).
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.RefreshToken), cancellationToken);

        if (user is null)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        var existingRefreshToken = user.RefreshTokens.Single(t => t.Token == request.RefreshToken);

        // 2. (Quan trọng) Kiểm tra token có active không, nếu còn => chưa bị revoke.
        if (!existingRefreshToken.IsActive(_dateTimeProvider.VietNamNow))
        {
            // Token đã bị dùng rồi hoặc hết hạn nhưng lại được gửi lên server:
            // -> Nghi vấn hack hay token đã bị đánh cắp.
            // -> Có thể revoke toàn bộ token của user này để bảo mật.
            return Errors.Authentication.InvalidCredentials;
        }

        // 3. Generate cặp Token mới.
        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // 4. Thêm Refresh Token.
        user.AddRefreshToken(
            newRefreshToken,
            _jwtSettings.RefreshTokenExpiryDays,
            _dateTimeProvider.VietNamNow);

        // 5. Save Changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            user.Id.Value,
            user.FullName,
            newAccessToken,
            newRefreshToken);
    }
}