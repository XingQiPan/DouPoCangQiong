using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DdouPoCangPong.Code.GongFa.GongFa;


namespace DdouPoCangPong.Code.GongFa
{
    public static class GongFaNameGenerator
    {
        // 词库
        private static readonly List<string> attributes = new List<string> { "炎", "冰", "雷", "风", "木", "金", "土", "光", "暗", "虚" };
        private static readonly List<string> natures = new List<string> { "云", "山", "海", "日", "月", "星", "河", "林", "火", "川" };
        private static readonly List<string> emperors = new List<string> { "苍", "玄", "元", "始", "圣", "神", "魔", "龙", "凰" };
        private static readonly List<string> concepts = new List<string> { "时光", "轮回", "寂灭", "混沌", "永恒", "创世", "虚无", "阴阳", "生死" };

        // 新增：所有可能的后缀列表，用于查找和替换
        private static readonly List<string> suffixes = new List<string> { "诀", "法", "典", "经", "道" };

        /// <summary>
        /// 根据功法等阶生成一个随机名字
        /// </summary>
        public static string GenerateGongFaName(GongFaRank rank)
        {
            string suffix;
            string prefix;

            suffix = "诀";
            prefix = $"{attributes.GetRandom()}{natures.GetRandom()}";

            return prefix + suffix;
        }

        /// <summary>
        /// (新增方法) 根据新的功法等阶，更新功法名字的后缀
        /// </summary>
        /// <param name="currentName">功法当前的名字</param>
        /// <param name="newRank">功法新的等阶</param>
        /// <returns>返回带有新后缀的名字</returns>
        public static string UpgradeGongFaName(string currentName, GongFaRank newRank)
        {
            string prefix = currentName;

            // 1. 找到并移除旧的后缀，得到名字的前缀
            foreach (var suffix in suffixes)
            {
                if (currentName.EndsWith(suffix))
                {
                    // Substring(startIndex, length)
                    prefix = currentName.Substring(0, currentName.Length - suffix.Length);
                    break; // 找到后就跳出循环
                }
            }
            // 2. 根据新的Rank获取新的后缀
            string newSuffix;
            switch (newRank)
            {
                case GongFaRank.黄阶: newSuffix = "诀"; break;
                case GongFaRank.玄阶: newSuffix = "法"; break;
                case GongFaRank.地阶: newSuffix = "典"; break;
                case GongFaRank.天阶: newSuffix = emperors.GetRandom()+"经"; break;
                case GongFaRank.超阶: newSuffix = concepts.GetRandom()+"道"; break;
                default: return currentName; // 如果出现意外情况，返回原名
            }

            // 3. 组合前缀和新后缀
            return prefix + newSuffix;
        }
    }
}