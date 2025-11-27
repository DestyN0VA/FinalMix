using HarmonyLib;
using SpaceCore;
using StardewValley;
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace FinalMix.Drops;

public static class PureOreUtility
{
    public readonly static Dictionary<string, string> PureOreMappings = new()
    {
        { Object.copperQID, "(O)DN.SnS_PureCopperOre" },
        { Object.ironQID, "(O)DN.SnS_PureIronOre" },
        { Object.goldQID, "(O)DN.SnS_PureGoldOre" },
        { Object.iridiumQID, "(O)DN.SnS_PureIridiumOre" },
        { "(O)909", "(O)DN.SnS_PureRadioactiveOre" }
    };
}

[HarmonyPatch(typeof(GameLocation), "breakStone")]
internal static class PureOresCheck
{
    public static int BreakingStone = 0;

    [HarmonyPrefix]
    public static void Check()
    {
        BreakingStone++;
    }

    [HarmonyPostfix]
    public static void Uncheck()
    {
        BreakingStone--;
    }
}

[HarmonyPatch(typeof(Game1), nameof(Game1.createObjectDebris), [typeof(string), typeof(int), typeof(int), typeof(long), typeof(GameLocation)])]
[HarmonyBefore("DaLion.Professions")]
public static class Game1ChangeGemToExquisitePatch
{
    public static void Prefix(ref string id)
    {
        if (PureOresCheck.BreakingStone <= 0)
            return;

        if (PureOreUtility.PureOreMappings.TryGetValue(id, out var newId) && Game1.random.NextBool(0.2f /*+ Math.Clamp(Game1.player.GetCustomSkillLevel(FinalMix.ArtificerSkill), 0, 5) * 0.05f*/))
            id = newId;
    }
}