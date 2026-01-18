
using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Domain.Common.Errors;
using SmartCommune.Domain.UserAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Manage.Users.Commands.UnlockUser;

public class UnlockUserCommandHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<UnlockUserCommand, ErrorOr<Unit>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Unit>> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
    {
        var userId = ApplicationUserId.Create(request.UserId);
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Errors.User.NotFound;
        }

        user.UnlockAccount();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}