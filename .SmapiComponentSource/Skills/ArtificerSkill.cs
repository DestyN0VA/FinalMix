using FinalMix.Skills.Professions;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FinalMix.Skills;

internal class ArtificerSkill : Skill, IHaveSharedInfo
{
    //Initial Professions
    public static GenericProfession RougishArchetype { get; set; } = null!;
    public static GenericProfession ArtificerSpecialist { get; set; } = null!;

    //Rougish Archetype Path 
    public static GenericProfession CunningAction { get; set; } = null!;
    public static GenericProfession UncannyDodge { get; set; } = null!;

    //Artificer Specialist Path
    public static GenericProfession BattleSmith { get; set; } = null!;
    public static GenericProfession SoulOfArtifice { get; set; } = null!;

    public ArtificerSkill() : base("DN.SnS_ArtificerSkill")
    {
        Icon = FinalMix.SkillIcons.ArtificerIcon;
        SkillsPageIcon = FinalMix.SkillIcons.ArtificerSkillPageIcon;

        ExperienceCurve = [100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000];
        ExperienceBarColor = new Color(252, 121, 27);

        RougishArchetype = new(this, "Artificer_RougishArchetype", I18n.Skills_ArtificerSkill_RougishArchetype_Name, I18n.Skills_ArtificerSkill_RougishArchetype_Description)
        {
            Icon = FinalMix.SkillIcons.ArtificerIcon
        };
        Professions.Add(RougishArchetype);

        ArtificerSpecialist = new(this, "Artificer_ArtificerSpecialist", I18n.Skills_ArtificerSkill_ArtificerSpecialist_Name, I18n.Skills_ArtificerSkill_ArtificerSepcialist_Description)
        {
            Icon = FinalMix.SkillIcons.ArtificerIcon
        };
        Professions.Add(ArtificerSpecialist);

        ProfessionsForLevels.Add(new(5, RougishArchetype, ArtificerSpecialist));

        CunningAction = new(this, "Artificer_CunningAction", I18n.Skills_ArtificerSkill_CunningAction_Name, I18n.Skills_ArtificerSkill_CunningAction_Description)
        {
            Icon = FinalMix.SkillIcons.ArtificerIcon,
            
        };
        Professions.Add(CunningAction);

        UncannyDodge = new(this, "Artificer_UncannyDodge", I18n.Skills_ArtificerSkill_UncannyDodge_Name, I18n.Skills_ArtificerSkill_UncannyDodge_Description)
        {
            Icon = FinalMix.SkillIcons.ArtificerIcon
        };
        Professions.Add(UncannyDodge);

        ProfessionsForLevels.Add(new(10, CunningAction, UncannyDodge, RougishArchetype));

        BattleSmith = new(this, "Artificer_BattleSmith", I18n.Skills_ArtificerSkill_RougishArchetype_Name, I18n.Skills_ArtificerSkill_RougishArchetype_Description)
        {
            Icon = FinalMix.SkillIcons.ArtificerIcon
        };
        Professions.Add(BattleSmith);

        SoulOfArtifice = new(this, "Artificer_SoulOfArtifice", I18n.Skills_ArtificerSkill_RougishArchetype_Name, I18n.Skills_ArtificerSkill_RougishArchetype_Description)
        {
            Icon = FinalMix.SkillIcons.ArtificerIcon
        };
        Professions.Add(SoulOfArtifice);

        ProfessionsForLevels.Add(new(10, BattleSmith, SoulOfArtifice, ArtificerSpecialist));
    }

    public override bool ShouldShowOnSkillsPage => Game1.player.hasOrWillReceiveMail("DN.SnS_ArtificerUnlocked");
    
    public override void DoLevelPerk(int level)
    {
        Game1.player.maxStamina.Value += 5;
        base.DoLevelPerk(level);
    }

    public override List<string> GetExtraLevelUpInfo(int level)
    {
        List<string> LevelUpInfo = [ I18n.Skills_ArtificerSkill_ExtraStamina(5), I18n.Skills_ArtificerSkill_PureOre() ];
        string Rank = "";
        switch(level)
        {
            case 1:
                LevelUpInfo.Add(I18n.Skills_ArtificerSkill_MateriaSlotUnlocked());
                break;
            case 2:
                Rank = "I";
                goto case 4;
            case 3:
                    Rank = "II";
                goto case 4;
            case 4:
                if (string.IsNullOrEmpty(Rank))
                    Rank = "III";
                LevelUpInfo.Add(I18n.Skills_ArtificerSkill_MateriaRankUnlocked(Rank));
                break;
            case 6:
            case 7:
            case 8:
            case 9:
                LevelUpInfo.Add(I18n.Skills_ArtificerSkill_FlareEssenceIncrease());
                break;
        }

        return LevelUpInfo;
    }

    public override string GetName()
    {
        return I18n.Skills_ArtificerSkill_Name();
    }

    public override string GetSkillPageHoverText(int level)
    {
        return I18n.Skills_ArtificerSkill_ExtraStamina(level * 5);
    }

    public bool HasSharedInfoForLevel(int level)
    {
        return level == 5 || level == 10;
    }

    public string GetSharedInfo(int level)
    {
        return level switch
        {
            5 => I18n.Skills_ArtificerSkill_SharedInfo_Lvl5(),
            10 => I18n.Skills_ArtificerSkill_SharedInfo_Lvl10(),
            _ => string.Empty,
        };
    }
}
