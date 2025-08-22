using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DdouPoCangPong.Code.GongFa.GongFa;

namespace DdouPoCangPong.Code.GongFa
{
    internal static class GongFaExtendTools
    {
        private static readonly ConditionalWeakTable<Actor, GongFaData> _gongFaData = new ConditionalWeakTable<Actor, GongFaData>();

        /// <summary>
        /// 获得功法
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static GongFaData GetGongFaData(this Actor actor)
        {
            return _gongFaData.GetValue(actor, key => new GongFaData(GetRandomBookName()));
        }

        /// <summary>
        /// 为角色的功法增加经验值，并自动处理层数升级。
        /// </summary>
        /// <param name="actor">目标角色</param>
        /// <param name="expToAdd">要增加的经验值</param>
        /// <returns>如果层数发生了提升，返回true；否则返回false。</returns>
        public static bool AddGongFaExp(this Actor actor, int expToAdd)
        {
            GongFaData gongFa = actor.GetGongFaData();
            bool leveledUp = false;

            // 假设最高10层，如果已满级，则不再增加经验
            if (gongFa.ceng >= gongFa.maxceng)
            {
                return false;
            }

            gongFa.exp += expToAdd;

            // 使用while循环处理一次性获得大量经验升多级的情况
            while (gongFa.exp >= gongFa.max_exp && gongFa.ceng < gongFa.maxceng)
            {
                leveledUp = true;

                // 1. 扣除升级所需的经验
                gongFa.exp -= gongFa.max_exp;

                // 2. 提升层数
                gongFa.ceng++;

                gongFa.max_exp = (int)(gongFa.max_exp * 1.05f);
                //    - 提升功法带来的天赋加成
                gongFa.mod_talent += 0.01f + Randy.randomFloat(0,0.1f);

            }

            return leveledUp;
        }

        /// <summary>
        /// 提升功法品级（推演）
        /// 只有在功法层数达到10层圆满后才能进行。
        /// </summary>
        /// <param name="actor">目标角色</param>
        /// <param name="deductionAmount">本次增加的推演值</param>
        /// <returns>如果品级发生了提升，返回true；否则返回false。</returns>
        public static bool DeduceGongFaRank(this Actor actor, float deductionAmount)
        {
            GongFaData gongFa = actor.GetGongFaData();

            // 前置条件检查：必须是10层圆满
            if (gongFa.ceng < gongFa.maxceng)
            {
                return false;
            }

            // 如果已是天阶一品，无法再提升
            if (gongFa.rank == GongFaRank.天阶 && gongFa.grade == 1)
            {
                return false;
            }

            gongFa.deduction += deductionAmount;

            if (gongFa.deduction >= gongFa.max_deduction)
            {
                // 1. 扣除推演值
                gongFa.deduction -= gongFa.max_deduction;

                // 2. 提升品级逻辑
                gongFa.grade--;
                gongFa.maxceng++;

                // 如果品级从1降到0，意味着需要提升阶级
                if (gongFa.grade < 1)
                {
                    // 如果不是最高阶（天阶），则提升阶级
                    if (gongFa.rank < GongFaRank.天阶)
                    {
                        gongFa.rank++;
                        gongFa.grade = 9;
                    }
                    else
                    {
                        gongFa.grade = 1;
                    }
                }

                gongFa.max_deduction += 1;

                return true;
            }

            return false;
        }

        public static string GetRandomBookName()
        {
            return "基础功法";
        }
    }
}
