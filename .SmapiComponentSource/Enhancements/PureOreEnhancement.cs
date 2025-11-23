using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;
using System.Text;
namespace FinalMix.Enhancements;

[HarmonyPatch(typeof(MeleeWeapon), nameof(MeleeWeapon.drawTooltip))]
public static class PureOreEnhancement
{
    private readonly static Dictionary<string, int> oreValues = new() {
        { "(O)DN.SnS_PureCopperOre", 5 },
        { "(O)DN.SnS_PureIronOre", 10 },
        { "(O)DN.SnS_PureGoldOre", 15 },
        { "(O)DN.SnS_PureIridiumOre", 20 },
        { "(O)DN.SnS_PureRadioactiveOre", 25 },
        { "(O)DN.SnS_AetheriteBar", 30 }
    };

    [HarmonyPrefix]
    public static void Postfix(Item __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
    {
        if (!__instance.modData.TryGetValue("PureOre", out var ore))
            return;
        
        ParsedItemData data = ItemRegistry.GetData(ore);

        Utility.drawWithShadow(spriteBatch, data.GetTexture(), new(x + 16 + 4, y + 16 + 4), data.GetSourceRect(), Color.White, 0f, Vector2.Zero, 2.5f);
        Utility.drawTextWithShadow(spriteBatch, I18n.PureOre_Effect(oreValues[ore]), font, new Vector2(x + 16 + 52, y + 16 + 12), Game1.textColor * 0.9f * alpha);
        y += (int)Math.Max(font.MeasureString("TT").Y, 48f);
    }
}
