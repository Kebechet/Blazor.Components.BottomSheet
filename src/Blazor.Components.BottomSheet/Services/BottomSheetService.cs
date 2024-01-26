using Blazor.Components.BottomSheet.Components;
using Blazor.Components.BottomSheet.Interfaces;
using Microsoft.AspNetCore.Components;
using SatisFIT.Shared.Extensions;

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
            throw new InvalidOperationException("BottomSheetContainer is not initialized");
        }

        var renderFragment = await _bottomSheetContainer.Show(componentToRender);
        await _bottomSheetContainer.WaitForHide(renderFragment);
    }

    public async Task<TOutput?> Show<TOutput>(IBottomSheetReturnable<TOutput> componentToRender)
    {
        if (_bottomSheetContainer is null)
        {
            throw new InvalidOperationException("BottomSheetContainer is not initialized");
        }

        var cancellationTokenSource = new CancellationTokenSource();

        var taskCompletionSource = new TaskCompletionSource<TOutput>();
        cancellationTokenSource.Token.Register(() => taskCompletionSource.TrySetCanceled());

        componentToRender.OnReturnValue = new EventCallback<TOutput>(null, new Action<TOutput>((arg) =>
        {
            taskCompletionSource.SetResult(arg);
        }));

        await _bottomSheetContainer.Show(
            componentToRender,
            taskCompletionSource.Task,
            cancellationTokenSource);

        TOutput? returnValue;
        try
        {
            returnValue = await taskCompletionSource.Task;
            if (!typeof(TOutput?).IsValueType && returnValue is not null)
            {
                //for som reasom after calling `_bottomSheetContainer.Hide` the original value in `returnValue` is disposed. So I create a copy of it.
                returnValue = returnValue.DeepCopy();
            }
        }
        catch (OperationCanceledException)
        {
            returnValue = default;
        }

        await _bottomSheetContainer.Hide();

        return returnValue;
    }

    public async Task Hide()
    {
        if (_bottomSheetContainer is null)
        {
            throw new InvalidOperationException("BottomSheetContainer is not initialized");
        }

        await _bottomSheetContainer.Hide();
    }
}