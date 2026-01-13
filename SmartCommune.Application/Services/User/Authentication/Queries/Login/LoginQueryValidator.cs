using FluentValidation;

namespace SmartCommune.Application.Services.User.Authentication.Queries.Login;

public class LoginQueryValidator : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .MaximumLength(100).WithMessage("{PropertyName} không được vượt quá {MaxLength} ký tự.")
            .WithName("Tên đăng nhập");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .WithName("Mật khẩu");
    }
}