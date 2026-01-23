using FluentValidation;

namespace SmartCommune.Application.Services.Manage.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .WithName("Tiêu đề");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("{PropertyName} không được để trống.")
            .WithName("Type");
    }
}