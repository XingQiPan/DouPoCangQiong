using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    internal class SkillsLibrary
    {
        public static Dictionary<string, SkillAsset> spells = new Dictionary<string, SkillAsset>();

        public static void init()
        {
            // 注册 “次级治疗术”
            registerSpell(new SkillAsset
            {
                id = "minor_heal",
                name = "Minor Heal",
                description = "Heals a friendly target for a small amount.",
                mana_cost = 20,
                cooldown = 1,
                action = SkillsActionLibrary.castMinorHeal,
                condition = SkillConditionsLibrary.SelfHealWhenHealthLow
            });

            // 注册 “火焰飞镖”
            registerSpell(new SkillAsset
            {
                id = "my_mod_fire_dart",
                name = "Fire Dart",
                description = "Shoots a small fire dart at an enemy.",
                mana_cost = 15,
                cooldown = 3,
                action = SkillsActionLibrary.castFireDart,
                condition = SkillConditionsLibrary.HasValidEnemyTarget
            });
        }

        private static void registerSpell(SkillAsset pSpell)
        {
            spells.Add(pSpell.id, pSpell);
        }

        public static SkillAsset get(string pID)
        {
            spells.TryGetValue(pID, out SkillAsset result);
            return result;
        }
    }
}
