using GenericModConfigMenu;
using static FinalMix.FinalMix;

namespace FinalMix.Mod_Integrations;
internal class ConfigMenu
{
    public static void SetUpGMCM(IGenericModConfigMenuApi GMCM)
    {
        var mod = Instance.ModManifest;
        GMCM.Register(mod, () => Config = new(), () => Helper.WriteConfig(Config));
        GMCM.AddBoolOption(mod, () => Config.EnableEssenceRainVisuals, (value) => Config.EnableEssenceRainVisuals = value, I18n.Config_EnableEssenceRainVisuals_Name, I18n.Config_EnableEssenceRainVisuals_Description);
    }
}
