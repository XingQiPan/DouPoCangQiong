using System;

namespace DdouPoCangPong.Code.Tool
{
    internal class LevelTool
    {
        // base_num 似乎没有被使用，如果不需要可以移除
        public static int base_num = 10000;

        public static string GetFormattedLevel(int actor_level)
        {
            // 境界列表
            string[] realms = { "斗之气", "斗者", "斗师", "大斗师", "斗灵", "斗王", "斗皇", "斗宗", "斗尊", "斗圣", "斗帝" };

            if (actor_level <= 0)
            {
                return "未入门"; // 或者其他您想显示的初始状态
            }

            // 计算境界索引 (level 1-9 -> index 0, 10-18 -> index 1, etc.)
            int realmIndex = (actor_level - 1) / 9;

            // 计算当前境界的层级 (level 9 -> layer 9, level 10 -> layer 1)
            int layer = (actor_level - 1) % 9 + 1;

            // 防止索引超出境界列表的范围
            if (realmIndex >= realms.Length)
            {
                // 如果等级超过了斗帝九层，可以定义一个特殊显示
                realmIndex = realms.Length - 1;
                return $"境界:{realms[realmIndex]}巅峰";
            }

            string realmName = realms[realmIndex];

            return $"境界:{realmName}{layer}星";
        }
    }
}