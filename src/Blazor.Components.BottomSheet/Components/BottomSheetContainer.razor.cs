using Blazor.Components.BottomSheet.Extensions;
using Blazor.Components.BottomSheet.Interfaces;
using Blazor.Components.BottomSheet.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Globalization;

namespace Blazor.Components.BottomSheet.Components;

public partial class BottomSheetContainer : ComponentBase
{
    [Parameter] public EventCallback OnHide { get; set; }

    private List<BottomSheetData> _bottomSheetData { get; set; } = [];
    private bool IsVisible { get; set; }
    private bool _isChangingSlide { get; set; } = false;

    private double _opacity = 0.2;
    private double _blur = 2;
    private double _transitionDelaySeconds => _transitionDelayMilliseconds / 1000.0;
    private int _transitionDelayMilliseconds = 300;

    private string _darkAreaStyle =>
        "position: fixed;" +
        "pointer-events:none;" +
        "touch-action: none;" +
        "z-index: 1100;" +
        "top: 0;" +
        "left: 0;" +
        "height: 100%;" +
        "width: 100%;" +
        "max-width: 100%;" +
        $"background-color: rgba(0, 0, 0, {_opacity.ToString(CultureInfo.InvariantCulture)});" +
        $"-webkit-backdrop-filter: blur({_blur.ToString(CultureInfo.InvariantCulture)}px);" + //fix for iOS Safari
        $"backdrop-filter: blur({_blur.ToString(CultureInfo.InvariantCulture)}px);" +
        "display: flex;" +
        "flex-direction: column;" +
        "overflow-x: hidden;" +
        "transform-origin: top;" +
        (IsVisible
           ? "opacity: 1; " +
                "pointer-events: auto; " +
                $"transition: all {_transitionDelaySeconds.ToString(CultureInfo.InvariantCulture)}s ease; " +
                "transform: translateY(0%);" +
                "transform: scaleY(0.1);" 
           : "opacity: 0;" +
                "pointer-events: none; " +
                $"transition: all {_transitionDelaySeconds.ToString(CultureInfo.InvariantCulture)}s ease; " +
                "transform: translateY(100%);" +
                "transform: scaleY(1);"
        ) +
        (_isChangingSlide
            ? "animation-name: example;" +
                "animation-duration: 1.2s;" +
                "--from-width:10px; --to-width:20px;"
            : string.Empty
        );

    private string _contentStyle =>
        "position: fixed;" +
        "pointer-events:none;" +
        "touch-action: none;" +
        "z-index: 1100;" +
        "top:10%;" +
        "left: 0;" +
        "height: 90%;" +
        "width: 100%;" +
        "max-width: 100%;" +
        "display: flex;" +
        "flex-direction: column;" +
        "overflow-x: hidden;" +
        (IsVisible
           ? "opacity: 1; " +
                "pointer-events: auto; " +
                $"transition: all {_transitionDelaySeconds.ToString(CultureInfo.InvariantCulture)}s ease; " +
                "transform: translateY(0%);"
           : "opacity: 0;" +
                "pointer-events: none; " +
                $"transition: all {_transitionDelaySeconds.ToString(CultureInfo.InvariantCulture)}s ease; " +
                "transform: translateY(100%);"
        ) +
        (_isChangingSlide
            ? "animation-name: example;" +
                "animation-duration: 1.2s;" +
                "--from-width:10px; --to-width:20px;"
            : string.Empty
        );

    protected override void OnInitialized()
    {
        _bottomSheetService.Initialize(this);
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    public async Task<RenderFragment> Show(
        IBottomSheet bottomSheet,
        Task? task = default,
        CancellationTokenSource? cancellationTokenSource = default)
    {
        if (_bottomSheetData.Any())
        {
            await ChangeVisiblity(false, _transitionDelayMilliseconds);
        }

        var bottomSheetData = new BottomSheetData
        {
            RenderFragment = bottomSheet.CreateRenderFragmentFromInstance(_serviceProvider),
            OnBeforeHide = bottomSheet.CanHideBottomSheet,
            Task = task,
            CancellationTokenSource = cancellationTokenSource
        };

        _bottomSheetData.Add(bottomSheetData);

        await InvokeAsync(StateHasChanged);

        await ChangeVisiblity(true, _transitionDelayMilliseconds);

        return bottomSheetData.RenderFragment;
    }

    public async Task Hide()
    {
        var bottomSheetData = _bottomSheetData.LastOrDefault();
        if (bottomSheetData is null)
        {
            return;
        }

        if (!(bottomSheetData.Task?.IsCompleted ?? true))
        {
            var canHide = await bottomSheetData.OnBeforeHide();
            if (!canHide)
            {
                return;
            }

            bottomSheetData.CancellationTokenSource?.Cancel(false);
            return;
        }

        await ChangeVisiblity(false, _transitionDelayMilliseconds);

        _bottomSheetData.Remove(bottomSheetData);
        await InvokeAsync(StateHasChanged);

        if (_bottomSheetData.Any())
        {
            await ChangeVisiblity(true, _transitionDelayMilliseconds);
        }
    }

    public async Task WaitForHide(RenderFragment bottomSheetParentRenderFragment)
    {
        while (true)
        {
            if (!_bottomSheetData.Any(x => x.RenderFragment == bottomSheetParentRenderFragment))
            {
                break;
            }

            await Task.Delay(50);
        }
    }

    private async Task ChangeVisiblity(bool isVisible, int transitionDelayMilliseconds)
    {
        _transitionDelayMilliseconds = transitionDelayMilliseconds;
        IsVisible = isVisible;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(transitionDelayMilliseconds);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (IsVisible)
        {
            Hide().Wait();
        }
    }
}
