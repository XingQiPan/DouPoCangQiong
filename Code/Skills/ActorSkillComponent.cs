using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    internal class ActorSkillComponent
    {
        private Actor owner;
        public List<SkillAsset> known_spells = new List<SkillAsset>();
        private Dictionary<string, float> spell_cooldowns = new Dictionary<string, float>();

        public ActorSkillComponent(Actor pOwner)
        {
            this.owner = pOwner;
        }

        // 给 Actor 添加一个技能
        public void LearnSpell(string pSpellID)
        {
            SkillAsset spell = SkillsLibrary.get(pSpellID);
            if (spell != null && !known_spells.Any(s => s.id == pSpellID))
            {
                known_spells.Add(spell);
            }
        }

        // 新增：让 Actor 忘记一个技能
        public void ForgetSpell(string pSpellID)
        {
            var spellToRemove = known_spells.FirstOrDefault(s => s.id == pSpellID);
            if (spellToRemove != null)
            {
                known_spells.Remove(spellToRemove);
            }
        }

        // 更新冷却时间 (每帧调用)
        public void Update(float pElapsed)
        {
            if (spell_cooldowns.Count == 0) return;

            List<string> keys = new List<string>(spell_cooldowns.Keys);
            foreach (string key in keys)
            {
                spell_cooldowns[key] -= pElapsed;
                if (spell_cooldowns[key] <= 0)
                {
                    spell_cooldowns.Remove(key);
                }
            }
        }

        public bool IsOnCooldown(string pSpellID)
        {
            return spell_cooldowns.ContainsKey(pSpellID);
        }

        public float GetCooldownRemaining(string pSpellID)
        {
            if (spell_cooldowns.TryGetValue(pSpellID, out float remaining))
            {
                return remaining;
            }
            return 0f;
        }

        // 检查一个技能是否可以使用
        public bool CanCastSpell(SkillAsset pSpell, Actor pTarget)
        {
            if (pSpell == null) return false;
            if (IsOnCooldown(pSpell.id)) return false;
            if (owner.getMana() < pSpell.mana_cost) return false;
            return true;
        }

        // 施放技能
        public void CastSpell(SkillAsset pSpell, Actor pTarget)
        {
            if (!CanCastSpell(pSpell, pTarget)) return;

            owner.spendMana(pSpell.mana_cost);
            spell_cooldowns[pSpell.id] = pSpell.cooldown;
            pSpell.action(owner, pTarget);

            owner.doCastAnimation();
        }
    }
}
