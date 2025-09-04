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

    public class SkillAsset
    {
        public string id;
        public string name;
        public string description;

        //消耗蓝量
        public int mana_cost = 10;
        //冷却（年）
        public int cooldown = 5;

        //提升潜力
        public int qianli = 1;

        public SpellAction action;

        public SpellCondition condition;

        // 以后可以扩展更多属性:
        // public float cast_range;
        // public string required_trait;
        // public string effect_id; 
    }
}
