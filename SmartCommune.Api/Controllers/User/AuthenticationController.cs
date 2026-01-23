using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Application.Common.Options;
using SmartCommune.Application.Services.User.Authentication.Commands.RefreshToken;
using SmartCommune.Application.Services.User.Authentication.Commands.RevokeToken;
using SmartCommune.Application.Services.User.Authentication.Queries.Login;
using SmartCommune.Constracts.User.Authentication;

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
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var query = _mapper.Map<LoginQuery>(request);
        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return result.Match(
            auth =>
            {
                SetRefreshTokenCookie(auth.RefreshToken);
                return Ok(_mapper.Map<AuthenticationResponse>(auth));
            },
            HandleProblem);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var command = new RefreshTokenCommand(refreshToken);
        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return result.Match(
            auth => Ok(_mapper.Map<AuthenticationResponse>(auth)),
            HandleProblem);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var command = new RevokeTokenCommand(refreshToken);
        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return result.Match(
            success =>
            {
                Response.Cookies.Delete("refreshToken");
                return NoContent();
            },
            HandleProblem);
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