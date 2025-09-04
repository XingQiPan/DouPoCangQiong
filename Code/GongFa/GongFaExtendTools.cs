using DdouPoCangPong.Code.Tool;
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
        /// 外部调用的主方法：获取或创建角色的功法数据。
        /// </summary>
        public static GongFaData GetGongFaData(this Actor actor)
        {
            return _gongFaData.GetValue(actor, key => CreateNewGongFaForActor(key));
        }

        /// <summary>
        /// 内部获取方法，用于避免在家族遍历时产生无限递归。
        /// </summary>
        internal static GongFaData GetGongFaDataInternal(this Actor actor)
        {
            _gongFaData.TryGetValue(actor, out var data);
            return data;
        }

        /// <summary>
        /// 为角色创建新功法的核心逻辑，包含继承机制。
        /// </summary>
        private static GongFaData CreateNewGongFaForActor(Actor actor)
        {
            // 检查角色是否有家族
            if (actor.family != null)
            {
                // 获取家族中最强的功法
                GongFaData bestFamilyGongFa = actor.family.GetBestGongFa();

                if (bestFamilyGongFa != null)
                {
                    // 继承家族最强功法（的副本）
                    return bestFamilyGongFa.Clone();
                }
                else
                {
                    // 如果家族中无人有功法，则为他创建一部新的黄阶功法
                    var newName = GongFaNameGenerator.GenerateGongFaName(GongFaRank.黄阶);
                    return new GongFaData(newName, true); // isNewFamilyGongFa = true
                }
            }

            // 如果没有家族，则给予一个默认的、非唯一的“基础功法”
            return new GongFaData(GongFaNameGenerator.GenerateGongFaName(GongFaRank.黄阶));
        }

        /// <summary>
        /// 10年结算一次：计算并更新角色的悟性
        /// </summary>
        public static void CalculateWuxing(this Actor actor)
        {
            var gongFa = actor.GetGongFaData();
            int age = actor.getAge() > 1000 ? 1000 : actor.getAge();

            // 悟性值 = 智慧值*2 + 年龄(最高1000)*15% + 初始随机0~100
            var wuxing = (int)(actor.intelligence * 2 + age * 0.15f + gongFa.GetInitialWuxingBonus(actor));
            actor.SetWuXing(wuxing);
        }

        /// <summary>
        /// 每年结算一次：处理功法推演
        /// </summary>
        public static void UpdateGongFaDeduction(this Actor actor)
        {
            if (actor.family == null) return;

            var strongestMember = actor.family.GetStrongestMember();
            // 只有家族最强者才能主持推演
            if (actor != strongestMember) return;

            var gongFa = actor.GetGongFaData();
            // 必须是10层圆满才能推演
            if (gongFa.ceng < gongFa.maxceng) return;

            // 如果已经是顶级功法，则无法再推演
            if (gongFa.rank == GongFaRank.超阶 && gongFa.grade == 1) return;

            // -- 计算本年度获得的推演值 --
            // 自身贡献：推演值 = 悟性*1.5 + 等级*1.8
            float progressThisYear = actor.GetWuXing() * 1.5f + actor.data.level * 1.8f;

            // 家族成员助力：每人每年提供 等级/2 的推演值
            foreach (var member in actor.family.units)
            {
                if (member == null || !member.isAlive() || member == actor) continue;
                progressThisYear += member.data.level / 2.0f;
            }

            gongFa.deduction_progress += progressThisYear;

            // 检查是否满足升级条件
            if (gongFa.deduction_progress >= gongFa.max_deduction)
            {
                gongFa.deduction_progress -= gongFa.max_deduction;
                UpgradeGongFa(gongFa); // 升级功法
                // 升级后，将新的功法模板同步给所有家族成员
                PropagateGongFaUpgrade(actor.family, gongFa);
            }
        }

        /// <summary>
        /// 功法升级的核心逻辑
        /// </summary>
        private static void UpgradeGongFa(GongFaData gongFa)
        {
            gongFa.grade--;

            // 阶级突破
            if (gongFa.grade < 1)
            {
                if (gongFa.rank < GongFaRank.超阶)
                {
                    gongFa.rank++;
                    gongFa.grade = 9;

                    // !! 主要修改点 !!
                    // 调用新的方法来只更新后缀，而不是完全重命名
                    gongFa.name = GongFaNameGenerator.UpgradeGongFaName(gongFa.name, gongFa.rank);

                    // 破阶提高修炼速度
                    switch (gongFa.rank)
                    {
                        case GongFaRank.玄阶: gongFa.cultivation_speed_mod += 0.10f; break;
                        case GongFaRank.地阶: gongFa.cultivation_speed_mod += 0.15f; break;
                        case GongFaRank.天阶: gongFa.cultivation_speed_mod += 0.20f; break;
                        case GongFaRank.超阶: gongFa.cultivation_speed_mod += 0.35f; break;
                    }
                }
                else
                {
                    // 已经是最高阶，品级卡在1品
                    gongFa.grade = 1;
                }
            }
            else // 品级提升
            {
                // 每升级一品，修炼速度提高
                switch (gongFa.rank)
                {
                    case GongFaRank.黄阶: gongFa.cultivation_speed_mod += Randy.randomFloat(0.01f, 0.07f); break;
                    case GongFaRank.玄阶: gongFa.cultivation_speed_mod += Randy.randomFloat(0.03f, 0.10f); break;
                    case GongFaRank.地阶: gongFa.cultivation_speed_mod += Randy.randomFloat(0.05f, 0.14f); break;
                    case GongFaRank.天阶: gongFa.cultivation_speed_mod += Randy.randomFloat(0.10f, 0.25f); break;
                    case GongFaRank.超阶: gongFa.cultivation_speed_mod += Randy.randomFloat(0.30f, 0.45f); break;
                }
            }

            // 更新推演上限
            UpdateMaxDeduction(gongFa);
        }


        /// <summary>
        /// 更新推演所需经验
        /// </summary>
        private static void UpdateMaxDeduction(GongFaData gongFa)
        {
            int gradeFromTop = 9 - gongFa.grade; // 9品是0，1品是8
            switch (gongFa.rank)
            {
                case GongFaRank.黄阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 1500 : 250 + gradeFromTop * 75;
                    break;
                case GongFaRank.玄阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 7000 : 800 + gradeFromTop * 375;
                    break;
                case GongFaRank.地阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 12000 : 2000 + gradeFromTop * 800;
                    break;
                case GongFaRank.天阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 15000 : 4000 + gradeFromTop * 1300;
                    break;
                case GongFaRank.超阶:
                    gongFa.max_deduction = 5000 + gradeFromTop * 1500;
                    break;
            }
        }

        /// <summary>
        /// 将升级后的功法同步给所有家族成员
        /// </summary>
        private static void PropagateGongFaUpgrade(Family family, GongFaData newGongFaTemplate)
        {
            if (family == null) return;
            foreach (var member in family.units)
            {
                if (member == null || !member.isAlive()) continue;
                var memberGongFa = member.GetGongFaData();

                // 只更新同源的功法
                if (memberGongFa.id == newGongFaTemplate.id)
                {
                    // 复制所有模板属性，但保留个人修炼进度
                    memberGongFa.name = newGongFaTemplate.name;
                    memberGongFa.rank = newGongFaTemplate.rank;
                    memberGongFa.grade = newGongFaTemplate.grade;
                    memberGongFa.max_deduction = newGongFaTemplate.max_deduction;
                    memberGongFa.cultivation_speed_mod = newGongFaTemplate.cultivation_speed_mod;
                }
            }
        }

        public static bool AddGongFaExp(this Actor actor, int expToAdd)
        {
            GongFaData gongFa = actor.GetGongFaData();
            bool leveledUp = false;

            if (gongFa.ceng >= gongFa.maxceng) return false;

            // 应用修炼速度加成
            gongFa.exp += (int)(expToAdd * gongFa.cultivation_speed_mod);

            while (gongFa.exp >= gongFa.max_exp && gongFa.ceng < gongFa.maxceng)
            {
                leveledUp = true;
                gongFa.exp -= gongFa.max_exp;
                gongFa.ceng++;
                // 新的最大经验值计算公式：结合悟性和当前等级，使增长更合理
                // 基础增长率1.03加上基于悟性的调整值，再加上等级带来的微小影响
                float growthFactor = 1.03f + (actor.GetCultisysLevel() * 0.0005f);
                // 确保增长率不会过低或过高
                growthFactor = Mathf.Clamp(growthFactor, 1.02f, 1.2f);

                gongFa.max_exp = (int)(gongFa.max_exp * growthFactor);
            }
            return leveledUp;
        }
    }
}