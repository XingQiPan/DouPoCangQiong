using DdouPoCangPong;
using DdouPoCangPong.Code.Abstract;
using DdouPoCangPong.Code.Tool;
using NeoModLoader.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DdouPoCangPong.Code
{
    internal class Tooltips : ExtendLibrary<TooltipAsset, Tooltips>
    {
        public static TooltipAsset cultisys { get; private set; }
        public static TooltipAsset common { get; private set; }

        protected override void OnInit()
        {
            RegisterAssets(Main.asset_id_prefix);
            cultisys.callback = ShowCultisys;
            common.callback = ShowCommon;
        }

        private void ShowCommon(Tooltip tooltip, string type, TooltipData data)
        {
            var tip_name = data.tip_name;
            if (LocalizedTextManager.stringExists(tip_name)) tip_name = LM.Get(tip_name);

            tooltip.name.text = tip_name;

            var tip_description = data.tip_description;
            if (LocalizedTextManager.stringExists(tip_description)) tip_description = LM.Get(tip_description);

            tooltip.setDescription(tip_description);

            var tip_description2 = data.tip_description_2;
            if (LocalizedTextManager.stringExists(tip_description2)) tip_description2 = LM.Get(tip_description2);

            tooltip.setBottomDescription(tip_description2);
        }

        private static void ShowCultisys(Tooltip tooltip, string type, TooltipData data)
        {
            Actor actor = data.actor;

            var level = actor.GetCultisysLevel();
            tooltip.name.text = LevelTool.GetFormattedLevel(level);
            tooltip.setDescription($"{Main.asset_id_prefix}.ui.cultiprogress".Localize() + $": {actor.GetExp():N0}/{Cultisys.LevelExpRequired[level]:N0}");
            tooltip.addLineIntText($"{Main.asset_id_prefix}.ui.talent", (int)actor.GetTalent());
            tooltip.addLineIntText($"{Main.asset_id_prefix}.ui.wuxing", (int)actor.GetWuXing());
            BaseStatsHelper.showBaseStats(tooltip.stats_description, tooltip.stats_values, Cultisys.LevelStats[level]);
        }
    }
}
