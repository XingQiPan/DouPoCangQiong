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
        var mod_talent = __instance.GetModTalent();
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

                // 为了显示正确的突破后等级，应在LevelUp()之后再获取一次等级
                int new_level = __instance.GetCultisysLevel();

                if (new_level > 59)
                {
                    WorldTip.instance.show($"{__instance.name}突破至{LevelTool.GetFormattedLevel(new_level + 1)}!", false, "top", 1);
                }

                level = new_level;
                if (level >= Cultisys.MaxLevel) break;
            }
            else
            {
                // 突破失败！
                // 显示突破失败的提示 (使用当前等级)
                if (level > 39) // 您可以调整这个等级限制
                {
                    WorldTip.instance.show($"{__instance.name}冲击{LevelTool.GetFormattedLevel(level + 1)}失败, 瓶颈仍需打磨。", false, "top", 1);
                }
                // 失败后必须跳出while循环，避免在经验溢出时连续无限次尝试突破
                break;
            }
        }
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
        
    }

    

}