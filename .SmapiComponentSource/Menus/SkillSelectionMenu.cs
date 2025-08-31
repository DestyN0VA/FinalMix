using StardewValley.Menus;
using StardewValley;
using PropertyChanged.SourceGenerator;

namespace FinalMix.Menus;

public partial class SkillSelectionContext
{
    public IClickableMenu Menu { get; set; } = null!;
    public IClickableMenu ParentMenu { get; set; } = null!;
    [Notify] private bool isExpanded = true;

    //EarlyUnlocks
    [Notify] public bool artificer = Game1.player.hasOrWillReceiveMail("DN.SnS_ArtificerUnlocked");
    [Notify] public bool bardics = Game1.player.hasOrWillReceiveMail("DN.SnS_BardicsUnlocked");
    [Notify] public bool druidics = Game1.player.hasOrWillReceiveMail("DN.SnS_DruidicsUnlocked");
    [Notify] public bool paladin = Game1.player.hasOrWillReceiveMail("DN.SnS_PaladinUnlocked");
    [Notify] public bool sorcery = Game1.player.hasOrWillReceiveMail("DN.SnS_SorceryUnlocked");

    public void Exit()
    {
        static void DoMailFlag(bool add, string whichSkill)
        {
            if (add)
                Game1.addMail($"DN.SnS_{whichSkill}Unlocked", true);
            else
                Game1.player.RemoveMail($"DN.SnS_{whichSkill}Unlocked");
        }

        DoMailFlag(Artificer, "Artificer");
        DoMailFlag(Bardics, "Bardics");
        DoMailFlag(Druidics, "Druidics");
        DoMailFlag(Paladin, "Paladin");
        DoMailFlag(Sorcery, "Sorcery");

        TitleMenu.subMenu = ParentMenu;
        ParentMenu.RemoveDependency();
        FinalMix.Helper.Reflection.GetMethod(ParentMenu, "ResetComponents").Invoke();
        if (Game1.options.SnappyMenus)
        {
            ParentMenu.populateClickableComponentList();
            ParentMenu.setCurrentlySnappedComponentTo(700);
            ParentMenu.snapCursorToCurrentSnappedComponent();
        }
        Menu.exitThisMenu();
    }
}
