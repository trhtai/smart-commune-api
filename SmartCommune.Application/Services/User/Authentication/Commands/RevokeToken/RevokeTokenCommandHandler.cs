using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.Common.Errors;

namespace SmartCommune.Application.Services.User.Authentication.Commands.RevokeToken;

public class RevokeTokenCommandHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<RevokeTokenCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<ErrorOr<Success>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        // 0. Validate refresh token.
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Errors.Authentication.InvalidCredentials;
        }

        // 1. Tìm User chứa token.
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == request.RefreshToken), cancellationToken);

        if (user is null)
        {
            // Token không tồn tại, nhưng để bảo mật không nên báo lỗi chi tiết.
            // Trả về Success coi như đã revoke xong.
            return Result.Success;
        }

        // 2. Gọi Domain logic để tiến hành revoke.
        var result = user.RevokeRefreshToken(request.RefreshToken, _dateTimeProvider.VietNamNow);

        if (!result)
        {
            // Token đã hết hạn hoặc đã revoke trước đó.
            return Result.Success;
        }

        // 3. Lưu thay đổi.
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}