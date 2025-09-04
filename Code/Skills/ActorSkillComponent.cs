using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DdouPoCangPong.Code.Skills
{
    internal class ActorSkillComponent
    {
        private Actor owner;
        public List<LearnedSkill> known_spells = new List<LearnedSkill>();
        private Dictionary<string, float> spell_cooldowns = new Dictionary<string, float>();

        public ActorSkillComponent(Actor pOwner)
        {
            this.owner = pOwner;
        }

        /// <summary>
        /// 学习技能时，创建 LearnedSkill 实例
        /// </summary>
        /// <param name="pSpellID"></param>
        /// <param name="quality"></param>
        public void LearnSpell(string pSpellID, GongFa.GongFa.GongFaRank quality)
        {
            SkillAsset spellAsset = SkillsLibrary.get(pSpellID);
            if (spellAsset != null && !known_spells.Any(s => s.id == pSpellID))
            {
                var newLearnedSkill = new LearnedSkill(pSpellID, quality);
                known_spells.Add(newLearnedSkill);
            }
        }

        /// <summary>
        /// 忘记技能
        /// </summary>
        /// <param name="pSpellID"></param>
        public void ForgetSpell(string pSpellID)
        {
            var spellToRemove = known_spells.FirstOrDefault(s => s.id == pSpellID);
            if (spellToRemove != null)
            {
                known_spells.Remove(spellToRemove);
            }
        }

        /// <summary>
        /// 更新冷却时间
        /// </summary>
        /// <param name="pElapsed"></param>
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

        /// <summary>
        /// 检查一个技能是否可以使用
        /// </summary>
        /// <param name="pLearnedSkill"></param>
        /// <param name="pTarget"></param>
        /// <returns></returns>
        public bool CanCastSpell(LearnedSkill pLearnedSkill, Actor pTarget)
        {
            if (pLearnedSkill == null) return false;
            var spellAsset = pLearnedSkill.GetAsset();
            if (spellAsset == null) return false;

            if (IsOnCooldown(spellAsset.id)) return false;
            if (owner.getHealth() < spellAsset.mana_cost) return false;
            return true;
        }

        /// <summary>
        /// 施放技能，并增加熟练度
        /// </summary>
        /// <param name="pLearnedSkill"></param>
        /// <param name="pTarget"></param>
        public bool CastSpell(LearnedSkill pLearnedSkill, Actor pTarget)
        {
            Debug.Log("释放法术");
            if (!CanCastSpell(pLearnedSkill, pTarget)) return false;

            var spellAsset = pLearnedSkill.GetAsset();

            owner.spendMana(spellAsset.mana_cost);
            spell_cooldowns[spellAsset.id] = spellAsset.cooldown;
            spellAsset.action(owner, pTarget);

            // 新增：每次成功施法，增加熟练度
            pLearnedSkill.AddProficiency(Randy.randomInt(10, 25));

            owner.doCastAnimation();

            return true;
        }
    }
}