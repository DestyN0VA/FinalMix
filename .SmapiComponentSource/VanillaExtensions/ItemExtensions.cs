using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace FinalMix.VanillaExtensions;

public static class ItemExtensions
{

    public static bool IsArmor(this Item item)
    {
        return item is Object o && Game1.objectData.TryGetValue(o.ItemId, out var data) && (data.CustomFields?.ContainsKey("ArmorValue") ?? false);
    }
    public static bool IsMateria(this Item item)
    {
        return item != null && item.QualifiedItemId.StartsWith("(O)DN.SnS_") && item.ItemId.EndsWith("Materia");
    }

    public static bool CanHoldMateria(this Item item)
    {
        return (item is MeleeWeapon mw && !mw.isScythe()) || item.IsArmor();
    }

    private const string EffectKey = "DN.SnS_MateriaType";
    private const string RankKey = "DN.SnS_MateriaRank";
    public static bool TryGetMateriaInfo(this Item item, out List<Tuple<string, int>> Effects)
    {
        Effects = [];
        if (!item.IsMateria() && !item.CanHoldMateria())
            return false;
        else if (!item.modData.ContainsKey(EffectKey))
            return true;
        else
        {
            Ef

            Effect = item.modData[EffectKey];
            Rank = int.Parse(item.modData[RankKey]);
            return true;
        }
    }

    public static bool ApplyMateria(this Item item, Item materia)
    {
        FinalMix.Log.Warn("trying");
        if (!item.CanHoldMateria())
            return false;

        if (/*!materia.IsMateria() || */materia.TryGetMateriaInfo(out string Effect, out int Rank))
        { } //return false;

        if (!item.modData.TryAdd(EffectKey, Effect))
            item.modData[EffectKey] = Effect;
        if (!item.modData.TryAdd(RankKey, $"{Rank}"))
            item.modData[RankKey] = $"{Rank}";

        FinalMix.Log.Warn("done??");
        return true;

    }
}
