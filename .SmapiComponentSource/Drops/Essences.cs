using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System.Reflection.Emit;
using static StardewValley.Farm;
using Object = StardewValley.Object;

namespace FinalMix.Drops;

internal static class EssenceUtility
{
    private enum EssenceType
    {
        Tidal,
        Storm,
        Aether,
        Druidic
    }

    private class WeatherEssence(Vector2 position, EssenceType type)
    {
        private readonly static Dictionary<EssenceType, Rectangle> sourceRects = new() {
            [EssenceType.Tidal] = new(32, 16, 16, 16),
            [EssenceType.Storm] = new(64, 16, 16, 16),
            [EssenceType.Aether] = new(96, 16, 16, 16),
            [EssenceType.Druidic] = new(80, 16, 16, 16)
        };
        private readonly static Dictionary<EssenceType, Item> essences = new()
        {
            [EssenceType.Tidal] = ItemRegistry.Create(TidalEssenceID),
            [EssenceType.Storm] = ItemRegistry.Create(StormEssenceID),
            [EssenceType.Aether] = ItemRegistry.Create(AetherEssenceID),
            [EssenceType.Druidic] = ItemRegistry.Create(DruidicEssenceID)
        };
        private readonly bool flipped = Game1.random.NextBool();
        private bool didAlpha = false;
        
        public Vector2 Position { get; set; } = position;
        public Rectangle SourceRect { get; set; } = sourceRects[type];
        public double Alpha { get; set; } = 0f;
        public double Timer { get; set; } = 0;
        public float Speed { get; set; } = Game1.random.Next(90, 111) / 100;

        public bool Update()
        {
            if (!didAlpha)
            {
                Alpha += 0.05;
                didAlpha = Alpha >= 1;
                Alpha = Math.Clamp(Alpha, 0, 1);
            }

            if (Timer >= 1500)
            {
                Game1.createObjectDebris(essences[type].QualifiedItemId, (int)(Position.X / 64), (int)(Position.Y / 64), Game1.player.currentLocation);
                return true;
            }
            Position -= new Vector2(4, -7) * Speed;
            return false;
        }

        public void Draw(SpriteBatch b)
        {
            b.Draw(FinalMix.Helper.GameContent.Load<Texture2D>("DN.Objects/Curios"), Game1.GlobalToLocal(Game1.viewport, Position), SourceRect, Color.White * (float)Alpha, 0.35f, Vector2.Zero, 4, flipped ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 1f);
        }
    }

    private readonly static List<WeatherEssence> Essences = [];
    private const string Prefix = "DN.SnS_";
    
    public const string TidalEssenceID = $"(O){Prefix}TidalEssence";
    public const string FlareEssenceID = $"(O){Prefix}FlareEssence";
    public const string StormEssenceID = $"(O){Prefix}StormEssence";
    public const string DruidicEssenceID = $"(O){Prefix}DruidicEssence";
    public const string AetherEssenceID = $"(O){Prefix}AetherEssence";
 

    public static void EssencesDuringRain(object? sender, RenderedWorldEventArgs e)
    {
        if (!Game1.IsRainingHere() || !Game1.currentLocation.IsOutdoors || Game1.viewport.Width <= 0)
            return;

        if (Game1.shouldTimePass() && Game1.random.NextBool(0.00125f))
        {
            EssenceType which = Game1.random.NextBool(0.1) ? EssenceType.Aether : (Game1.isGreenRain ? EssenceType.Druidic : (Game1.random.NextBool() && Game1.isLightning ? EssenceType.Storm : EssenceType.Tidal)); 
            Vector2 Pos = new(Game1.random.Next(Game1.viewport.X - 64 * 2, Game1.viewport.X + Game1.viewport.Width + 64 * 2), Game1.random.Next(Game1.viewport.Y - 64 * 2, Game1.viewport.Y + Game1.viewport.Height /3));
            
            while (Essences.Any(e => e.Position == Pos))
                Pos = new(Game1.random.Next(Game1.viewport.X, Game1.viewport.X + Game1.viewport.Width + 64 * 3), Game1.random.Next(Game1.viewport.Y - 64 * 4, Game1.viewport.Y + Game1.viewport.Height * 3 / 2));

            Essences.Add(new(Pos, which));
        }

        for (int i = Essences.Count - 1; i > 0; i--)
        {
            var essence = Essences[i];

            if (Game1.shouldTimePass() && !Game1.IsFading())
            {
                if (essence.Update())
                    Essences.Remove(essence);
                else if (essence.Alpha >= 1)
                    essence.Timer += Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (FinalMix.Config.EnableEssenceRainVisuals)
                essence.Draw(e.SpriteBatch);
        }
    }

    public static void ResetLocationChange(object? sender, WarpedEventArgs e)
    {
        Essences.Clear();
    }

    public static void ResetDayStarted(object? sender, DayStartedEventArgs e)
    {
        Essences.Clear();
    }
}

[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.monsterDrop))]
internal static class EssencesFromMonsters
{
    [HarmonyPostfix]
    public static void MonsterDropPostfix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
    {

        if (monster.isGlider.Value)
        {
            if (Game1.random.NextDouble() < 0.25)
                DropEssence(EssenceUtility.StormEssenceID, 1, __instance, x, y);
            if (Game1.random.NextDouble() < 0.10)
                DropEssence(EssenceUtility.AetherEssenceID, 1, __instance, x, y);
        }

        double FlareChance = 0.25 + 0.05 * Math.Clamp(who?.GetCustomBuffedSkillLevel(FinalMix.ArtificerSkill) ?? 0, 0, 5);

        if ((monster is GreenSlime or RockCrab && __instance is MineShaft ms && ms.mineLevel >= 81 && ms.mineLevel < 120) || __instance is VolcanoDungeon || (monster is Bat && monster.Name == "Lava Bat") || monster is LavaLurk or SquidKid)
        {
            if (Game1.random.NextDouble() < FlareChance)
                DropEssence(EssenceUtility.FlareEssenceID, 1, __instance, x, y);
            if (Game1.random.NextDouble() < 0.10)
                DropEssence(EssenceUtility.AetherEssenceID, 1, __instance, x, y);
        }
    }

    public static void DropEssence(string Item, int Quantity, GameLocation location, int x, int y)
    {
        Item drop = ItemRegistry.Create(Item, Quantity);
        Game1.createItemDebris(drop, new(x, y), 2, location);
    }
}

[HarmonyPatch(typeof(GameLocation), nameof(GameLocation.OnStoneDestroyed))]
internal static class EssencesFromRocks
{
    [HarmonyPostfix]
    public static void OnStoneDestroyedPostfix(GameLocation __instance, int x, int y, Farmer who)
    {
        if (who == null)
            return;

        if (Game1.random.NextDouble() < 0.15 /*+ 0.03 * Math.Clamp(who.GetCustomBuffedSkillLevel(FinalMix.ArtificerSkill), 0, 5)*/)
            Game1.createObjectDebris(EssenceUtility.FlareEssenceID, x, y, who.UniqueMultiplayerID, __instance);
        if (Game1.random.NextDouble() < 0.02)
            Game1.createObjectDebris(EssenceUtility.AetherEssenceID, x, y, who.UniqueMultiplayerID, __instance);
    }
}

[HarmonyPatch(typeof(FishingRod), nameof(FishingRod.playerCaughtFishEndFunction))]
internal static class EssencesFromFishing
{
    [HarmonyPostfix]
    public static void PlayerCoughtFishEndFunctionPostfix(FishingRod __instance, bool isBossFish)
    {
        Farmer who = __instance.lastUser ?? Game1.player;

        if (Game1.random.NextDouble() < (isBossFish ? 1 : 0.25))
        {   
            if ((who.currentLocation is MineShaft ms && ms.mineLevel == 100) || who.currentLocation is Caldera or VolcanoDungeon)
                Game1.createObjectDebris(EssenceUtility.FlareEssenceID, (int)who.Tile.X, (int)who.Tile.Y, who.UniqueMultiplayerID, who.currentLocation);
            else 
                Game1.createObjectDebris(EssenceUtility.TidalEssenceID, (int)who.Tile.X, (int)who.Tile.Y, who.UniqueMultiplayerID, who.currentLocation);
        }
        if (Game1.random.NextDouble() < (isBossFish ? 1 : 0.05))
            Game1.createObjectDebris(EssenceUtility.AetherEssenceID, (int)who.Tile.X, (int)who.Tile.Y, who.UniqueMultiplayerID, who.currentLocation);
    }
}

[HarmonyPatch(typeof(Pan), nameof(Pan.getPanItems))]
internal static class EessencesFromPanning
{
    [HarmonyPostfix]
    public static void GetPanItemsPostfix(List<Item> __result)
    {
        if (Game1.random.NextDouble() < 0.5)
            __result.Add(ItemRegistry.Create(EssenceUtility.TidalEssenceID, Game1.random.Next(2, 4)));
        if (Game1.random.NextDouble() < 0.2)
            __result.Add(ItemRegistry.Create(EssenceUtility.AetherEssenceID, Game1.random.Next(2, 4)));
    }
}

[HarmonyPatch(typeof(CrabPot), nameof(CrabPot.checkForAction))]
internal static class EssencesFromCrabPot
{
    [HarmonyPrefix]
#pragma warning disable IDE0060 // Remove unused parameter
    public static void CheckForActionPrefix(CrabPot __instance, bool __state)
#pragma warning restore IDE0060 // Remove unused parameter
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        __state = __instance.readyForHarvest.Value && __instance.heldObject.Value?.QualifiedItemId != EssenceUtility.TidalEssenceID && __instance.heldObject.Value?.QualifiedItemId != EssenceUtility.AetherEssenceID;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }

    [HarmonyPostfix]
    public static void CheckForActionPostfix(CrabPot __instance, bool __state)
    {
        if (!__state || __instance.readyForHarvest.Value)
            return;

        void setUpCrabPot(string id)
        {
            __instance.heldObject.Value = ItemRegistry.Create<Object>(id, Game1.random.Next(1, 5));
            __instance.readyForHarvest.Value = true;
            __instance.tileIndexToShow = 714;
        }

        if (Game1.random.NextDouble() < 0.25)
            setUpCrabPot(EssenceUtility.TidalEssenceID);
        else if (Game1.random.NextDouble() < 0.05)
            setUpCrabPot(EssenceUtility.AetherEssenceID);
    }
}

[HarmonyPatch(typeof(Object), "CheckForActionOnMachine", [typeof(Farmer), typeof(bool)])]
internal static class EssencesFromLightningRods
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> PerformLightningUpdateTranspiler(IEnumerable<CodeInstruction> insns)
    {
        CodeMatcher matcher = new(insns);

        matcher.MatchEndForward([
            new(OpCodes.Call, AccessTools.Method(typeof(Item), nameof(Item.ResetParentSheetIndex)))
            ]);
        matcher.Advance(1);
        matcher.Insert([
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Call, AccessTools.Method(typeof(EssencesFromLightningRods), nameof(DoEssence)))
            ]);

        return matcher.Instructions();
    }

    public static void DoEssence(Object __instance, Farmer who)
    {
        if (__instance.QualifiedItemId != "(BC)9" || who == null)
            return;

        if (Game1.random.NextDouble() < 0.25)
            Game1.createObjectDebris(EssenceUtility.StormEssenceID, (int)__instance.TileLocation.X, (int)__instance.TileLocation.Y, who.UniqueMultiplayerID, who.currentLocation);
        if (Game1.random.NextDouble() < 0.05)
            Game1.createObjectDebris(EssenceUtility.AetherEssenceID, (int)__instance.TileLocation.X, (int)__instance.TileLocation.Y, who.UniqueMultiplayerID, who.currentLocation);
    }
}

[HarmonyPatch(typeof(Crop), nameof(Crop.harvest))]
internal static class EssencesFromCrops
{
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> HarvestTranspiler(IEnumerable<CodeInstruction> insns)
    {
        CodeMatcher matcher = new(insns);
        object operand = insns.ToList().First(i => i.opcode == OpCodes.Ldarg_S).operand;

        matcher.MatchEndForward([
            new(OpCodes.Ldloc_0),
            new(OpCodes.Brfalse)
            ]);
        matcher.Advance(1);
        matcher.Insert([
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldarg_1),
            new(OpCodes.Ldarg_2),
            new(OpCodes.Ldarg_S, operand),
            new(OpCodes.Call, AccessTools.Method(typeof(EssencesFromCrops), nameof(DropEssence)))
            ]);

        return matcher.Instructions();
    }

    public static void DropEssence(Crop crop, int xTile, int yTile, JunimoHarvester junimoHarvester)
    {
        if (junimoHarvester == null && Game1.random.NextDouble() < 0.0125)
        {
            Vector2 Pos = new(xTile, yTile);
            Pos *= Game1.tileSize;
            Game1.createItemDebris(ItemRegistry.Create(EssenceUtility.DruidicEssenceID), Pos, -1, crop.currentLocation);
        }
    }
}

[HarmonyPatch(typeof(Grass), nameof(Grass.TryDropItemsOnCut))]
internal static class EssencesFromGrass
{
    public static void Postfix(Grass __instance, Tool tool, bool __result)
    {
        if (!__result)
            return;

        Farmer who = tool?.lastUser ?? Game1.player;

        if (Game1.random.NextDouble() < 0.1)
            Game1.createObjectDebris(EssenceUtility.DruidicEssenceID, (int)__instance.Tile.X, (int)__instance.Tile.Y, who.UniqueMultiplayerID, __instance.Location);
        if (Game1.random.NextDouble() < 0.01)
            Game1.createObjectDebris(EssenceUtility.AetherEssenceID, (int)__instance.Tile.X, (int)__instance.Tile.Y, who.UniqueMultiplayerID, __instance.Location);
    }
}

[HarmonyPatch(typeof(Tree), "performTreeFall")]
internal static class EssencesFromTrees
{
    [HarmonyPostfix]
    public static void PerformTreeFallPostfix(Tree __instance, Tool t)
    {
        Farmer who = t?.lastUser ?? Game1.player;

        if (Game1.random.NextDouble() < 0.25)
            Game1.createObjectDebris(EssenceUtility.DruidicEssenceID, (int)__instance.Tile.X, (int)__instance.Tile.Y, who.UniqueMultiplayerID, __instance.Location);
        if (Game1.random.NextDouble() < 0.05)
            Game1.createObjectDebris(EssenceUtility.AetherEssenceID, (int)__instance.Tile.X, (int)__instance.Tile.Y, who.UniqueMultiplayerID, __instance.Location);
    }
}

[HarmonyPatch(typeof(Farm), "doLightningStrike")]
internal static class EssencesFromLightning
{
    public static void Postfix(LightningStrikeEvent lightning)
    {
        if (!Context.IsMainPlayer || lightning.createBolt)
            return;
        GameLocation loc = Game1.random.ChooseFrom([.. Game1.locations.Where(l => Game1.getAllFarmers().Any(f => f.locationsVisited.Contains(l.Name)))]);
        Vector2 Tile = loc.getRandomTile();

        Game1.createMultipleItemDebris(ItemRegistry.Create(Game1.random.NextDouble() > 0.1 ? EssenceUtility.StormEssenceID : EssenceUtility.AetherEssenceID, Game1.random.Next(1, 4)), Tile, -1, loc);
    }
}
