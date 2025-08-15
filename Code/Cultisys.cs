using ModTemplate.Code.Tool;
using NeoModLoader.General;
using System.Collections.ObjectModel;
using System;
using UnityEngine;

namespace ModTemplate.Code
{
    internal class Cultisys
    {
        public const int MaxLevel = 99;
        private static readonly BaseStats[] _level_stats;
        private static readonly float[] _level_exp_required;

        private static readonly string[] _traits_blacklist =
        {
            "eyepatch", "crippled", "cursed", "tumorInfection", "mushSpores", "plague", "skin_burns"
        };

        private static readonly string[] _statuses_blacklist =
        {
            "cough", "ash_fever"
        };

        public static ReadOnlyCollection<BaseStats> LevelStats { get; }
        public static ReadOnlyCollection<float> LevelExpRequired { get; }

        static Cultisys()
        {
            _level_stats = new BaseStats[MaxLevel + 1];
            _level_exp_required = new float[MaxLevel + 1];
            for (var i = 0; i <= MaxLevel; i++)
            {
                _level_stats[i] = new BaseStats();
            }

            // 对应 斗之气, 斗者, 斗师, 大斗师, 斗灵, 斗王, 斗皇, 斗宗, 斗尊, 斗圣, 斗帝 的基础经验
            int[] exp_realms = { 500, 1000, 2000, 4000, 9000, 4000, 4000, 25000, 9000, 9000, 10000 };

            for (int i = 0; i < MaxLevel; i++)
            {
                int realm_index = i / 9; // 0-8级是第0个境界, 9-17是第1个, 以此类推
                if (realm_index < exp_realms.Length)
                {
                    _level_exp_required[i] = exp_realms[realm_index];
                    //Debug.Log(_level_exp_required[i]);
                }
            }
            _level_exp_required[MaxLevel] = 0; // 满级经验为0

            LevelExpRequired = new ReadOnlyCollection<float>(_level_exp_required);
            LevelStats = new ReadOnlyCollection<BaseStats>(_level_stats);

            for (var i = 0; i < MaxLevel; i++)
            {
                int current_level = i + 1;
                BaseStats current_stats = _level_stats[i];

                current_stats[S.health] += LevelTool.GetLevelHealth(current_level);
                current_stats[S.damage] += LevelTool.GetDamage((int)current_stats[S.health]);
                current_stats[S.lifespan] += LevelTool.GetAge(current_level);
            }
        }

        public static string GetName(int level)
        {
            level = Math.Min(MaxLevel, Math.Max(level, 0));
            return LM.Get($"trait_Grade{level}");
        }
    }
}