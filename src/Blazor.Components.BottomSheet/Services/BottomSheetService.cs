using Blazor.Components.BottomSheet.Components;
using Blazor.Components.BottomSheet.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.BottomSheet.Services;

public class BottomSheetService
{
    private BottomSheetContainer? _bottomSheetContainer = null;

    public void Initialize(BottomSheetContainer bottomSheetContainer)
    {
        _bottomSheetContainer = bottomSheetContainer;
    }

    public async Task Show(IBottomSheet componentToRender)
    {
        if (_bottomSheetContainer is null)
        {
            throw new InvalidOperationException("PopupWrapper is not initialized");
        }

        var renderFragment = await _bottomSheetContainer.Show(componentToRender);
        await _bottomSheetContainer.WaitForHide(renderFragment);
    }

    public async Task<TOutput?> Show<TOutput>(IBottomSheetReturnable<TOutput> componentToRender)
    {
        if (_bottomSheetContainer is null)
        {
            throw new InvalidOperationException("PopupWrapper is not initialized");
        }

        var _cancellationTokenSource = new CancellationTokenSource();

        var taskCompletionSource = new TaskCompletionSource<TOutput>();
        _cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

        componentToRender.OnReturnValue = new EventCallback<TOutput?>(null, new Action<TOutput>((arg) =>
        {
            taskCompletionSource.SetResult(arg);
        }));

        await _bottomSheetContainer.Show(componentToRender);

        TOutput? returnValue;
        try
        {
            returnValue = await taskCompletionSource.Task;
        }
        catch (OperationCanceledException)
        {
            returnValue = default;
        }

        await _bottomSheetContainer.Hide(false);

        return returnValue;
    }
}