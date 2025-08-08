using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModTemplate.Code
{
    internal class LevelTool
    {
        public static int base_num = 10000;

        /// <summary>
        /// 获取当前境界血量
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ceng"></param>
        /// <returns></returns>
        public static int GetLevelHealth(int level)
        {
            // 计算当前层数（1-9）
            int ceng = level % 9 + 1;

            // 计算当前境界（0-10，对应11个境界）
            int jingjie = level < 10 ? 1 : level / 9;

            return (int)(jingjie + Math.Pow(jingjie + 1, 4) * (1 + SumAddtionRate(jingjie, ceng)));
        }

        public static int GetDamage(int health)
        {
            return (int)(health * 0.04f);
        }

        public static int GetMagic(int level)
        {
            return (int)(Math.Pow(10, level - 1));
        }

        public static int GetAge(int level)
        {
            return D.Ages[level];
        }

        /// <summary>
        /// 获取总增幅
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ceng"></param>
        /// <returns></returns>
        public static float SumAddtionRate(int level, int cen)
        {
            float sum = 0;
            for (int i = 1; i < level; i++)
            {
                sum += D.additions[i - 1] * 9;
            }
            return sum + cen * D.additions[level - 1];
        }
    }
}
