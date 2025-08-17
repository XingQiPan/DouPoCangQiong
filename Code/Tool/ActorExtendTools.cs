using CHANGEME;
using NeoModLoader.api.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModTemplate.Code.Tool
{
    public static class ActorExtendTools
    {
        private const string cultisys_level_key = $"d_level";

        private const string exp_key = $"d_exp";

        private const string talent_key = $"d_talent";

        public static int GetCultisysLevel(this Actor actor)
        {
            actor.data.get(cultisys_level_key, out int level);
            return level;
        }

        public static float GetExp(this Actor actor)
        {
            actor.data.get(exp_key, out float exp);
            return exp;
        }
        public static float GetModTalent(this Actor actor)
        {
            actor.data.get(Stats.mod_talent.id, out float mod_talent);
            return mod_talent + actor.stats[Stats.mod_talent.id];
        }
        public static void IncExp(this Actor actor, float value)
        {
            actor.data.set(exp_key, actor.GetExp() + value);
        }
        [Hotfixable]
        public static void ResetExp(this Actor actor,float exp)
        {
            actor.data.set(exp_key, actor.GetExp() - exp);
        }

        [Hotfixable]
        public static void LevelUp(this Actor actor)
        {
            var level = actor.GetCultisysLevel();
            if (level >= Cultisys.MaxLevel) return;
            level++;
            actor.data.set(cultisys_level_key, level);

            actor.event_full_stats = true;
            actor.setStatsDirty();
        }

        public static void SetLevel(this Actor actor, int level)
        {
            level = Mathf.Clamp(level, 0, 99);
            actor.data.set(cultisys_level_key, level);
            actor.setStatsDirty();
        }

        public static void SetTalent(this Actor actor, float talent)
        {
            talent = Mathf.Max(talent, 0);
            actor.data.set(talent_key, talent);
        }

        //基值为50
        //黄品40%（保底斗灵）
        //玄品280%（保底斗王，概率斗皇）
        //地品550%的基础修炼速度加成（保底斗宗，概率斗圣）
        //天品800%（保底斗尊，概率斗帝）
        //1~30是不入品，31~60是黄品，61~80是玄品，81~95是地品，96~100是天品
        public static float GetTalent(this Actor actor)
        {
            actor.data.get(talent_key, out float talent, -1);
            if(talent > 0) return talent;
            if (actor.asset.is_boat) return 0;
            //int _talent = Randy.randomInt(0, 1000);
            int _talent = 0;
            // 建议生成一个 0.0f 到 1.0f 之间的随机数，这样更容易计算
            float num = Randy.randomFloat(0.0f, 1.0f);

            // 天品: 0.001% 的概率
            if (num < 0.00001f)
            {
                _talent = Randy.randomInt(701, 1000);
            }
            // 地品: 0.099% 的概率
            else if (num < 0.00001f + 0.00099f) // 等同于 num < 0.001f
            {
                _talent = Randy.randomInt(401, 700);
            }
            // 玄品: 9.91% 的概率
            else if (num < 0.001f + 0.0991f) // 等同于 num < 0.1001f
            {
                _talent = Randy.randomInt(101, 401);
            }
            // 黄品: 剩下 89.99% 的概率
            else
            {
                _talent = Randy.randomInt(1, 100);
            }
            //actor.addTrait("trait_Flair_0");
            //Debug.Log($"{actor.getName()}天赋：{_talent}");
            if (_talent >= 400)
            {
                actor.data.favorite = true;
            }
            SetTalent(actor,_talent);
            return _talent;
        }
    }
}
