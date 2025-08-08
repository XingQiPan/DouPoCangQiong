using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static Cultisys()
        {
            _level_stats = new BaseStats[MaxLevel + 1];
            _level_exp_required = new float[MaxLevel + 1];
            for (var i = 0; i <= MaxLevel; i++) _level_stats[i] = new BaseStats();

            for (var i = 0; i < MaxLevel; i++)
            {
                _level_exp_required[i] =
                    i < 9 ? 25     // 境界1：等级0-8（9层）
                    : i < 18 ? 67   // 境界2：等级9-17（9层）
                    : i < 27 ? 180  // 境界3：等级18-26（9层）
                    : i < 36 ? 474  // 境界4：等级27-35（9层）
                    : i < 45 ? 1192 // 境界5：等级36-44（9层）
                    : i < 54 ? 2689 // 境界6：等级45-53（9层）
                    : i < 63 ? 5000 // 境界7：等级54-62（9层）
                    : i < 72 ? 6792 // 境界8：等级63-71（9层）
                    : i < 81 ? 8176 // 境界9：等级72-80（9层）
                    : i < 90 ? 8808 // 境界10：等级81-89（9层）
                    : 10000;        // 境界11：等级90-98（9层）
            }

            BaseStats stats = _level_stats[0];
            for (var i = 0;i < MaxLevel; i++)
            {
                int level = i + 1;
                stats = _level_stats[i];
                stats[S.health] += LevelTool.GetLevelHealth(level);
                stats[S.damage] += LevelTool.GetDamage(level);
                stats[S.lifespan] += LevelTool.GetAge(level);
            }
        }
    }
}
