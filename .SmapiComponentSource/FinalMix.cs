using ContentPatcher;
using FinalMix.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using FinalMix.Integrations;

namespace FinalMix;

internal class FinalMix : Mod
{
    public static FinalMix Instance { get; set; } = null!;
    public static new IModHelper Helper { get; set; } = null!;
    public static LogUtil Log { get; set; } = null!;

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Helper = helper;
        Log = new LogUtil(Monitor);

        I18n.Init(Helper.Translation);

        Helper.Events.GameLoop.GameLaunched += GameLaunched;
    }

    private void GameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Tokens.CP = Helper.ModRegistry.GetApi<IContentPatcherAPI>("pathoschild.ContentPatcher");
        if (Tokens.CP != null)
            Tokens.RegisterTokens();
    }
}
