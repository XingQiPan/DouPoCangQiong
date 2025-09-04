using DdouPoCangPong.Code.Tool;
using NeoModLoader.General;
using System.Collections.ObjectModel;
using System;
using UnityEngine;

namespace DdouPoCangPong.Code
{
    internal class Cultisys
    {
        public const int MaxLevel = 99;
        private static readonly BaseStats[] _level_stats;
        private static readonly float[] _level_exp_required;
        private static readonly float[] _dou_qi_required;
        private static readonly float[] _minor_star_breakthrough_chance;
        private static readonly float[] _major_realm_breakthrough_chance;

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
        public static ReadOnlyCollection<float> LevelDouQiRequired { get; }
        public static ReadOnlyCollection<float> MinorStarBreakthroughChance { get; }
        public static ReadOnlyCollection<float> MajorRealmBreakthroughChance { get; }

        static Cultisys()
        {
            _level_stats = new BaseStats[MaxLevel + 1];
            _level_exp_required = new float[MaxLevel + 1];
            _dou_qi_required = new float[MaxLevel + 1];

            for (var i = 0; i <= MaxLevel; i++)
            {
                _level_stats[i] = new BaseStats();
            }

            int[] health_realms = { 20, 105, 443, 1642, 4567, 11699, 27327, 58532, 134209, 301907, 614211 };
            int[] lifespan_realms = { 100, 100, 120, 160, 220, 300, 400, 550, 800, 2000, 10000 };
            float[] exp_realms = { 25, 67, 180, 474, 1192, 2689, 5000, 6792, 8176, 8808, 10000 };
            float[] dou_qi_realms = { 25, 67, 180, 474, 1192, 2689, 5000, 6792, 8176, 8808, 10000 };

            // 定义概率数据
            float[] minor_chances = { 1.0f, 1.0f, 1.0f, 0.95f, 0.90f, 0.80f, 0.75f, 0.70f, 0.60f, 0.50f, 0.25f };
            float[] major_chances = { 1.0f, 1.0f, 0.90f, 0.80f, 0.50f, 0.20f, 0.10f, 0.05f, 0.02f, 0.005f, 0.0001f };

            for (int i = 0; i < MaxLevel; i++)
            {
                int realm_index = i / 9;
                int star_level = i % 9 + 1;

                int prev_health = realm_index == 0 ? 0 : health_realms[realm_index - 1];
                int prev_lifespan = realm_index == 0 ? 100 : lifespan_realms[realm_index - 1];

                int target_health = health_realms[realm_index];
                int target_lifespan = lifespan_realms[realm_index];

                float health_step = (target_health - prev_health) / 9.0f;
                float lifespan_step = (target_lifespan - prev_lifespan) / 9.0f;

                BaseStats current_stats = _level_stats[i];

                current_stats[S.health] = (int)(prev_health + health_step * star_level);
                current_stats[S.lifespan] = (int)(prev_lifespan + lifespan_step * star_level);
                current_stats[S.damage] = (int)(current_stats[S.health] * 0.04f);
            }

            for (int i = 0; i < MaxLevel; i++)
            {
                int realm_index = i / 9;
                if (realm_index < exp_realms.Length)
                {
                    _level_exp_required[i] = exp_realms[realm_index];
                    _dou_qi_required[i] = dou_qi_realms[realm_index];
                }
            }
            _level_exp_required[MaxLevel] = 0;

            _minor_star_breakthrough_chance = new float[MaxLevel];
            _major_realm_breakthrough_chance = new float[MaxLevel];
            for (int i = 0; i < MaxLevel; i++)
            {
                int realm_index = i / 9;
                _minor_star_breakthrough_chance[i] = minor_chances[realm_index];
                _major_realm_breakthrough_chance[i] = major_chances[realm_index];
            }

            LevelExpRequired = new ReadOnlyCollection<float>(_level_exp_required);
            LevelDouQiRequired = new ReadOnlyCollection<float>(_dou_qi_required);
            LevelStats = new ReadOnlyCollection<BaseStats>(_level_stats);
            MinorStarBreakthroughChance = new ReadOnlyCollection<float>(_minor_star_breakthrough_chance);
            MajorRealmBreakthroughChance = new ReadOnlyCollection<float>(_major_realm_breakthrough_chance);
        }

        public static string GetName(int level)
        {
            level = Math.Min(MaxLevel, Math.Max(level, 0));
            return LM.Get($"trait_Grade{level}");
        }
    }
}