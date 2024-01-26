using System.Text.Json;

namespace SatisFIT.Shared.Extensions;

internal static class ObjectExtensions
{
    internal static T DeepCopy<T>(this T objSource)
    {
        var jsonString = JsonSerializer.Serialize(objSource);
        return JsonSerializer.Deserialize<T>(jsonString)!;
    }
}
