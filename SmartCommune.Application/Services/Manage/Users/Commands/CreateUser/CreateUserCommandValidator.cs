using FluentValidation;

namespace SmartCommune.Application.Services.Manage.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
            .Length(3, 20).WithMessage("{PropertyName} phải có độ dài từ {MinLength} đến {MaxLength} kí tự")
            .Matches(@"^[a-zA-Z0-9!@#$%^&*()_+{}\[\]:;<>,.?/~`-]{3,20}$")
                .WithMessage("{PropertyName} chỉ bao gồm chữ hoặc số")
            .WithName("Tên tài khoản");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
            .Length(6, 16).WithMessage("{PropertyName} phải có độ dài từ {MinLength} đến {MaxLength} kí tự")
            .Matches(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>/?]).{6,16}$")
                .WithMessage("{PropertyName} phải bao gồm ít nhất một chữ cái, một số và ký tự đặc biệt")
            .WithName("Mật khẩu");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
            .MaximumLength(50).WithMessage("{PropertyName} không được vượt quá {MaxLength} kí tự")
            .WithName("Họ và tên");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("{PropertyName} là bắt buộc.")
            .WithName("Vai trò");
    }
}