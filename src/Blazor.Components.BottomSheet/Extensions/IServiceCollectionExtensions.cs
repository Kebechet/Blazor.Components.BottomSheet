using Microsoft.Extensions.DependencyInjection;
using SatisFIT.Client.App.Services;

namespace Blazor.Components.Popup.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBottomSheetServices(this IServiceCollection services)
    {
        services.AddSingleton<BottomSheetService>();
        return services;
    }
}
