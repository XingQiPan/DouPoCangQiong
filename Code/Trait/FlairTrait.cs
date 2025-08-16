using ModTemplate.Code.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModTemplate.Code.Trait
{
    internal class FlairTrait
    {
        public static void Init()
        {
            ActorTrait Flair0 = BaseTraitTool.CreateTrait("Flair_0", "././icon.png", "interesting2");
            SafeSetStat(Flair0.base_stats, Stats.mod_talent.id, 0.1f);
            AssetManager.traits.add(Flair0);

            ActorTrait Flair1 = BaseTraitTool.CreateTrait("Flair_1", "././icon.png", "interesting2");
            SafeSetStat(Flair0.base_stats, Stats.mod_talent.id, 0.2f);
            AssetManager.traits.add(Flair1);

            ActorTrait Flair2 = BaseTraitTool.CreateTrait("Flair_2", "././icon.png", "interesting2");
            SafeSetStat(Flair0.base_stats, Stats.mod_talent.id, 0.3f);
            AssetManager.traits.add(Flair2);

            ActorTrait Flair3 = BaseTraitTool.CreateTrait("Flair_3", "././icon.png", "interesting2");
            SafeSetStat(Flair0.base_stats, Stats.mod_talent.id, 0.4f);
            AssetManager.traits.add(Flair3);
        }

        //设置属性
        private static void SafeSetStat(BaseStats baseStats, string statKey, float value)
        {
            baseStats[statKey] = value;
        }
    }
}
