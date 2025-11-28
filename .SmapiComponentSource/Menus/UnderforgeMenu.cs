using FinalMix.Drops;
using PropertyChanged.SourceGenerator;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Object = StardewValley.Object;

namespace FinalMix.Menus;

public partial class UnderforgeMenuContext : INotifyPropertyChanged
{
    [Notify] private SlotData slot = new("Slot", (i) => (i is MeleeWeapon mw && !mw.isScythe()) || (i is Object o && Game1.objectData.TryGetValue(o.ItemId, out var data) && (data.CustomFields?.ContainsKey("ArmorValue") ?? false)));
    [Notify] private SlotData preview = new("Preview", (_) => false);
    [Notify] public SlotData materia1 = new("Materia1", materiaMatch);
    [Notify] public SlotData materia2 = new("Materia2", materiaMatch);
    [Notify] public SlotData materia3 = new("Materia3", materiaMatch);
    [Notify] public SlotData alloy = new("Alloy", (i) => (PureOreUtility.PureOreMappings.ContainsValue(i.QualifiedItemId) || i.QualifiedItemId == "(O)DN.SnS_AetheriteBar") && Game1.player.Items.CountId(i.QualifiedItemId) >= 25);
    [Notify] public SlotData keychain = new("Keychain", (i) => i is Object o && Game1.objectData.TryGetValue(o.ItemId, out var value) && (value.CustomFields?.ContainsKey("keychain") ?? false));
    [Notify] private bool choosing = false;

    [DependsOn(nameof(Slot))]
    public bool IsLLTK => Slot.Item?.QualifiedItemId.EqualsIgnoreCase("(W)DN.SnS_longlivetheking") ?? false;
    [DependsOn(nameof(Choosing), nameof(Slot))]
    public bool ShouldFocus => !Choosing && Slot.Item != null;
    [DependsOn(nameof(Choosing))]
    public SlotData? Focus => SlotData.GetById(ChosenID);

    private readonly static Func<Item, bool> materiaMatch = (i) => i.QualifiedItemId.StartsWith("(O)DN.SnS_") && i.ItemId.EndsWith("Materia");
    public ObservableCollection<SlotData> AvailableItems { get; set; } = [];
    public string? ChosenID = null;

    public void OpenChooseMenu(string ID)
    {
        var slot = SlotData.GetById(ID);
        if (slot == null)
            return;

        if (slot != Slot && !Slot.HasItem)
            return;

        int num = 0;
        AvailableItems = [..Game1.player.Items.Where(i => i != null && slot.ItemMatch(i)).Select(i =>
        {
            SlotData data = new($"Item{num}", (_) => false)
            {
                item = i.getOne()
            };
            data.UpdateState();
            num++;
            return data;
        })];

        AvailableItems.RemoveWhere(slot => AvailableItems.Any(s => s != slot && s.Item?.QualifiedItemId == slot.item?.QualifiedItemId));

        if (AvailableItems.Any())
        {
            ChosenID = slot.ID;
            Choosing = true;
        }
        else
            Game1.addHUDMessage(HUDMessage.ForCornerTextbox("No materias available"));
    }

    public void RemoveItem(string ID)
    {
        Choosing = false;
        var slot = SlotData.GetById(ID);
        if (slot == null || slot.Item == null)
            return;

        slot.item = null;
        slot.UpdateState();

        if (slot == Slot)
        {
            SlotData.RemoveAllItems();
            Keychain.Item = null;
            Keychain.UpdateState();
            UpdatePreview();
        }
    }

    public void ChooseItem(Item item)
    {
        var slot = SlotData.GetById(ChosenID!);
        if (slot == null)
            return;

        slot.item = item;
        slot.UpdateState();
        ChosenID = null;
        Choosing = false;

        if (slot == Slot)
        {
            if (!IsLLTK)
            {
                Keychain.Item = null;
                Keychain.UpdateState();
            }
        }
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        Preview.Item = Slot.Item?.getOne();
        Preview.UpdateState();
    }
}

public partial class SlotData
{
    public SlotData(string id, Func<Item,bool> match)
    {
        ID = id;
        ItemMatch = match;
        itemData = ItemRegistry.GetDataOrErrorItem(Item?.QualifiedItemId ?? "");
        if (!id.StartsWithIgnoreCase("item"))
            slots.Add(id, this);
    }

    public string ID { get; init; }
    public Func<Item, bool> ItemMatch { get; set; }
    [Notify] public object description = null;
    [Notify] public Item? item = null;
    [Notify] public bool hasItem = false;
    [Notify] public ParsedItemData itemData;
    
    public void UpdateState()
    {
        HasItem = Item != null;
        if (HasItem)
            ItemData = ItemRegistry.GetDataOrErrorItem(Item?.QualifiedItemId ?? "");
    }

    private readonly static Dictionary<string, SlotData> slots = [];

    public static void RemoveAllItems()
    {
        foreach (var slot in slots.Values)
        {
            slot.Item = null;
            slot.UpdateState();
        }
    }

    public static SlotData? GetById(string? id)
    {
        if (id != null && slots.TryGetValue(id, out var slot))
            return slot;
        else return null;
    }
    
    public static void ResetForExit()
    {
        slots.Clear();
    }
}