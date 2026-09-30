using System.Reflection;

namespace Labirint.Core.Items.Base;

/// <summary>
/// Каталог предметов: собирает наследников Item из зарегистрированных сборок.
/// </summary>
public static class ItemCatalog
{
    private static readonly List<Assembly> Sources = [typeof(Item).Assembly];

    private static Item[]? _items;

    /// <summary>
    /// Все предметы игры.
    /// </summary>
    public static IReadOnlyList<Item> Items => _items ??= Build();

    /// <summary>
    /// Добавить сборку с предметами: инвентари, созданные раньше, о новых предметах не узнают.
    /// </summary>
    /// <param name="assembly">Сборка с наследниками Item.</param>
    public static void AddSource(Assembly assembly)
    {
        if (Sources.Contains(assembly))
        {
            return;
        }

        Sources.Add(assembly);
        _items = null;
    }

    private static Item[] Build()
    {
        return Sources
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(Item)) && type.IsAbstract == false)
            .Select(type => (Item)Activator.CreateInstance(type)!)
            .OrderByDescending(item => item.ControlSettings != null)
            .ThenByDescending(item => item.MaxCount)
            .ToArray();
    }
}
