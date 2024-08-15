using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;

namespace LabirintBlazorApp.Components;

public partial class Inventory
{
    [Parameter]
    public required ObservableCollection<ItemStack> Stacks { get; set; }
}
