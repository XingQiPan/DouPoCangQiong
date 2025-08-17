using HarmonyLib;
using ModTemplate.Code;
using ModTemplate.Code.Tool;
using ModTemplate.Code.Trait;
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

namespace CHANGEME;

public class Patches
{
    private static bool _initialized = false;


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

                // 显示突破成功的提示 (使用新等级)
                if (new_level > 39) // 您可以调整这个等级限制
                {
                    WorldTip.instance.show($"{__instance.name}突破至{LevelTool.GetFormattedLevel(new_level)}!", false, "top", 1);
                }

                // 更新 level 变量以供下一次循环使用
                level = new_level;
                if (level >= Cultisys.MaxLevel) break; // 如果已满级，则跳出循环

                // 收藏逻辑
                //if (level > 39 && !__instance.data.favorite)
                //{
                //    __instance.data.favorite = true;
                //}
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

    [Hotfixable]
    [HarmonyPrefix, HarmonyPatch(typeof(UnitWindow), nameof(UnitWindow.OnEnable))]
    private static void OnEnable_prefix(UnitWindow __instance)
    {
        if (!(__instance.actor?.isAlive() ?? false)) return;
        SimpleButton button = UnityEngine.Object.Instantiate(SimpleButton.Prefab, __instance.transform.Find("Background"));
        button.transform.localPosition = new Vector3(-250, 0);
        button.transform.localScale = Vector3.one;
        button.Setup(null, SpriteTextureLoader.getSprite("cultiway/icons/iconCultivation"));

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
        var actor_level = __instance.actor.GetCultisysLevel();
        var actor_exp = __instance.actor.GetExp();
        var actor_max_exp = Cultisys.LevelExpRequired[actor_level];
        var actor_talent = __instance.actor.GetTalent();

        sb.AppendLine($"{LevelTool.GetFormattedLevel(actor_level)} 经验:{actor_exp}/{actor_max_exp}");
        sb.AppendLine($"天赋:{actor_talent}");

        var stats_for_level = Cultisys.LevelStats[actor_level];
        
        if (stats_for_level[S.health] != 0)
        {
            sb.AppendLine($"  生命值: +{stats_for_level[S.health]}");
        }
        if (stats_for_level[S.damage] != 0)
        {
            sb.AppendLine($"  伤害: +{stats_for_level[S.damage]}");
        }
        if (stats_for_level[S.lifespan] != 0)
        {
            // 寿命通常是直接设定而不是加成，所以不用"+"号
            sb.AppendLine($"  寿命: {stats_for_level[S.lifespan]}");
        }

        info_text.text = sb.ToString();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.getHit))]
    private static void Actor_getHit_postfix(Actor __instance, BaseSimObject pAttacker = null)
    {
        
    }

    

}