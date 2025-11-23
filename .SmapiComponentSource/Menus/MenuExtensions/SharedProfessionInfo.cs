using FinalMix.Skills;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.Interface;
using StardewUI.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Reflection.Emit;

namespace FinalMix.Menus.MenuExtensions;
[HarmonyPatch(typeof(SkillLevelUpMenu))]
internal class SharedProfessionInfo
{
    private readonly static Dictionary<SkillLevelUpMenu, IViewDrawable> Drawable = [];

    [HarmonyPrefix]
    [HarmonyPatch(MethodType.Constructor, [typeof(string), typeof(int)])]
    public static void CtorPrefix(SkillLevelUpMenu __instance, string skillName, int level)
    {
        Skill skill = SpaceCore.Skills.GetSkill(skillName);
        if (skill == null || skill is not IHaveSharedInfo infoSkill || !infoSkill.HasSharedInfoForLevel(level))
            return;

        SharedProfessionContext ctx = new(infoSkill.GetSharedInfo(level));
        IViewDrawable drawable = FinalMix.StarUI.CreateDrawableFromAsset("DN.SnS/Views/SharedInfoSection");
        drawable.Context = ctx;

        __instance.yPositionOnScreen -= (int)drawable.ActualSize.Y;

        Drawable.Add(__instance, drawable);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SkillLevelUpMenu.gameWindowSizeChanged))]
    public static bool GameWindowSizeChangedPrefix(SkillLevelUpMenu __instance)
    {
        if (Drawable == null)
            return true;

        __instance.xPositionOnScreen = Game1.uiViewport.Width / 2 - __instance.width / 2;
        __instance.yPositionOnScreen = Game1.uiViewport.Height / 2 - __instance.height / 2 - (int)Drawable[__instance].ActualSize.Y;
        __instance.okButton.bounds = new(__instance.xPositionOnScreen + __instance.width + 4, __instance.yPositionOnScreen + __instance.height - 64 - IClickableMenu.borderWidth, 64, 64);
        __instance.RepositionOkButton();

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(SkillLevelUpMenu.update))]
    public static void UpdatePostfix(SkillLevelUpMenu __instance)
    {
        if (!__instance.isActive && Drawable.TryGetValue(__instance, out var drawable))
        {
            drawable.Dispose();
            Drawable.Remove(__instance);
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(SkillLevelUpMenu.draw))]
    public static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> insns)
    {
        CodeMatcher matcher = new(insns);

        matcher.MatchEndForward([
            new(OpCodes.Call, AccessTools.Method(typeof(IClickableMenu), "drawHorizontalPartition"))
            ])
            .Insert([
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Call, AccessTools.Method(typeof(SharedProfessionInfo), nameof(DrawDrawable)))
            ]);

        return matcher.Instructions();
    }


    public static void DrawDrawable(SkillLevelUpMenu __instance, SpriteBatch b)
    {
        if (!Drawable.TryGetValue(__instance, out var drawable)) 
            return;

        drawable.Draw(b, new(__instance.xPositionOnScreen, __instance.yPositionOnScreen + __instance.height - 48));
        FinalMix.Helper.Reflection.GetMethod(__instance, "drawHorizontalPartition")?.Invoke([b, __instance.yPositionOnScreen + __instance.height - 56, false, -1, -1, -1]);
    }
}

public class SharedProfessionContext(string sharedInfo)
{
    public string SharedInfo { get; set; } = sharedInfo;
}