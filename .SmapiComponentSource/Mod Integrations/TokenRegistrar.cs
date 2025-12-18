using ContentPatcher;
using StardewModdingAPI;
using StardewValley;
using FinalMix.TemporaryTokenForSpellbook;
using StardewValley.Extensions;

namespace FinalMix.ModIntegrations;

internal static class TokenRegistrar
{
    public static void RegisterTokens(IContentPatcherAPI ContentPatcher)
    {
        ContentPatcher.RegisterToken(FinalMix.Instance.ModManifest, "PlayerHasArtificer", () =>
        {
            Farmer player;

            if (Context.IsWorldReady)
                player = Game1.player;
            else if (SaveGame.loaded?.player != null)
                player = SaveGame.loaded.player;
            else
                return null;

            return [player.hasOrWillReceiveMail("ET.SnS_ArtificerUnlocked") ? "true" : "false"];
        });

        ContentPatcher.RegisterToken(FinalMix.Instance.ModManifest, "PlayerStat", new PlayerStatToken());

        ContentPatcher.RegisterToken(FinalMix.Instance.ModManifest, "CorianderHair", () =>
        {
            if (!Context.IsWorldReady)
                return null;

            if (Game1.stats.Get("CorianderCooldown") == 0 && Game1.random.NextBool(1 / 3))
            {
                Game1.stats.Increment("CorianderCooldown", 6);
                Game1.stats.Increment("CorianderHair", 1);
            }
            else if (Game1.stats.Get("CorianderCooldown") != 0)
            {
                Game1.stats.Increment("CorianderCooldown", -1);
            }

            Random r = Utility.CreateRandom(Game1.hash.GetDeterministicHashCode("CorianderHair"), Game1.uniqueIDForThisGame, Game1.stats.Get("CorianderHair"));

            return r.Next(0, 5) switch
            {
                0 => ["Blue"],
                1 => ["Red"],
                2 => ["Black"],
                3 => ["Orange"],
                _ => ["Purple"],
            };
        });
    }
}
