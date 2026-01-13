using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Application.Common.Options;
using SmartCommune.Application.Services.User.Authentication.Queries.Login;
using SmartCommune.Contracts.User.Authentication;

namespace SmartCommune.Api.Controllers.User;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(
    ISender sender,
    IMapper mapper,
    IDateTimeProvider dateTimeProvider,
    IOptions<JwtSettings> jwtSettingsOption)
    : BaseController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly JwtSettings _jwtSettings = jwtSettingsOption.Value;

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var loginQuery = _mapper.Map<LoginQuery>(loginRequest);
        var result = await _sender.Send(loginQuery, HttpContext.RequestAborted);

        if (result.IsError)
        {
            return HandleProblem(result.Errors);
        }

        var authenticationResult = result.Value;

        SetRefreshTokenCookie(authenticationResult.RefreshToken);

        return Ok(_mapper.Map<AuthenticationResponse>(authenticationResult));
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Quan trọng: JS không đọc được, dù Hacker có chèn được script vào hệ thống thì cũng không lấy được Refresh Token
            Expires = _dateTimeProvider.VietNamNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            SameSite = SameSiteMode.Strict, /* Chống CSRF, ngăn trình duyệt gửi cookie trong các yêu cầu bên thứ 3.
                                            Bắt buộc khi Frontend và Backend khác domain */
            Secure = true, // Chỉ chạy trên HTTPS.
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}