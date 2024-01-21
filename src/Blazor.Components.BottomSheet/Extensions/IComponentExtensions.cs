using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Blazor.Components.BottomSheet.Extensions;

public static class IComponentExtensions
{
    public static RenderFragment CreateRenderFragmentFromInstance(this IComponent instance)
    {
        int attributeNumber = 0;

        return builder =>
        {
            builder.OpenComponent(attributeNumber++, instance.GetType());
            builder.SetKey(instance.GetHashCode());

            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(EventCallback))
                {
                    var originalCallback = (EventCallback)property.GetValue(instance)!;

                    var wrappedCallback = new EventCallback(null, new Func<object, Task>(async (arg) =>
                    {
                        if (originalCallback.HasDelegate)
                        {
                            await originalCallback.InvokeAsync(arg);
                        }
                    }));

                    builder.AddAttribute(attributeNumber++, property.Name, wrappedCallback);
                }
                else
                {
                    builder.AddAttribute(attributeNumber++, property.Name, property.GetValue(instance));
                }
            }

            builder.CloseComponent();
        };
    }
}
