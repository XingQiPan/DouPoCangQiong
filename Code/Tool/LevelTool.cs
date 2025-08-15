using System;

namespace ModTemplate.Code.Tool
{
    internal class LevelTool
    {
        // base_num 似乎没有被使用，如果不需要可以移除
        public static int base_num = 10000;

        /// <summary>
        /// 获取当前境界血量
        /// </summary>
        public static int GetLevelHealth(int level)
        {
            // 增加一个基础的有效性检查
            if (level <= 0) return 0;

            // 计算当前层数（1-9）
            int ceng = (level - 1) % 9 + 1;

            // 计算当前境界（1-11），这个逻辑是正确的
            int jingjie = ((level - 1) / 9) + 1;

            // 由于下面的SumAddtionRate已经修复，这里现在是安全的
            return (int)(jingjie + Math.Pow(jingjie + 1, 4) * (1 + SumAddtionRate(jingjie, ceng)));
        }

        public static int GetDamage(int health)
        {
            // 建议检查 health 是否为负数，以避免意外结果
            if (health < 0) return 0;
            return (int)(health * 0.04f);
        }

        public static int GetMagic(int level)
        {
            return (int)Math.Pow(10, level - 1);
        }

        /// <summary>
        /// 获取年龄
        /// </summary>
        public static int GetAge(int level)
        {
            // 【安全修复】 这是导致游戏崩溃的关键点之一。
            // 在访问数组前，必须检查索引是否在有效范围内。
            // 1. 检查 D.Ages 是否存在 (不为 null)
            // 2. 检查 level 是否大于等于0
            // 3. 检查 level 是否小于数组的长度
            if (D.Ages != null && level >= 0 && level < D.Ages.Count)
            {
                return D.Ages[level];
            }

            // 如果上面的条件不满足（比如 level 是 99，但 D.Ages 数组长度只有 50），
            // 就返回一个安全的默认值，而不是让游戏崩溃。
            // 你可以把 500 改成你认为合适的任何默认年龄。
            return 500;
        }

        /// <summary>
        /// 获取总增幅
        /// </summary>
        /// <param name="jingjie">这里传入的参数是境界（最大为11）</param>
        /// <param name="ceng">层数</param>
        public static float SumAddtionRate(int jingjie, int ceng)
        {
            // 【安全修复】 这是另一个关键崩溃点。
            // 首先检查 D.additions 数组是否存在。
            if (D.additions == null || D.additions.Count == 0)
            {
                // 如果数组不存在或为空，直接返回0，避免后续计算出错。
                return 0f;
            }

            float sum = 0;
            // 循环计算之前所有境界的总增幅
            for (int i = 1; i < jingjie; i++)
            {
                int index = i - 1;
                // 在循环内部也进行检查，确保索引不会越界
                if (index < D.additions.Count)
                {
                    sum += D.additions[index] * 9;
                }
            }

            int final_index = jingjie - 1;
            // 计算当前境界的增幅时，再次检查索引
            if (final_index >= 0 && final_index < D.additions.Count)
            {
                sum += ceng * D.additions[final_index];
            }

            return sum;
        }
    }
}