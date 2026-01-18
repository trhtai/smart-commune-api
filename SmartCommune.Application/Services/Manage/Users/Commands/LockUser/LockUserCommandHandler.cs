using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.Common.Errors;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Manage.Users.Commands.LockUser;

public class LockUserCommandHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<LockUserCommand, ErrorOr<Unit>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<ErrorOr<Unit>> Handle(LockUserCommand request, CancellationToken cancellationToken)
    {
        var userId = ApplicationUserId.Create(request.UserId);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Errors.User.NotFound;
        }

        user.LockAccount(_dateTimeProvider.VietNamNow);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}