using Blazor.Components.BottomSheet.Components;
using Microsoft.AspNetCore.Components;

namespace SatisFIT.Client.App.Pages._PageComponents.BottomSheets;

public interface IBottomSheet : IComponent
{
    public BottomSheet? BottomSheetComponent { get; set; }
    public Task<bool> CanHideBottomSheet()
    {
        return Task.FromResult(true);
    }
}
