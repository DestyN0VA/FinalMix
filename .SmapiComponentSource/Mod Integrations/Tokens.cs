using ContentPatcher;
using StardewModdingAPI;
using StardewValley;

namespace FinalMix.ModIntegrations;

internal static class Tokens
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

        //Register tokens here:
    }
}
