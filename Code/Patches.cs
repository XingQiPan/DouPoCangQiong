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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.updateAge))]
    public static void Actor_updateAge_postfix(Actor __instance)
    {
        var level = __instance.GetCultisysLevel();
        if (level > __instance.GetCultisysLevel()) return;
        var talent = __instance.GetTalent();
        var mod_talent = __instance.GetModTalent() + __instance.GetGongFaData().mod_talent;
        var wu_xing = __instance.GetWuXing();
        var gongfa = __instance.GetGongFaData();

        if (mod_talent > 0)
            __instance.IncExp(50 + talent * 0.01f * 50 + mod_talent * 50);
        else
            __instance.IncExp(50 + talent * 0.01f * 50);
        // 【核心修改】当经验满足时，进行突破尝试
        while (__instance.GetExp() >= Cultisys.LevelExpRequired[level])
        {
            // 消耗掉本次突破所需的经验，无论成功与否
            __instance.ResetExp(Cultisys.LevelExpRequired[level]);

            // 判断是小境界突破还是大境界突破 (level 8 -> 9级，是冲击大境界)
            bool is_major_breakthrough = (level + 1) % 9 == 0;

            // 【已修正】从 Cultisys 中获取正确的概率
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

                __instance.data.setName(cleanName + "-" + LevelTool.GetFormattedLevel(level));

                if(!__instance.hasTrait("fire_proof") && __instance.GetCultisysLevel() > 44)
                {
                    __instance.addTrait("fire_proof");
                }
              
                if (__instance.hasTrait("crippled")) __instance.removeTrait("crippled");
                if (__instance.hasTrait("fragile_health")) __instance.removeTrait("fragile_health");
                if (__instance.hasTrait("weak")) __instance.removeTrait("weak");
                if (__instance.hasTrait("fat")) __instance.removeTrait("fat");

                if (level >= Cultisys.MaxLevel) break;
            }
            else
            {
                break;
            }
        }

        //修炼功法
        if(!__instance.AddGongFaExp(wu_xing)|| gongfa.ceng >= 10)
        {
            __instance.DeduceGongFaRank(0.1f);
        }
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
        sb.AppendLine($"推演进度：{__instance.actor.GetGongFaData().deduction * 100:F1}% / {__instance.actor.GetGongFaData().max_deduction * 100:F1}%");
        sb.AppendLine($"修炼速度加成：{__instance.actor.GetGongFaData().mod_talent * 100:F1}%");

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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.getHit))]
    private static void Actor_getHit_postfix(Actor __instance, BaseSimObject pAttacker = null)
    {
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
    }

    /// <summary>
    /// 创建功法
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="pActor1"></param>
    /// <param name="pActor2"></param>
    [HarmonyPatch(typeof(Family), nameof(Family.newFamily))]
    public static void Family_book_postfix(Family __instance, Actor pActor1, Actor pActor2)
    {
        GongFaData actor1GongFa = pActor1.GetGongFaData();
    }

}