using HarmonyLib;
using ModTemplate.Code;
using ModTemplate.Code.Tool;
using ModTemplate.Code.Trait;
using NeoModLoader.api.attributes;
using NeoModLoader.General.UI.Prefabs;
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

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.updateAge))]
    public static void Actor_updateAge_postfix(Actor __instance)
    {
        //Debug.Log("成功插入");
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
            //Debug.Log($"{__instance.name} 突破了！ 当前等级{__instance.GetCultisysLevel()} 经验：{__instance.GetExp()} / {Cultisys.LevelExpRequired[level]}");
        }
        //Debug.Log($"{__instance.name}当前等级{level} 经验：{__instance.GetExp()} / {Cultisys.LevelExpRequired[level]}");
    }

    [Hotfixable]
    [HarmonyPrefix, HarmonyPatch(typeof(UnitWindow), nameof(UnitWindow.OnEnable))]
    private static void OnEnable_prefix(UnitWindow __instance)
    {
        if (!(__instance.actor?.isAlive() ?? false)) return;
        SimpleButton button = Object.Instantiate(SimpleButton.Prefab, __instance.transform.Find("Background"));
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
        //sb.AppendLine(__instance.actor.data.id.ToString());
        var actor_level = __instance.actor.GetCultisysLevel();
        var actor_exp = __instance.actor.GetExp();
        var actor_max_exp = Cultisys.LevelExpRequired[actor_level];
        var actor_talent = __instance.actor.GetTalent();
        sb.AppendLine($"等级:{actor_level} 经验:{actor_exp}/{actor_max_exp}");
        sb.AppendLine($"天赋:{actor_talent}");
       
        info_text.text = sb.ToString();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.getHit))]
    private static void Actor_getHit_postfix(Actor __instance, BaseSimObject pAttacker = null)
    {
        
    }

}