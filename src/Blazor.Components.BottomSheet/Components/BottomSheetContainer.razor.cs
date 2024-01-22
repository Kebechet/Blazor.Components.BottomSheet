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

    private double _opacity = 0.0;
    private double _blur = 10;
    private double _transitionDelaySeconds => _transitionDelayMilliseconds / 1000.0;
    private int _transitionDelayMilliseconds = 300;

    private string _wrapperStyle =>
        "position: fixed;" +
        "z-index: 1100;" +
        "top: 0;" +
        "left: 0;" +
        "height: 100%;" +
        "width: 100%;" +
        $"background-color: rgba(0, 0, 0, {_opacity.ToString(CultureInfo.InvariantCulture)});" +
        $"backdrop-filter: blur({_blur.ToString(CultureInfo.InvariantCulture)}px);" +
        "display: flex;" +
        "flex-direction: column;" +
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

    public async Task<RenderFragment> Show(IBottomSheet bottomSheet)
    {
        if (_bottomSheetData.Any())
        {
            await ChangeVisiblity(false, _transitionDelayMilliseconds);
        }

        var bottomSheetData = new BottomSheetData
        {
            RenderFragment = bottomSheet.CreateRenderFragmentFromInstance(),
            OnBeforeHide = bottomSheet.CanHideBottomSheet
        };

        _bottomSheetData.Add(bottomSheetData);

        await InvokeAsync(StateHasChanged);

        await ChangeVisiblity(true, _transitionDelayMilliseconds);

        return bottomSheetData.RenderFragment;
    }

    public async Task Hide(bool isManualClose)
    {
        var bottomSheetData = _bottomSheetData.Last();

        if (isManualClose)
        {
            var canHide = await bottomSheetData.OnBeforeHide();
            if (!canHide)
            {
                return;
            }
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
            Hide(true).Wait();
        }
    }
}
