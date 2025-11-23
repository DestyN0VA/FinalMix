using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewUI.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Reflection.Emit;

namespace FinalMix.Menus.MenuExtensions;

[HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.update))]
static class CharacterCustomizationUpdate
{
    static void Postfix(CharacterCustomization __instance)
    {
        if (__instance.getComponentWithID(700) is not ClickableTextureComponent button)
            AddClickableComponent(__instance);
        else
        {
            Rectangle Bounds = new(__instance.xPositionOnScreen - 80, __instance.yPositionOnScreen + __instance.height - 80 - 16, 80, 80);
            if (__instance.source == CharacterCustomization.Source.HostNewFarm)
                Bounds.X -= Bounds.Width + 12;
            else if (__instance.source != CharacterCustomization.Source.NewFarmhand)
                Bounds.Y -= Bounds.Height + 12;
            button.bounds = Bounds;
        }
    }

    static void AddClickableComponent(CharacterCustomization __instance)
    {
        Rectangle Bounds = new(__instance.xPositionOnScreen - 80, __instance.yPositionOnScreen + __instance.height - 80 - 16, 80, 80);
        if (__instance.source == CharacterCustomization.Source.HostNewFarm)
            Bounds.X -= Bounds.Width + 12;
        else if (__instance.source != CharacterCustomization.Source.NewFarmhand)
            Bounds.Y -= Bounds.Height + 12;

        ClickableTextureComponent button = new("DN.SnS_SKILL_SELECT", Bounds, null, null, Game1.mouseCursors2, new Rectangle(154, 154, 20, 20), 4f)
        {
            myID = 700,
            leftNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            rightNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            upNeighborID = ClickableComponent.SNAP_AUTOMATIC,
            downNeighborID = ClickableComponent.SNAP_AUTOMATIC,
        };
        __instance.allClickableComponents ??= [];
        __instance.allClickableComponents.Add(button);
    }
}

[HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.draw), [typeof(SpriteBatch)])]
static class CharacterCustomizationDraw
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insns)
    {
        CodeMatcher matcher = new(insns);
        matcher.MatchStartForward([
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterCustomization), nameof(CharacterCustomization.advancedOptionsButton)))
            ]).Insert([
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Call, AccessTools.Method(typeof(CharacterCustomizationDraw), nameof(Draw)))
            ]);

        return matcher.Instructions();
    }

    static void Draw(CharacterCustomization __instance, SpriteBatch b)
    {
        if (__instance.source != CharacterCustomization.Source.NewGame
            && __instance.source != CharacterCustomization.Source.HostNewFarm
            && __instance.source != CharacterCustomization.Source.NewFarmhand)
            return;

        if (__instance.getComponentWithID(700) is not ClickableTextureComponent button)
            return;

        button.draw(b);
    }
}

[HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.performHoverAction))]
static class CharacterCutomizationHover
{
    static void Postfix(CharacterCustomization __instance, int x, int y)
    {
        if (__instance.getComponentWithID(700) is ClickableTextureComponent button)
            button.tryHover(x, y);
    }
}

[HarmonyPatch(typeof(CharacterCustomization), nameof(CharacterCustomization.receiveLeftClick))]
static class CharacterCustomizationClick
{
    public static void Postfix(CharacterCustomization __instance, int x, int y)
    {
        if (__instance.getComponentWithID(700)?.containsPoint(x, y) ?? false)
        {
            Game1.playSound("drumkit6");

            __instance.AddDependency();
            SkillSelectionContext ctx = new();
            IMenuController controller = FinalMix.StarUI.CreateMenuControllerFromAsset("DN.SnS/Views/SkillSelection", ctx);
            ctx.Menu = controller.Menu;
            ctx.ParentMenu = __instance;
            controller.CanClose = () => false;

            TitleMenu.subMenu = controller.Menu;
        }
    }
}
