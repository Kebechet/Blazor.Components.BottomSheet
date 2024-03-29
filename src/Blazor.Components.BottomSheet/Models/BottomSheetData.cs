﻿using Microsoft.AspNetCore.Components;

namespace Blazor.Components.BottomSheet.Models;

public class BottomSheetData
{
    public required RenderFragment RenderFragment { get; set; }
    public required Func<Task<bool>> OnBeforeHide { get; set; }
    public required Task? Task { get; set; }
    public required CancellationTokenSource? CancellationTokenSource { get; set; }
}
