using SmartCommune.Domain.UserAggregate;

namespace SmartCommune.Application.Common.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
}