using Microsoft.AspNetCore.Components;

namespace Blazor.Components.BottomSheet.Components;

public partial class BottomSheetTemplate
{
    [Parameter] public RenderFragment? FixedContent { get; set; }
    [Parameter] public RenderFragment? Content { get; set; }

    private readonly Guid _elementId = Guid.NewGuid();

    private string _contentStyle =>
        "pointer-events:none;" +
        "touch-action: none;" +
        "width: 100%;" +
        "max-width: 100%;" +
        "height: 100%;" +
        "position:fixed;" +
        "margin-left: auto;" +
        "margin-right: auto;" +
        "box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);" +
        "border-radius: 0.5rem;" +
        "background-color: white;" +
        "z-index:auto;";

    private string _contentFramePadding =>
        "padding: 0.75rem;" +
        "height:100%;" +
        "display: flex;" +
        "flex-direction: column;";

    private string _contentFixedStyle =>
        "pointer-events:auto;" +
        "padding-bottom: 0.75rem;";

    private string _contentScrollableStyle =>
        "pointer-events:auto;" +
        "touch-action: pan-y;" +
        "overscroll-behavior: none;" +
        "overflow-y: auto;" +
        "overflow-x: hidden;"+
        "width:100%;" +
        "max-width:100%;" +
        "height:100%;";
}
