using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    /// <summary>
    /// 存放所有技能的使用条件和目标选择逻辑.
    /// </summary>
    internal class SkillConditionsLibrary
    {
        /// <summary>
        /// 条件：当施法者生命值低于70%时，选择自己为目标。
        /// 用于 "次级治疗术".
        /// </summary>
        public static bool SelfHealWhenHealthLow(Actor pCaster, out Actor pTarget)
        {
            if (pCaster.data.health < pCaster.getMaxHealth() * 0.7f)
            {
                pTarget = pCaster;
                return true; // 满足条件
            }

            pTarget = null;
            return false; // 不满足条件
        }

        /// <summary>
        /// 条件：当存在一个有效的敌对攻击目标时，选择该目标。
        /// 用于 "火焰飞镖".
        /// </summary>
        public static bool HasValidEnemyTarget(Actor pCaster, out Actor pTarget)
        {
            pTarget = (Actor)pCaster.attack_target;

            // 确保目标存在，没有死亡，并且是敌人
            if (pTarget != null && !pTarget.isAlive() && pCaster.areFoes(pTarget))
            {
                return true;
            }

            pTarget = null;
            return false;
        }
    }
}
