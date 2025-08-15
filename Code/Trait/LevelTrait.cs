using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ReflectionUtility;
using ai;
using System.Numerics;
using UnityEngine;
using ModTemplate.Code.Tool;

namespace ModTemplate.Code.Trait
{
    internal class LevelTrait
    {
        public static void Init()
        {
            ActorTrait Grade0 = BaseTraitTool.CreateTrait("Grade0", "icon.png", "interesting1");
            AssetManager.traits.add(Grade0);
        }

        //设置属性
        private static void SafeSetStat(BaseStats baseStats, string statKey, float value)
        {
            baseStats[statKey] = value;
        }
    }
}
