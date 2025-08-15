using HarmonyLib;
using ModTemplate.Code;
using ModTemplate.Code.Tool;
using ModTemplate.Code.Trait;
using NeoModLoader.api.attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace CHANGEME;

public class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.updateAge))]
    public static void Actor_updateAge_postfix(Actor __instance)
    {
        Debug.Log("成功插入");
        var level = __instance.GetCultisysLevel();
        if (level > __instance.GetCultisysLevel()) return;
        var talent = __instance.GetTalent();
        var mod_talent = __instance.GetModTalent();
        if (mod_talent > 0)
            __instance.IncExp(50 + talent * 0.01f * 50 + mod_talent * 50);
        else
            __instance.IncExp(50 + talent * 0.01f * 50);
        if (__instance.GetExp() >= Cultisys.LevelExpRequired[level])
        {
            __instance.LevelUp();
            __instance.ResetExp();
            Debug.Log($"{__instance.name} 突破了！ 当前等级{level} 经验：{__instance.GetExp()} / {Cultisys.LevelExpRequired[level]}");
        }
        Debug.Log($"{__instance.name}当前等级{level} 经验：{__instance.GetExp()} / {Cultisys.LevelExpRequired[level]}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.getHit))]
    private static void Actor_getHit_postfix(Actor __instance, BaseSimObject pAttacker = null)
    {
        
    }

}