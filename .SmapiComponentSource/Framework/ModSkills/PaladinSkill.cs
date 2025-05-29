using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static SpaceCore.Skills;

namespace SwordAndSorcerySMAPI.Framework.ModSkills
{
    public class PaladinSkill : Skill
    {
        public static GenericProfession ProfessionShieldThrowHit2 { get; set; }
        public static GenericProfession ProfessionShieldArmor1 { get; set; }
        public static GenericProfession ProfessionShieldThrowLightning { get; set; }
        public static GenericProfession ProfessionShieldThrowHit3 { get; set; }
        public static GenericProfession ProfessionShieldArmor2 { get; set; }
        public static GenericProfession ProfessionShieldRetribution { get; set; }

        public PaladinSkill()
            : base("DestyNova.SwordAndSorcery.Paladin")
        {
            Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/icon.png");
            SkillsPageIcon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/icon.png");

            ExperienceCurve = [100, 380, 770, 1300, 2150, 3300, 4800, 6900, 10000, 15000];

            ExperienceBarColor = new Microsoft.Xna.Framework.Color(252, 121, 27);

            // Level 5
            ProfessionShieldThrowHit2 = new GenericProfession(skill: this, id: "ThrowHit2", name: I18n.PaladinSkill_Profession_ShieldThrowHit2_Name, description: I18n.PaladinSkill_Profession_ShieldThrowHit2_Description)
            {
                Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/OnYourLeft.png")
            };
            Professions.Add(ProfessionShieldThrowHit2);

            ProfessionShieldArmor1 = new GenericProfession(skill: this, id: "Armor1", name: I18n.PaladinSkill_Profession_ShieldArmor1_Name, description: I18n.PaladinSkill_Profession_ShieldArmor1_Description)
            {
                Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/FightToEndTheFight.png")
            };
            Professions.Add(ProfessionShieldArmor1);

            ProfessionsForLevels.Add(new ProfessionPair(5, ProfessionShieldThrowHit2, ProfessionShieldArmor1));

            // Level 10 - track A
            ProfessionShieldThrowLightning = new GenericProfession(skill: this, id: "ThrowLightning", name: I18n.PaladinSkill_Profession_ShieldThrowLightning_Name, description: I18n.PaladinSkill_Profession_ShieldThrowLightning_Description)
            {
                Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/TheSunWillShineOnUsAgain.png")
            };
            Professions.Add(ProfessionShieldThrowLightning);

            ProfessionShieldThrowHit3 = new GenericProfession(skill: this, id: "ThrowHit3", name: () => I18n.PaladinSkill_Profession_ShieldThrowHit3_Name(Game1.player.Name), description: I18n.PaladinSkill_Profession_ShieldThrowHit3_Description)
            {
                Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/TheyHaveAnArmyWeHaveAFarmerName.png")
            };
            Professions.Add(ProfessionShieldThrowHit3);

            ProfessionsForLevels.Add(new ProfessionPair(10, ProfessionShieldThrowLightning, ProfessionShieldThrowHit3, ProfessionShieldThrowHit2));

            // Level 10 - track B
            ProfessionShieldArmor2 = new GenericProfession(skill: this, id: "Armor2", name: I18n.PaladinSkill_Profession_ShieldArmor2_Name, description: I18n.PaladinSkill_Profession_ShieldArmor2_Description)
            {
                Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/ICanDoThisAllDay.png")
            };
            Professions.Add(ProfessionShieldArmor2);

            ProfessionShieldRetribution = new GenericProfession(skill: this, id: "Retribution", name: I18n.PaladinSkill_Profession_ShieldRetribution_Name, description: I18n.PaladinSkill_Profession_ShieldRetribution_Description)
            {
                Icon = ModSnS.Instance.Helper.ModContent.Load<Texture2D>("assets/paladin/YouGetHurtHurtEmBack.png")
            };
            Professions.Add(ProfessionShieldRetribution);

            ProfessionsForLevels.Add(new ProfessionPair(10, ProfessionShieldArmor2, ProfessionShieldRetribution, ProfessionShieldArmor1));
        }

        public override string GetName()
        {
            return I18n.PaladinSkill_Name();
        }
        public override void DoLevelPerk(int level)
        {
            base.DoLevelPerk(level);

            if (level > 10) return; // Walk of Life

            Game1.player.maxHealth += 5;
        }

        public override List<string> GetExtraLevelUpInfo(int level)
        {
            List<string> ret = [];

            if (level > 10) return ret;

            if (level % 5 != 0)
                ret.Add(I18n.Level_Health(5) + "\n");

            switch (level)
            {
                case 2:
                    ret.Add(I18n.PaladinSkill_Unlock_2().Replace('^', '\n'));
                    break;
                case 4:
                    ret.Add(I18n.PaladinSkill_Unlock_4().Replace('^', '\n'));
                    break;
                case 6:
                    ret.Add(I18n.PaladinSkill_Unlock_6().Replace('^', '\n'));
                    break;
                case 8:
                    ret.Add(I18n.PaladinSkill_Unlock_8().Replace('^', '\n'));
                    break;
            }

            return ret;
        }

        public override string GetSkillPageHoverText(int level)
        {
            return I18n.Level_Health(5 * level);
        }
        public override bool ShouldShowOnSkillsPage => Game1.player.eventsSeen.Any(e => e.StartsWith("SnS.Ch4.Intermission.")) || ModSnS.Config.EarlyPaladinUnlock;
    }

    [HarmonyPatch(typeof(LevelUpMenu), nameof(LevelUpMenu.RevalidateHealth))]
    public static class LevelUpMenuRevalidateHealthPatchAgain
    {
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> insns)
        {
            var ret = new List<CodeInstruction>();

            int emhIndex = ModSnS.SpaceCore.GetLocalIndexForMethod(original, "expected_max_health")[0];

            bool inserted = false;
            foreach (var insn in insns)
            {
                if (insn.opcode == OpCodes.Ldfld && (FieldInfo)insn.operand == AccessTools.Field(typeof(Farmer), nameof(Farmer.maxHealth)))
                {
                    if (!inserted)
                    {
                        ret.InsertRange(ret.Count - 1,
                            [
                                new CodeInstruction(OpCodes.Ldloc, emhIndex),
                                new CodeInstruction(OpCodes.Ldarg_0),
                                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LevelUpMenuRevalidateHealthPatchAgain), nameof(GetExtraHealth) ) ),
                                new CodeInstruction(OpCodes.Add),
                                new CodeInstruction(OpCodes.Stloc, emhIndex)
                            ]);
                        inserted = true;
                    }
                }

                ret.Add(insn);
            }

            return ret;
        }

        public static int GetExtraHealth(Farmer farmer)
        {
            return farmer.GetCustomSkillLevel(ModTOP.PaladinSkill) * 5;
        }
    }

    [HarmonyPatch(typeof(SpecialOrder), nameof(SpecialOrder.Update))]
    public static class PaladinExpPatch1
    {
        public static void Postfix(SpecialOrder __instance)
        {
            if (!ModTOP.PaladinSkill.ShouldShowOnSkillsPage || __instance.questState.Value != SpecialOrderStatus.Complete || !__instance.readyForRemoval.Value)
                return;
            Game1.player.AddCustomSkillExperience(ModTOP.PaladinSkill, 250);
        }
    }

    [HarmonyPatch(typeof(Quest), nameof(Quest.questComplete))]
    public static class PaladinExpPatch2
    {
        public static void Postfix(Quest __instance)
        {
            if (!ModTOP.PaladinSkill.ShouldShowOnSkillsPage || !__instance.completed.Value)
                return;
            Game1.player.AddCustomSkillExperience(ModTOP.PaladinSkill, 100);
        }
    }

    [HarmonyPatch(typeof(NPC), nameof(NPC.receiveGift))]
    public static class PaladinExpPatch3
    {
        public static void Postfix(NPC __instance, Object o)
        {
            if (!ModTOP.PaladinSkill.ShouldShowOnSkillsPage)
                return;

            int taste = __instance.getGiftTasteForThisItem(o);
            switch (taste)
            {
                case NPC.gift_taste_stardroptea:
                    Game1.player.AddCustomSkillExperience(ModTOP.PaladinSkill, 250 / 5);
                    break;
                case NPC.gift_taste_love:
                    Game1.player.AddCustomSkillExperience(ModTOP.PaladinSkill, 80 / 5);
                    break;
                case NPC.gift_taste_like:
                    Game1.player.AddCustomSkillExperience(ModTOP.PaladinSkill, 45 / 5);
                    break;
                case NPC.gift_taste_neutral:
                    Game1.player.AddCustomSkillExperience(ModTOP.PaladinSkill, 20 / 5);
                    break;
            }
        }
    }
}