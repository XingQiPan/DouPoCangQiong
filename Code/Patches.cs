using HarmonyLib;
using DdouPoCangPong.Code;
using DdouPoCangPong.Code.Tool;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DdouPoCangPong.Code.GongFa;
using static DdouPoCangPong.Code.GongFa.GongFa;
using DdouPoCangPong.Code.Skills;

namespace DdouPoCangPong;

public class Patches
{
    private static bool _initialized = false;
    private static bool window_creature_info_initialized;
    internal static bool window_favorites_global_mode;

    [Hotfixable]
    private static void addition_stats(Actor actor_base)
    {
        var level = actor_base.a.GetCultisysLevel();

        if (level >= 0)
        {
            actor_base.stats.mergeStats(Cultisys.LevelStats[level]);
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Actor), nameof(Actor.updateStats))]
    private static IEnumerable<CodeInstruction> ActorBase_updateStats_transpiler(IEnumerable<CodeInstruction> codes)
    {
        var list = codes.ToList();

        var addition_stats_idx = list.FindIndex(x =>
            x.opcode == OpCodes.Callvirt && (x.operand as MemberInfo)?.Name == nameof(BaseStats.normalize)) - 2;
        var add_codes = new List<CodeInstruction>
        {
            new(OpCodes.Ldarg_0),
            new(OpCodes.Call, AccessTools.Method(typeof(Patches), nameof(addition_stats)))
        };
        list[addition_stats_idx].MoveLabelsTo(add_codes[0]);
        list.InsertRange(addition_stats_idx, add_codes);

        return list;
    }

    /// <summary>
    /// 每年的逻辑
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.updateAge))]
    public static void Actor_updateAge_postfix(Actor __instance)
    {
        // 确保角色是活着的并且有功法数据
        if (!__instance.isAlive() || __instance.GetGongFaDataInternal() == null)
        {
            return;
        }

        // 每10年更新一次悟性
        if (__instance.getAge() % 10 == 0)
        {
            __instance.CalculateWuxing();
        }

        var level = __instance.GetCultisysLevel();
        if (level > __instance.GetCultisysLevel()) return;
        var talent = __instance.GetTalent();
        var mod_talent = __instance.GetModTalent() + __instance.GetGongFaData().cultivation_speed_mod;
        var wu_xing = __instance.GetWuXing();
        var gongfa = __instance.GetGongFaData();

        //获取法术实例
        var skillComp = ActorSkillManager.GetComponent(__instance);

        if (mod_talent > 0)
            __instance.IncExp(50 + talent * 0.01f * 50 + mod_talent * 50);
        else
            __instance.IncExp(50 + talent * 0.01f * 50);
        while (__instance.GetExp() >= Cultisys.LevelExpRequired[level])
        {
            __instance.ResetExp(Cultisys.LevelExpRequired[level]);

            // 判断是小境界突破还是大境界突破 (level 8 -> 9级，是冲击大境界)
            bool is_major_breakthrough = (level + 1) % 9 == 0;

            float success_chance = is_major_breakthrough
                ? Cultisys.MajorRealmBreakthroughChance[level]  // 大境界用 Major 概率
                : Cultisys.MinorStarBreakthroughChance[level];  // 小境界用 Minor 概率

            // 进行随机检定
            if (UnityEngine.Random.value <= success_chance)
            {
                // 突破成功！
                __instance.LevelUp();

                int new_level = __instance.GetCultisysLevel();

                level = new_level;
                string currentName = __instance.getName();

                string cleanName = System.Text.RegularExpressions.Regex.Replace(
                    currentName,
                    @"\s*-?\s*(斗之气|斗者|斗师|大斗师|斗灵|斗王|斗皇|斗宗|斗尊|斗圣|斗帝).*?(星|巅峰)$",
                    ""
                ).Trim();

                __instance.data.setName(cleanName + "-" + LevelTool.GetFormattedLevel(level+1));

                if(!__instance.hasTrait("fire_proof") && __instance.GetCultisysLevel() > 36)
                {
                    __instance.addTrait("fire_proof");
                }
              
                //去掉不好的体质
                if (__instance.hasTrait("crippled")) __instance.removeTrait("crippled");
                if (__instance.hasTrait("fragile_health")) __instance.removeTrait("fragile_health");
                if (__instance.hasTrait("weak")) __instance.removeTrait("weak");
                if (__instance.hasTrait("fat")) __instance.removeTrait("fat");

                //增加好的特质
                if (!__instance.hasTrait("immune") && level > 9) __instance.addTrait("immune");



                if (level >= Cultisys.MaxLevel) break;
            }
            else
            {
                break;
            }
        }

        //获得法术
        if (skillComp != null && skillComp.known_spells.Count <= 0)
        {
            skillComp.LearnSpell("minor_heal");
        }



        // 每年更新一次功法推演
        __instance.UpdateGongFaDeduction();
        __instance.AddGongFaExp((int)(wu_xing * 1.6 + level * 1.4 + Randy.randomInt(0, 50)));
    }

    [Hotfixable]
    [HarmonyPrefix, HarmonyPatch(typeof(UnitWindow), nameof(UnitWindow.OnEnable))]
    private static void OnEnable_prefix(UnitWindow __instance)
    {
        if (!(__instance.actor?.isAlive() ?? false)) return;

        Text info_text = null;
        if (!_initialized)
        {
            _initialized = true;
            var obj = new GameObject("TempInfo", typeof(Text), typeof(ContentSizeFitter));
            obj.transform.SetParent(__instance.transform.Find("Background"));
            obj.transform.localPosition = new(250, 0);
            obj.transform.localScale = Vector3.one;
            obj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            info_text = obj.GetComponent<Text>();
            info_text.font = LocalizedTextManager.current_font;
            info_text.resizeTextForBestFit = true;
            info_text.resizeTextMinSize = 1;
            info_text.resizeTextMaxSize = 8;
        }
        else
        {
            info_text = __instance.transform.Find("Background/TempInfo").GetComponent<Text>();
        }

        var sb = new StringBuilder();
        sb.AppendLine($"功法:{__instance.actor.GetGongFaData().name} {__instance.actor.GetGongFaData().rank} {__instance.actor.GetGongFaData().grade}品");
        sb.AppendLine($"功法层数：{__instance.actor.GetGongFaData().ceng} / {__instance.actor.GetGongFaData().maxceng}");
        sb.AppendLine($"功法进度：{__instance.actor.GetGongFaData().exp}/{__instance.actor.GetGongFaData().max_exp}");
        sb.AppendLine($"推演进度：{__instance.actor.GetGongFaData().deduction_progress * 100:F1}% / {__instance.actor.GetGongFaData().max_deduction * 100:F1}%");
        sb.AppendLine($"修炼速度加成：{__instance.actor.GetGongFaData().cultivation_speed_mod * 100:F1}%");
        sb.AppendLine($"{Main.asset_id_prefix}.skill".Localize());
        foreach (var item in ActorSkillManager.GetComponent(__instance.actor).known_spells)
        {
            sb.AppendLine($"{Main.asset_id_prefix}.{item.id}".Localize());
        }
        info_text.text = sb.ToString();
    }


    [HarmonyPostfix]
    [HarmonyPatch(typeof(UnitWindow), nameof(UnitWindow.OnEnable))]
    private static void WindowCreatureInfo_OnEnable_postfix(UnitWindow __instance)
    {
        if (__instance.actor == null || !__instance.actor.isAlive()) return;
        if (!window_creature_info_initialized)
        {
            __instance.StartCoroutine(new WaitUntil(() =>
            {
                if (UnitWindowHelper.Initialize(__instance))
                {
                    window_creature_info_initialized = true;
                }

                return window_creature_info_initialized;
            }));
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="pDamage"></param>
    /// <param name="pAttacker"></param>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.getHit))]
    private static void Actor_getHit_postfix(Actor __instance, ref float pDamage, BaseSimObject pAttacker = null)
    {
        if (pAttacker == null || !(pAttacker is Actor attackerActor1) || attackerActor1 == __instance)
        {
            return;
        }

        if (!__instance.hasHealth())
        {
            if (pAttacker is Actor attackerActor)
            {
                if (attackerActor == __instance)
                {
                    return;
                }
                int experienceToAward = __instance.GetCultisysLevel();

                if (experienceToAward <= 0)
                {
                    experienceToAward = 1;
                }
                attackerActor.IncExp(experienceToAward);
            }
        }

        #region 法术部分
        var knownSpells = __instance.GetKnownSpells();

        if (knownSpells.Count > 0)
        {
            var availableSpells = new List<(SkillAsset spell, Actor target)>();
            var skillComp = ActorSkillManager.GetComponent(__instance);

            foreach (var spell in knownSpells)
            {
                // 检查技能自带的使用条件
                if (spell.condition != null && spell.condition(__instance, out Actor potentialTarget))
                {
                    // 再检查蓝量和冷却
                    if (skillComp.CanCastSpell(spell, potentialTarget))
                    {
                        availableSpells.Add((spell, potentialTarget));
                    }
                }
            }

            // 如果有可用的技能, 随机选择一个并施放
            if (availableSpells.Count > 0)
            {
                // 从可用技能中随机选择一个
                var (spellToCast, target) = availableSpells[Randy.randomInt(0, availableSpells.Count)];

                // 使用工具类来尝试施法，代码更简洁、意图更明确
                __instance.TryCastSpell(spellToCast.id, target);
            }
        }
        #endregion

        int defenderLevel = __instance.GetCultisysLevel();
        Actor attack = (Actor)pAttacker;
        int attackerLevel = attack.GetCultisysLevel();

        if (defenderLevel > 90 && attackerLevel <= 90)
        {
            pDamage *= 0.1f;
            pDamage = Mathf.Max(1f, pDamage);
        }
    }

    /// <summary>
    /// 防击退
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.addForce), new Type[] { typeof(float), typeof(float), typeof(float), typeof(bool) })]
    public static bool addForce_Prefix(Actor __instance)
    {
        if (__instance.GetCultisysLevel() > 64)
        {
            return false; // 拦截原始函数
        }
        return true; // 放行原始函数
    }

    /// <summary>
    /// 防眩晕
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Actor))]
    [HarmonyPatch(typeof(Actor), nameof(Actor.makeStunned), new Type[] { typeof(float) })]
    public static bool makeStunned_Prefix(Actor __instance)
    {
        if (__instance.GetCultisysLevel() > 81)
        {
            return false; // 拦截原始函数
        }
        return true; // 放行原始函数
    }

    /// <summary>
    /// 在创建新生物时触发，使其自动继承或创建功法.生成法术权限。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.newCreature))]
    public static void Actor_newCreature_Postfix(Actor __instance)
    {
        __instance.GetGongFaData();
        ActorSkillManager.AddComponent(__instance);
    }

    [HarmonyPatch(typeof(MapBox), nameof(MapBox.Update))]
    [HarmonyPostfix]
    public static void World_update_Postfix()
    {
        ActorSkillManager.Update(Time.deltaTime);
    }
}