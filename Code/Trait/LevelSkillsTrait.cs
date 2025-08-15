using ModTemplate.Code.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModTemplate.Code.Trait
{
    internal class LevelSkillsTrait
    {
        public static void init()
        {
            // 斗气化马 - 移速+30
            ActorTrait Skill1 = BaseTraitTool.CreateTrait("trait_Skills_1", "icon.png", "interesting3");
            AssetManager.traits.add(Skill1);

            // 斗气化铠 - 攻击+5-10%,防御+3-7%,移速-15,攻速-10
            ActorTrait Skill2 = BaseTraitTool.CreateTrait("trait_Skills_2", "icon.png", "interesting3");
            AssetManager.traits.add(Skill2);

            // 斗气化形 - 无特殊属性
            ActorTrait Skill3 = BaseTraitTool.CreateTrait("trait_Skills_3", "icon.png", "interesting3");
            AssetManager.traits.add(Skill3);

            // 斗气化翼 - 移速+100,攻速+20
            ActorTrait Skill4 = BaseTraitTool.CreateTrait("trait_Skills_4", "icon.png", "interesting3");
            AssetManager.traits.add(Skill4);

            // 斗气破空 - 攻击+10-30%,攻速+30
            ActorTrait Skill5 = BaseTraitTool.CreateTrait("trait_Skills_5", "icon.png", "interesting3");
            AssetManager.traits.add(Skill5);

            // 斗气裂空
            ActorTrait Skill6 = BaseTraitTool.CreateTrait("trait_Skills_6", "icon.png", "interesting3");
            AssetManager.traits.add(Skill6);

            // 斗气圣域 - 生命值+30%
            ActorTrait Skill7 = BaseTraitTool.CreateTrait("trait_Skills_7", "icon.png", "interesting3");
            AssetManager.traits.add(Skill7);

            // 斗气终极 - 防御固定99%,攻击+50%
            ActorTrait Skill8 = BaseTraitTool.CreateTrait("trait_Skills_8", "icon.png", "interesting3");
            AssetManager.traits.add(Skill8);
        }

        //设置属性
        private static void SafeSetStat(BaseStats baseStats, string statKey, float value)
        {
            baseStats[statKey] = value;
        }
    }
}
