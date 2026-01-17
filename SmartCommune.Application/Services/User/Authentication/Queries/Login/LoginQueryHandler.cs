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

namespace SmartCommune.Application.Services.User.Authentication.Queries.Login;

public class LoginQueryHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    IOptions<JwtSettings> jwtSettingsOption)
    : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly JwtSettings _jwtSettings = jwtSettingsOption.Value;

    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        // 1. Tìm user theo UserName.
        var user = await _dbContext.Users
            .Include(u => u.RefreshTokens)
            .Include(u => u.Permissions)
                .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);

        if (user is null)
        {
            return Errors.Authentication.InvalidCredentials;
        }

        // 2. Kiểm tra user có bị khóa không?
        if (!user.IsActived || user.DisableAt is not null)
        {
            return Errors.Authentication.AccountLocked;
        }

        // 3. Validate Password.
        if (!user.PasswordHash.Verify(request.Password))
        {
            return Errors.Authentication.InvalidCredentials;
        }

        // 4. Generate Access Token (JWT).
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);

        // 5. Generate Refresh Token.
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        // 6. Lưu Refresh Token vào User.
        user.AddRefreshToken(refreshToken, _jwtSettings.RefreshTokenExpiryDays, _dateTimeProvider.VietNamNow);

        // 7. Lưu thay đổi vào DB.
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            user.Id.Value,
            user.FullName,
            accessToken,
            refreshToken);
    }
}