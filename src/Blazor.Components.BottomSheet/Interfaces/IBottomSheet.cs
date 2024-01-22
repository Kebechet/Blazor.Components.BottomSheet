using Blazor.Components.BottomSheet.Components;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.BottomSheet.Interfaces;

public interface IBottomSheet : IComponent
{
    public BottomSheetTemplate? BottomSheetComponent { get; set; }
    public Task<bool> CanHideBottomSheet()
    {
        return Task.FromResult(true);
    }
}
