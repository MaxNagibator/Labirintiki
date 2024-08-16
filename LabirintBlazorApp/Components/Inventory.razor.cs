using System.Collections.ObjectModel;
using Labirint.Core.Stacks;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class Inventory
{
    [Parameter]
    public required ObservableCollection<ItemStack> Stacks { get; set; }
}
