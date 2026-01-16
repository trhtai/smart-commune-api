using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Authentication;
using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Application.Services.User.Authentication.Common;
using SmartCommune.Domain.Common.Errors;

namespace SmartCommune.Application.Services.User.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IApplicationDbContext dbContext,
    IJwtTokenGenerator jwtTokenGenerator,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<RefreshTokenCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

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
            // REFRESH TOKEN RETATION.
            // Token đã bị dùng rồi hoặc hết hạn nhưng lại được gửi lên server:
            // -> Nghi vấn hack hay token đã bị đánh cắp.
            // -> Có thể revoke toàn bộ token của user này để bảo mật (Thà giết nhầm còn hơn bỏ sót).
            // user.RevokeAllRefreshTokens(_dateTimeProvider.VietNamNow);
            // Kết hợp thêm SecurityStamp.
            // user.RefreshSecurityStamp();
            // await _dbContext.SaveChangesAsync(cancellationToken);

            // Ở flow bình thường thì việc refresh token hết hạn mà vẫn gửi lên là bình thường,
            // Vì hết hạn nên mới xin cái mới.
            // Đơn giản trả về 401 và bắt User login lại.
            return Errors.Authentication.InvalidCredentials;
        }

        // 3. Generate cặp Token mới.
        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user);

        return new AuthenticationResult(
            user.Id.Value,
            user.FullName,
            newAccessToken,
            request.RefreshToken);
    }
}