# Blazor.Components.Popup

This repo contain `PopupWrapper` component. It's purpose is to simplify Popup usage in Blazor.

## Setup
- Install nuget package `Kebechet.Blazor.Components.Popup`
- In your `Program.cs` add:
```cs
builder.Services.AddPopupWrapperServices();
```

## Usage
- Ideally in `MainLayout.razor` put `<PopupWrapper />` component. This component is controlled from `PopupService` and it's purpose is to render popup content.
    - ⚠️ At one time only 1 popup can be rendered.
- In component where you would like to render popup inject Popup service like:
```cs
@inject PopupWrapperService _popupWrapperService
```

- Create Popup content component. E.g. `YesNoPopup.razor` that implements interface `IPopupable<T>` with return type you need to get from the Popup. This popup content will be rendered
inside `PopupWrapper`.
    - ⚠️ I recommend to always use nullable type like `IPopupable<bool?>` because that way you can differentiate between person clicking `NO`/`False` and person closing the popup be clicking 
    outside of it.
    - ⚠️ The child component must have `[Parameter]` at the beginning of the `OnReturn` property, otherwise it won't work.

`YesNoPopup.razor` part

```cs
@using Blazor.Components.Popup.Components;

<span>Do you want to save changes ?</span>
<button @onclick="()=>Process(true)">Yes, I do</button>
<button @onclick="()=>Process(false)">No, I don't</button>

```

`YesNoPopup.razor.cs` part

```cs
using Blazor.Components.Popup.Components;
using Microsoft.AspNetCore.Components;

namespace BlazorApp7.Pages;

public partial class YesNoComponent : IPopupable<bool?>
{
    [Parameter] public EventCallback<bool?> OnReturn { get; set; }

    public async Task Process(bool val)
    {
        await OnReturn.InvokeAsync(val);
    }
}
```

- then in your component where you would like to render popup:
  - inject `PopupWrapperService `
  - add functionality to trigger popup. In our case button `onClick` event
  - and finally use `Show` method to show `YesNoPopup` and get result from it

`YourComponent.razor` 

```cs
@inject PopupWrapperService _popupWrapperService

<button @onclick="Test">Trigger popup</button>

@code{
    public async Task Test()
    {
        var isSuccess =  await _popupWrapperService.Show(new YesNoPopup(), this);
        if (isSuccess is null)
        {
            // user closed the popup
        } 
        else if(isSuccess == true)
        {
            // user clicked the button that returns true
        } 
        else if(isSuccess == false){
            // user clicked the button that returns false
        }
    }
}
```

- ENJOY 🎉

## Future tasks:
- [ ] Add support for multiple popups at the same time
    - Probably through ref like: https://demos.blazorbootstrap.com/confirm-dialog
- [ ] Consider using `Dialog` HTML element instead of `div`
    - https://www.youtube.com/watch?v=q1fsBWLpYW4&t=33s