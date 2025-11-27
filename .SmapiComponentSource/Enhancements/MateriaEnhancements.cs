using FinalMix.Enhancements.Data;
using FinalMix.VanillaExtensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;

namespace FinalMix.Enhancements;
[HarmonyPatch(typeof(Item), nameof(Item.drawTooltip))]
public static class MateriaEnhancementObject
{
	[HarmonyPostfix]
	public static void DrawTooltipPostfix(Item __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
	{
		if (__instance.TryGetMateriaInfo(out string Effect, out int Rank))
		{

			if (!__instance.IsMateria() && Rank == -1)
				return;

			string MateriaText = I18n.Materia_Effect(Rank != -1 ? I18n.GetByKey($"Materia.Effects.{Effect}", Rank) : I18n.Materia_Effects_Unforged());

			spriteBatch.DrawString(font, MateriaText, new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
			spriteBatch.DrawString(font, MateriaText, new Vector2(x + 16, y + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
			spriteBatch.DrawString(font, MateriaText, new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
			spriteBatch.DrawString(font, MateriaText, new Vector2(x + 16, y + 16 + 4), Game1.textColor * 0.9f * alpha);
			y += (int)font.MeasureString(MateriaText).Y + 4;
		}
	}
}
[HarmonyPatch(typeof(MeleeWeapon))]
public static class MataEnhancementObject
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeleeWeapon.drawTooltip))]
    public static void DrawTooltipPostfix(MeleeWeapon __instance, SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha)
    {
        if (__instance.TryGetMateriaInfo(out string Effect, out int Rank))
        {
            if (!__instance.IsMateria() && Rank == -1)
                return;
            string MateriaText = I18n.Materia_Effect(Rank != -1 ? I18n.GetByKey($"Materia.Effects.{Effect}", new { Rank = Rank }) : I18n.Materia_Effects_Unforged());
            Texture2D tex = Game1.content.Load<Texture2D>("DN.Objects/Curios");
            Utility.drawWithShadow(spriteBatch, tex, new Vector2(x + 16 + 4, y + 16 + 4), Game1.getSourceRectForStandardTileSheet(tex, 6, 16, 16), Color.White, 0f, Vector2.Zero, 2.5f);
            Utility.drawTextWithShadow(spriteBatch, MateriaText, font, new(x + 16 + 52, y + 16 + 12), Game1.textColor * 0.9f * alpha);
            y += (int)font.MeasureString(MateriaText).Y + 4;
        }
    }
    [HarmonyPatch(nameof(MeleeWeapon.getExtraSpaceNeededForTooltipSpecialIcons))]
    public static void Postfix(MeleeWeapon __instance, SpriteFont font, ref Point __result)
    {
        if (!__instance.TryGetMateriaInfo(out string Effect, out int Rank) || (!__instance.IsMateria() && Rank == -1))
                return;

        Point dimensions = __result;

        dimensions.Y += Math.Max((int)font.MeasureString("TT").Y, 48);
        __result = dimensions;
    }
}