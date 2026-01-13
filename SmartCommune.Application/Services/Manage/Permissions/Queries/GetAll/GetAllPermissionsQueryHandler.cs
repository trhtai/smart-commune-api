using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Services.Manage.Permissions.Common;

namespace SmartCommune.Application.Services.Manage.Permissions.Queries.GetAll;

public class GetAllPermissionsQueryHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<GetAllPermissionsQuery, ErrorOr<List<PermissionResult>>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;

    public async Task<ErrorOr<List<PermissionResult>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
    {
        var permissions = await _dbContext.Permissions
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new List<PermissionResult>(
            permissions.Select(permission => new PermissionResult(
                permission.Id.Value,
                permission.Code,
                permission.Name)));
    }
}