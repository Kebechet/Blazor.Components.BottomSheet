# Blazor.Components.BottomSheet

This repo contain `BottomSheet` Blazor component.

## Setup
- Install nuget package `Kebechet.Blazor.Components.BottomSheet`
- In your `Program.cs` add:
```cs
builder.Services.AddBottomSheetServices();
```
- Ideally in `MainLayout.razor` put `<BottomSheetContainer />` component. This component is controlled from `BottomSheetService` and it's purpose is to render `BottomSheet` content.

## Usage
- In component where you would like to render `BottomSheet` inject service:
```cs
@inject BottomSheetService _bottomSheetService
```

- Create `BottomSheet` content component. E.g. `OptionBottomSheet.razor` that implements interface `IBottomSheet` in case you don't want to return any value from the `BottomSheet` or `IBottomSheetReturnable<T>` with return type `T`.
    - ⚠️ The child component must have `[Parameter]` at the beginning of the `OnReturnValue` property, otherwise it won't work.

`Index.razor` part

```cs
@page "/"
@using BlazorApp11.Components
@using SatisFIT.Client.App.Services
@inject BottomSheetService _bottomSheetService

<PageTitle>Home</PageTitle>

<button class="btn btn-primary" @onclick="Show">Click me</button>

<span>Result: @_selectedValue</span>

@code{
    public int _selectedValue { get; set; } = 0;

    public async Task Show()
    {
        var result = await _bottomSheetService.Show(
            new OptionBottomSheet()
            {
            }
        );

        _selectedValue = result;
    }
}
```

- then in your component where you would like to render `BottomSheet`:
  - inject `BottomSheetService `
  - add functionality to trigger `BottomSheet`. In our case button `onClick` event

`OptionBottomSheet.razor`

```
@using Blazor.Components.BottomSheet.Components
@using SatisFIT.Client.App.Services
@inject BottomSheetService _bottomSheetService

<BottomSheet @ref="BottomSheetComponent">
    <FixedContent>
        <span>This is header content</span>
    </FixedContent>

    <Content>
        <input placeholder="write here" />

        @for(int i = 0; i <= 30; i++)
        {
            var index = i;
            <button @onclick="() => ReturnValue(index)">Content btn @index</button>
            <br>
        }
    </Content>
</BottomSheet>
```

`OptionBottomSheet.razor.cd`
```cs
public partial class OptionBottomSheet : IBottomSheet
{
    [Parameter] public BottomSheet? BottomSheetComponent { get; set; }
    [Parameter] public EventCallback<int> OnReturnValue { get; set; }

    public async Task<bool> CanHideBottomSheet()
    {
        return await Task.FromResult(true);
    }

    public async Task ReturnValue(int number)
    {
        await OnReturnValue.InvokeAsync(number);
    }
}
```

- ENJOY 🎉
