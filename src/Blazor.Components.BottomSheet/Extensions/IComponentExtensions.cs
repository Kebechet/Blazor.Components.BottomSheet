using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;

namespace Blazor.Components.BottomSheet.Extensions;

internal static class IComponentExtensions
{
    internal static RenderFragment CreateRenderFragmentFromInstance(this IComponent instance, IServiceProvider serviceProvider)
    {
        var instanceType = instance.GetType();

        int attributeNumber = 0;

        // Inject services into properties marked with [Inject]
        var injectPropertyInfos = instanceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.GetCustomAttributes(typeof(InjectAttribute), true).Any());

        foreach (var propertyInfo in injectPropertyInfos)
        {
            if (propertyInfo.CanWrite)
            {
                var service = serviceProvider.GetService(propertyInfo.PropertyType);
                if (service != null)
                {
                    propertyInfo.SetValue(instance, service);
                }
            }
        }

        return builder =>
        {
            builder.OpenComponent(attributeNumber++, instanceType);
            builder.SetKey(instance.GetHashCode());

            var propertyInfos = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                builder.AddAttribute(instance, attributeNumber++, propertyInfo, serviceProvider);
            }

            builder.CloseComponent();
        };
    }

    private static void AddAttribute(this RenderTreeBuilder builder, IComponent instance, int sequence, PropertyInfo propertyInfo, IServiceProvider serviceProvider)
    {
        if (propertyInfo.PropertyType == typeof(EventCallback))
        {
            var originalCallback = (EventCallback)propertyInfo.GetValue(instance)!;

            var wrappedCallback = new EventCallback(null, new Func<object, Task>(async (arg) =>
            {
                if (originalCallback.HasDelegate)
                {
                    await originalCallback.InvokeAsync(arg);
                }
            }));

            builder.AddAttribute(sequence, propertyInfo.Name, wrappedCallback);
        }
        else
        {
            builder.AddAttribute(sequence, propertyInfo.Name, propertyInfo.GetValue(instance));
        }
    }
}
