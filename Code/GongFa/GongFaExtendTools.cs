using DdouPoCangPong.Code.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using static DdouPoCangPong.Code.GongFa.GongFa;
using DdouPoCangPong.Code.Skills; // 引入技能命名空间

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
                    // 新增：功法继承限制
                    // 如果家族最强功法是超阶，后代最多获得一个同源的天阶功法
                    if (bestFamilyGongFa.rank == GongFaRank.超阶)
                    {
                        var downgradedGongFa = bestFamilyGongFa.Clone();
                        downgradedGongFa.rank = GongFaRank.天阶;
                        downgradedGongFa.grade = 9; // 从天阶九品开始
                        // 重置修炼和推演进度，但保留其强大的血统（ID不变）
                        downgradedGongFa.ceng = 0;
                        downgradedGongFa.exp = 0;
                        downgradedGongFa.max_exp = 100;
                        downgradedGongFa.deduction_progress = 0;
                        // 可以给予一个比普通功法略高的初始修炼速度
                        downgradedGongFa.cultivation_speed_mod = 1.2f;
                        downgradedGongFa.granted_skill_ids.Clear();
                        UpdateMaxDeduction(downgradedGongFa); // 设置为天阶九品正确的推演值
                        return downgradedGongFa;
                    }

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
        /// 新增：检查是否满足功法推演的境界要求
        /// </summary>
        private static bool CanDeductGongFa(GongFaData gongFa, int actorLevel)
        {
            // 境界等级映射: 0-8 斗之气, 9-17 斗者, 18-26 斗师, 27-35 大斗师, 36-44 斗灵, 45-53 斗王,
            // 54-62 斗皇, 63-71 斗宗, 72-80 斗尊, 81-89 斗圣, 90-98 斗帝
            int realmLevel = actorLevel / 9;

            switch (gongFa.rank)
            {
                case GongFaRank.黄阶:
                    if (gongFa.grade >= 3) return realmLevel >= 3; // 大斗师
                    if (gongFa.grade < 3) return realmLevel >= 4;  // 斗灵
                    break;
                case GongFaRank.玄阶:
                    if (gongFa.grade >= 5) return realmLevel >= 4; // 斗灵
                    if (gongFa.grade < 5) return realmLevel >= 5;  // 斗王
                    break;
                case GongFaRank.地阶:
                    if (gongFa.grade >= 7) return realmLevel >= 6; // 斗皇
                    if (gongFa.grade == 6 || gongFa.grade == 5) return realmLevel >= 7; // 斗宗
                    if (gongFa.grade == 4 || gongFa.grade == 3) return realmLevel >= 8; // 斗尊
                    if (gongFa.grade < 3) return realmLevel >= 9;  // 斗圣
                    break;
                case GongFaRank.天阶:
                    if (gongFa.grade >= 7) return realmLevel >= 9;  // 斗圣
                    if (gongFa.grade < 7) return realmLevel >= 10; // 斗帝
                    break;
            }
            return true; // 超阶或未匹配到的情况默认允许
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

            // 检查推演者的境界是否满足要求
            if (!CanDeductGongFa(gongFa, actor.GetCultisysLevel())) return;

            // -- 计算本年度获得的推演值 --
            // 自身贡献：推演值 = 悟性*1.5 + 等级*1.8
            float progressThisYear = actor.GetWuXing() * 1.5f + actor.data.level * 1.8f;

            // 新增：超阶功法只能由自己推演，不能获得家族助力
            if (gongFa.rank != GongFaRank.超阶)
            {
                // 家族成员助力：每人每年提供 等级/2 的推演值
                foreach (var member in actor.family.units)
                {
                    if (member == null || !member.isAlive() || member == actor) continue;
                    progressThisYear += member.data.level / 2.0f;
                }
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
                    gongFa.name = GongFaNameGenerator.UpgradeGongFaName(gongFa.name, gongFa.rank);
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
                    // 新增：超阶功法达到1品后可以无限推演
                    gongFa.grade = 1; // 品级保持在1品
                    // 每次推演成功，都永久提升修炼速度
                    gongFa.cultivation_speed_mod += Randy.randomFloat(0.2f, 0.3f);
                    // 大幅增加下一次推演的难度
                    gongFa.max_deduction = (int)(gongFa.max_deduction * 1.5f);
                    return; // 直接返回，跳过后续的常规更新
                }
            }
            else // 品级提升
            {
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
                    gongFa.max_deduction = gongFa.grade == 1 ? 1500 * 2 : 250 + gradeFromTop * 75 * 3;
                    break;
                case GongFaRank.玄阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 7000 * 2 : 800 + gradeFromTop * 375 * 3;
                    break;
                case GongFaRank.地阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 12000 * 2 : 2000 + gradeFromTop * 800 * 3;
                    break;
                case GongFaRank.天阶:
                    gongFa.max_deduction = gongFa.grade == 1 ? 15000 * 2 : 4000 + gradeFromTop * 1300 * 3;
                    break;
                case GongFaRank.超阶:
                    gongFa.max_deduction = 5000 + gradeFromTop * 1500 * 3;
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
                var memberGongFa = member.GetGongFaDataInternal();
                if (memberGongFa == null) continue;

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

            gongFa.exp += (int)(expToAdd * gongFa.cultivation_speed_mod);

            while (gongFa.exp >= gongFa.max_exp && gongFa.ceng < gongFa.maxceng)
            {
                int nextLevel = gongFa.ceng + 1;
                bool isBreakthroughLevel = (nextLevel % 2 == 0);

                if (isBreakthroughLevel)
                {
                    if (Randy.randomFloat(0,1) > gongFa.GetBreakthroughChance())
                    {
                        gongFa.exp = 0;
                        break;
                    }
                }

                leveledUp = true;
                gongFa.exp -= gongFa.max_exp;
                gongFa.ceng++;

                if (isBreakthroughLevel)
                {
                    if (gongFa.granted_skill_ids.Count < gongFa.GetMaxSkillsCapacity())
                    {
                        var availableSkills = new List<string> { "minor_heal", "fire_dart" };
                        var skillToLearn = availableSkills.GetRandom();

                        if (!actor.HasSpell(skillToLearn))
                        {
                            // !! 主要修改点 !!
                            // 在学习技能时，传入当前功法的阶级作为技能的品质
                            actor.LearnSpell(skillToLearn, gongFa.rank);
                            gongFa.granted_skill_ids.Add(skillToLearn);
                        }
                    }
                }

                float growthFactor = 1.03f + (actor.GetCultisysLevel() * 0.0005f);
                growthFactor = Mathf.Clamp(growthFactor, 1.02f, 1.2f);
                gongFa.max_exp = (int)(gongFa.max_exp * growthFactor);
            }
            return leveledUp;
        }
    }
}