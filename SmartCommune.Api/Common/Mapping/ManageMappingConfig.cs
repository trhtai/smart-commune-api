using Mapster;

using SmartCommune.Application.Services.Manage.MenuItems.Common;
using SmartCommune.Constracts.Manage.MenuItems;

namespace SmartCommune.Api.Common.Mapping;

public class ManageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Menu Items.
        config.NewConfig<MenuItemManageResult, MenuItemsResponse>()
            .Map(dest => dest.Title, src => src.Label)
            .Map(dest => dest.To, src => src.RouterLink);
    }
}