using Mapster;

using SmartCommune.Application.Services.Manage.Permissions.Common;
using SmartCommune.Contracts.Manage.Permissions;

namespace SmartCommune.Api.Common.Mapping;

public class ManageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // PermissionsController.
        config.NewConfig<PermissionResult, PermissionResponse>();
    }
}