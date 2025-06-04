using SwordAndSorcerySMAPI.Framework.MercenaryPort;
using System;
using System.Collections.Generic;

namespace SwordAndSorcerySMAPI.Framework.MercenaryPort.Actions
{
    public class MercenaryActionData
    {
        public string Id { get; set; }
        public string ActionType { get; set; }

        public int Priority { get; set; }
        public int Weight { get; set; } = 1;

        //public JObject Parameters { get; set; } = new();

        public string AdditionalConditions { get; set; }

        public Dictionary<string, float> Cooldowns { get; set; } = new();
        public float ActionLength { get; set; }

        public static Dictionary<string, Func<Mercenary, MercenaryActionData, bool>> ActionTypes { get; internal set; } = new();
    }

    public class MeleeAttackMercenaryActionParameters
    {
        public int MaxEngagementRadius { get; set; } = 10;
        public int MinIgnoreRadius { get; set; } = 20;

        public string MeleeWeaponId { get; set; }

        public List<string> WeaponEnchantments { get; set; } = new();
        public Dictionary<string, string> WeaponModData { get; set; } = new();
        public bool ShowWeapon = true;

        public List<string> MonsterBlockList { get; set; } = new();
        public List<string> MonsterAllowList { get; set; } // null means everything is allowed
    }
}
