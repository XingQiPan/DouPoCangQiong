using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    internal class SkillsActionLibrary
    {
        // “次级治疗术”的实现
        public static void castMinorHeal(Actor pCaster, Actor pTarget)
        {
            // 检查目标是否存在且是友方
            if (pTarget == null || pCaster.areFoes(pTarget)) return;

            // 恢复生命值
            int heal_amount = 30 + pCaster.intelligence; // 治疗量受智力影响
            pTarget.restoreHealth(heal_amount);

            // 播放一个治疗特效
            EffectsLibrary.spawnAt("fx_cast_ground_green", pTarget.current_position, pTarget.actor_scale);
            pTarget.spawnParticle(Toolbox.color_heal); // 在目标身上产生粒子
        }

        // “火焰飞镖”的实现
        public static void castFireDart(Actor pCaster, Actor pTarget)
        {
            // 检查目标是否存在且是敌人
            if (pTarget == null || !pCaster.areFoes(pTarget)) return;

            // 生成一个火球投掷物
            World.world.projectiles.spawn(
                pCaster,                   // 投掷者
                pTarget,                   // 目标
                "fireball",                // 使用游戏自带的火球预设
                pCaster.getThrowStartPosition(), // 起始位置
                pTarget.current_position,  // 目标位置
                0f,                        // 角度偏移
                0.1f                       // 缩放
            );
        }
    }
}
