// DdouPoCangPong.Code.Tool/ActorSkillTools.cs

using DdouPoCangPong.Code.Skills;
using System.Collections.Generic;
using System.Linq;

namespace DdouPoCangPong.Code.Tool
{
    /// <summary>
    /// 提供一系列用于方便操作 Actor 技能系统的扩展方法.
    /// </summary>
    public static class ActorSkillTools
    {
        /// <summary>
        /// 获取指定 Actor 的技能组件. 这是一个内部辅助方法.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <returns>技能组件实例，如果不存在则返回 null.</returns>
        private static ActorSkillComponent GetSkillComponent(this Actor actor)
        {
            return ActorSkillManager.GetComponent(actor);
        }

        /// <summary>
        /// 获取一个 Actor 学会的所有技能列表.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <returns>一个 SkillAsset 列表. 如果角色没有技能组件，则返回一个空列表.</returns>
        public static List<SkillAsset> GetKnownSpells(this Actor actor)
        {
            var component = actor.GetSkillComponent();
            // 如果组件不存在，返回一个空的列表以避免空引用异常
            return component?.known_spells ?? new List<SkillAsset>();
        }

        /// <summary>
        /// 判断一个 Actor 是否学会了指定的技能.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <param name="pSpellID">技能的唯一ID</param>
        /// <returns>如果学会了该技能则返回 true, 否则返回 false.</returns>
        public static bool HasSpell(this Actor actor, string pSpellID)
        {
            var component = actor.GetSkillComponent();
            if (component == null) return false;

            // 使用 LINQ 的 Any 方法高效查找
            return component.known_spells.Any(spell => spell.id == pSpellID);
        }

        /// <summary>
        /// 让一个 Actor 学习一个新技能.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <param name="pSpellID">要学习的技能的唯一ID</param>
        public static void LearnSpell(this Actor actor, string pSpellID)
        {
            // 使用 ?. 操作符安全地调用方法，如果组件为null则不执行任何操作
            actor.GetSkillComponent()?.LearnSpell(pSpellID);
        }

        /// <summary>
        /// 让一个 Actor 忘记一个已学会的技能.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <param name="pSpellID">要忘记的技能的唯一ID</param>
        public static void ForgetSpell(this Actor actor, string pSpellID)
        {
            actor.GetSkillComponent()?.ForgetSpell(pSpellID);
        }

        /// <summary>
        /// 检查指定技能当前是否处于冷却状态.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <param name="pSpellID">技能的唯一ID</param>
        /// <returns>如果正在冷却则返回 true, 否则返回 false.</returns>
        public static bool IsSpellOnCooldown(this Actor actor, string pSpellID)
        {
            // 使用 ?? (null合并) 操作符，如果组件为null，安全地返回 false
            return actor.GetSkillComponent()?.IsOnCooldown(pSpellID) ?? false;
        }

        /// <summary>
        /// 获取指定技能剩余的冷却时间.
        /// </summary>
        /// <param name="actor">目标 Actor</param>
        /// <param name="pSpellID">技能的唯一ID</param>
        /// <returns>剩余的冷却时间(秒). 如果技能未冷却或不存在，则返回 0.</returns>
        public static float GetSpellCooldownRemaining(this Actor actor, string pSpellID)
        {
            return actor.GetSkillComponent()?.GetCooldownRemaining(pSpellID) ?? 0f;
        }

        /// <summary>
        /// 尝试为一个 Actor 施放一个技能.
        /// 该方法会检查所有施法条件 (冷却, 法力等).
        /// </summary>
        /// <param name="actor">施法者</param>
        /// <param name="pSpellID">技能的唯一ID</param>
        /// <param name="pTarget">技能目标</param>
        /// <returns>如果施法成功则返回 true, 否则返回 false.</returns>
        public static bool TryCastSpell(this Actor actor, string pSpellID, Actor pTarget)
        {
            var component = actor.GetSkillComponent();
            if (component == null) return false;

            var spellAsset = SkillsLibrary.get(pSpellID);
            if (spellAsset == null) return false;

            // 复用组件内已有的逻辑
            if (component.CanCastSpell(spellAsset, pTarget))
            {
                component.CastSpell(spellAsset, pTarget);
                return true;
            }

            return false;
        }
    }
}