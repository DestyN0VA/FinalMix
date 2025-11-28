using StardewValley;
using System.Collections.ObjectModel;

namespace FinalMix.Menus;
public class InventoryContext
{
    public ObservableCollection<Tuple<Item, bool>> Items { get; set; } = [.. Game1.player.Items.Select(i => new Tuple<Item, bool>(i, ShowSprite(i)))];

    public static bool ShowSprite(Item i) => i != null;
}
