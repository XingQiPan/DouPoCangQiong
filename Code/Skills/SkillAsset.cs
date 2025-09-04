using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Skills
{
    // 定义技能动作的委托
    public delegate void SpellAction(Actor pCaster, Actor pTarget);

    // 新增：定义技能使用条件的委托
    // 这个委托会判断技能是否应该使用，并通过 out 参数返回选定的目标
    // 返回 true 表示可以使用，false 表示不满足条件
    public delegate bool SpellCondition(Actor pCaster, out Actor pTarget);

    /// <summary>
    /// 新增：斗技类型枚举
    /// </summary>
    public enum SkillType
    {
        Sword,      // 剑系
        FistPalm,   // 拳掌系
        Saber,      // 刀系
        Fire,       // 火系
        Water,      // 水系
        Wood,       // 木系
        Earth,      // 土系
        Metal,      // 金系
        Space       // 空间系
    }

    public class SkillAsset
    {
        public string id;
        public string name;
        public string description;

        // 新增：斗技类型
        public SkillType type;

        //消耗蓝量
        public int mana_cost = 10;
        //冷却（年）
        public int cooldown = 5;

        //提升潜力
        public int qianli = 1;

        public SpellAction action;

        public SpellCondition condition;

    }
}
