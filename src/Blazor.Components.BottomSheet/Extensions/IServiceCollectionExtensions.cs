using Blazor.Components.BottomSheet.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Components.BottomSheet.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddBottomSheetServices(this IServiceCollection services)
    {
        services.AddSingleton<BottomSheetService>();
        return services;
    }
}
