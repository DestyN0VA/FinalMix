using ContentPatcher;
using FinalMix.Util;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using FinalMix.Integrations;
using StardewValley;

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


        //GonnaMoveThisLaterIJustDon'tWannaOrganizeThisYetWithNothingElseImplemented
        Event.RegisterCommand("DN.SnS_ConditionalSwitchEvent", (@event, args, ctx) =>
        {
            if (args.Length < 3)
                @event.LogCommandErrorAndSkip(args, "event command has less than the required amount of parameters (2).");
            else
            {
                string GSQ = ArgUtility.Get(args, 1);
                string EventSwitch = ArgUtility.Get(args, 2);
                string[] commands;


                if (!GameStateQuery.CheckConditions(GSQ))
                {
                    if (ArgUtility.TryGetOptional(args, 3, out string newEvent, out _, EventSwitch, name: "FalseEvent"))
                        EventSwitch = newEvent;
                    else
                    {
                        @event.CurrentCommand++;
                        return;
                    }
                }

                string assetName = "Data\\Events\\" + Game1.currentLocation.Name;
                if (!Game1.content.DoesAssetExist<Dictionary<string, string>>(assetName))
                {
                    ctx.LogErrorAndSkip("can't load new event from asset '" + assetName + "' because it doesn't exist");
                    return;
                }
                if (!Game1.content.Load<Dictionary<string, string>>(assetName).TryGetValue(EventSwitch, out var raw2))
                {
                    ctx.LogErrorAndSkip($"can't load new event from asset '{assetName}' because it doesn't contain the required '{EventSwitch}' key");
                    return;
                }
                commands = Event.ParseCommands(raw2, ctx.Event.farmer);
                @event.ReplaceAllCommands(commands);
                @event.eventSwitched = true;
            }
        });
    }
}
