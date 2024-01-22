using Microsoft.AspNetCore.Components;

namespace Blazor.Components.BottomSheet.Interfaces;

public interface IBottomSheetReturnable<T> : IBottomSheet
{
    public EventCallback<T?> OnReturnValue { get; set; }
}
