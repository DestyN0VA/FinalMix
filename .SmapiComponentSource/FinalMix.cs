using ContentPatcher;
using FinalMix.ModIntegrations;
using FinalMix.Skills;
using FinalMix.Util;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewUI.Framework;
using HarmonyLib;
using System.Runtime.CompilerServices;
using FinalMix.Patches;
using FinalMix.Menus;
using FinalMix.Mod_Integrations;

namespace FinalMix;

internal class FinalMix : Mod
{
    public static FinalMix Instance { get; set; } = null!;
    public static new IModHelper Helper { get; set; } = null!;
    public static LogUtil Log { get; set; } = null!;
    public static Configs Config { get; set; } = null!;

    public static ISpaceCoreApi SpaceCore { get => Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore")!; }
    public static IViewEngine StarUI { get => Helper.ModRegistry.GetApi<IViewEngine>("focustense.StardewUI")!; }
    public static GenericModConfigMenu.IGenericModConfigMenuApi? GMCM { get => Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu"); }

    public static ArtificerSkill ArtificerSkill { get; set; } = null!;
    public static SkillIcons SkillIcons = null!;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Entry(IModHelper helper)
    {
        Instance = this;
        Helper = helper;
        Log = new LogUtil(Monitor);

        I18n.Init(Helper.Translation);

        Helper.Events.GameLoop.GameLaunched += GameLaunched;

        SkillIcons = new();
        Config = Helper.ReadConfig<Configs>();
        Helper.Events.Content.AssetRequested += SkillIcons.AssetRequested;
        Helper.Events.Content.AssetsInvalidated += SkillIcons.AssetsInvalidated;

        Helper.Events.Display.RenderedWorld += EssenceUtility.EssencesDuringRain;
        Helper.Events.GameLoop.DayStarted += EssenceUtility.ResetDayStarted;
        Helper.Events.Player.Warped += EssenceUtility.ResetLocationChange;

        GameStateQuery.Register("DN.SNS_PLAYER_HAS_ARTIFICER", (query, context) =>
        {
            if (!ArgUtility.TryGet(query, 1, out var value, out var error, allowBlank: true, "string playerKey"))
                return GameStateQuery.Helpers.ErrorResult(query, error);

            return GameStateQuery.Helpers.WithPlayer(context.Player, value, (Farmer target) => target.hasOrWillReceiveMail("DN.SnS_ArtificerUnlocked"));
        });

        Harmony harmony = new(ModManifest.UniqueID);
        harmony.PatchAll();
    }

    private void GameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Tokens.RegisterTokens(Helper.ModRegistry.GetApi<IContentPatcherAPI>("pathoschild.ContentPatcher")!);

        if (GMCM != null)
            ConfigMenu.SetUpGMCM(GMCM);


        ArtificerSkill = new();
        SpaceSkills.RegisterSkill(ArtificerSkill);

        StarUI.RegisterViews("DN.SnS/Views", "Assets/Views");
        StarUI.RegisterSprites("DN.SnS/SkillSprites", "Assets/SkillPageIcons");
        StarUI.RegisterSprites("DN.SnS/UISprites", "Assets/UISprites");
        StarUI.PreloadAssets();

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