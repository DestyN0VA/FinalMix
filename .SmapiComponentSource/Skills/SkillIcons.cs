using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FinalMix.Skills;

internal class SkillIcons
{
    private Texture2D? _ArtificerIcon = null;
    private Texture2D? _ArtificerSkillPageIcon = null;

    public Texture2D ArtificerIcon 
    { 
        get => _ArtificerIcon ??= FinalMix.Helper.GameContent.Load<Texture2D>("DN.SnS/ArtificerIcon"); 
    }
    public Texture2D ArtificerSkillPageIcon
    {
        get => _ArtificerSkillPageIcon ??= FinalMix.Helper.GameContent.Load<Texture2D>("DN.SnS/ArtificerIcon");
    }

    public void AssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("DN.SnS/ArtificerIcon"))
            e.LoadFromModFile<Texture2D>("Assets/SkillIcons/Artificer.png", AssetLoadPriority.Low);
        if (e.NameWithoutLocale.IsEquivalentTo("DN.SnS/ArtificerIcon"))
            e.LoadFromModFile<Texture2D>("Assets/SkillPageIcons/Artificer.png", AssetLoadPriority.Low);
    }

    public void AssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        foreach (IAssetName name in e.Names)
        {
            if (name.IsEquivalentTo("DN.SnS/ArtificerIcon"))
                _ArtificerIcon = null;
            else if (name.IsEquivalentTo("DN.SnS/ArtificerSkillPageIcon"))
                _ArtificerSkillPageIcon = null;
        }
    }
}
