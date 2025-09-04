using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DdouPoCangPong.Code.GongFa.GongFa;

namespace DdouPoCangPong.Code.GongFa
{
    internal static class FamilyExtendTools
    {
        /// <summary>
        /// 获取家族中所有成员修炼的最高级的功法。
        /// </summary>
        public static GongFaData GetBestGongFa(this Family family)
        {
            GongFaData bestGongFa = null;

            if (family == null || family.units.Count == 0)
            {
                return null;
            }

            foreach (Actor member in family.units)
            {
                if (member == null || !member.isAlive()) continue;

                GongFaData currentGongFa = member.GetGongFaDataInternal(); // 使用一个内部方法避免无限循环
                if (currentGongFa == null) continue;

                if (bestGongFa == null)
                {
                    bestGongFa = currentGongFa;
                }
                else
                {
                    // 比较阶级，阶级高者胜
                    if (currentGongFa.rank > bestGongFa.rank)
                    {
                        bestGongFa = currentGongFa;
                    }
                    // 阶级相同，比较品级 (品级数值越小越强)
                    else if (currentGongFa.rank == bestGongFa.rank && currentGongFa.grade < bestGongFa.grade)
                    {
                        bestGongFa = currentGongFa;
                    }
                }
            }

            return bestGongFa;
        }

        /// <summary>
        /// 获取家族中的最强者 (基于等级)
        /// </summary>
        public static Actor GetStrongestMember(this Family family)
        {
            Actor strongest = null;
            if (family == null || family.units.Count == 0) return null;

            foreach (var member in family.units)
            {
                if (member == null || !member.isAlive()) continue;

                if (strongest == null || member.data.level > strongest.data.level)
                {
                    strongest = member;
                }
            }
            return strongest;
        }
    }
}
