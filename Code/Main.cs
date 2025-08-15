using HarmonyLib;
using ModTemplate.Code;
using ModTemplate.Code.Trait;
using NCMS.Utils;
using NeoModLoader.api;
using NeoModLoader.General;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CHANGEME
{
    public class Main : BasicMod<Main>
    {

        protected override void OnModLoad()
        {
            Config.isEditor = true;
            Stats.Init();
            Debug.Log("斗气属性加载成功……");
            LevelTrait.Init();
            FlairTrait.Init();
            TraitGroup.Init();
            Debug.Log("斗气特质加载成功……");
            Harmony.CreateAndPatchAll(typeof(Patches));
            Debug.Log("斗气机制加载成功……");
        }
    }
}