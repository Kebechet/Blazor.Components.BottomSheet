using Microsoft.AspNetCore.Components;

namespace SatisFIT.Client.App.Pages._PageComponents.BottomSheets;

public interface IBottomSheetReturnable<T> : IBottomSheet
{
    public EventCallback<T?> OnReturnValue { get; set; }
}
