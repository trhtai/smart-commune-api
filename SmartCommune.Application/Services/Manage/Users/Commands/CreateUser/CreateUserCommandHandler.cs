using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.Common.Errors;
using SmartCommune.Domain.RoleAggregate.ValueObjects;
using SmartCommune.Domain.UserAggregate;

namespace SmartCommune.Application.Services.Manage.Users.Commands.CreateUser;

public class CreateUserCommandHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateUserCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<ErrorOr<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Kiểm tra username đã tồn tại chưa?
        var userNameExists = await _dbContext.Users.AnyAsync(u => u.UserName == request.UserName, cancellationToken);
        if (userNameExists)
        {
            return Errors.User.DuplicateUserName;
        }

        // Kiểm tra Role có tồn tại không?
        var roleId = RoleId.Create(request.RoleId);
        var roleExists = await _dbContext.Roles.AnyAsync(r => r.Id == roleId, cancellationToken);
        if (!roleExists)
        {
            return Errors.Role.NotFound;
        }

        // Tạo user.
        var user = ApplicationUser.Create(
            request.UserName,
            request.Password,
            request.FullName,
            _dateTimeProvider.VietNamNow,
            roleId);

        // Lưu vào db.
        await _dbContext.Users.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user.Id.Value;
    }
}