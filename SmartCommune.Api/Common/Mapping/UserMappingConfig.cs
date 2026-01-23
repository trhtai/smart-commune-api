using Mapster;

using SmartCommune.Application.Services.Identity.MenuItems;
using SmartCommune.Constracts.User.MenuItems;

namespace SmartCommune.Api.Common.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<MenuItemResult, MenuItemResponse>()
            .Map(dest => dest.Title, src => src.Label)
            .Map(dest => dest.To, src => src.RouterLink)
            .Map(dest => dest.Children, src => src.Children);
    }
}