using DdouPoCangPong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code
{
    internal static class Stats
    {
        public static BaseStatAsset talent;
        public static BaseStatAsset cur_exp;
        public static BaseStatAsset max_exp;
        public static BaseStatAsset mod_talent;
        public static BaseStatAsset wu_xing;
        public static BaseStatAsset dou_qi;
        public static BaseStatAsset max_dou_qi;

        public static void Init()
        {
            talent = new BaseStatAsset();
            talent.id = "talent";
            talent.normalize = true;
            talent.normalize_min = -999999;
            talent.normalize_max = 999999;
            //Warrior.multiplier = true;
            talent.used_only_for_civs = false;
            AssetManager.base_stats_library.add(talent);

            cur_exp = new BaseStatAsset();
            cur_exp.id = "cur_exp";
            cur_exp.normalize = true;
            cur_exp.normalize_min = -999999;
            cur_exp.normalize_max = 999999;
            //Warrior.multiplier = true;
            cur_exp.used_only_for_civs = false;
            AssetManager.base_stats_library.add(cur_exp);

            max_exp = new BaseStatAsset();
            max_exp.id = "max_exp";
            max_exp.normalize = true;
            max_exp.normalize_min = -999999;
            max_exp.normalize_max = 999999;
            //Warrior.multiplier = true;
            talent.used_only_for_civs = false;
            AssetManager.base_stats_library.add(max_exp);

            mod_talent = new BaseStatAsset();
            mod_talent.id = "mod_talent";
            mod_talent.normalize = true;
            mod_talent.normalize_min = -999999;
            mod_talent.normalize_max = 999999;
            //Warrior.multiplier = true;
            mod_talent.used_only_for_civs = false;
            AssetManager.base_stats_library.add(mod_talent);

            wu_xing = new BaseStatAsset();
            wu_xing.id = "wu_xing";
            wu_xing.normalize = true;
            wu_xing.normalize_min = -999999;
            wu_xing.normalize_max = 999999;
            //Warrior.multiplier = true;
            wu_xing.used_only_for_civs = false;
            AssetManager.base_stats_library.add(wu_xing);

            dou_qi = new BaseStatAsset();
            dou_qi.id = "dou_qi";
            dou_qi.normalize = true;
            dou_qi.normalize_min = -999999;
            dou_qi.normalize_max = 999999;
            //Warrior.multiplier = true;
            dou_qi.used_only_for_civs = false;
            AssetManager.base_stats_library.add(dou_qi);

            max_dou_qi = new BaseStatAsset();
            max_dou_qi.id = "max_dou_qi";
            max_dou_qi.normalize = true;
            max_dou_qi.normalize_min = -999999;
            max_dou_qi.normalize_max = 999999;
            //Warrior.multiplier = true;
            max_dou_qi.used_only_for_civs = false;
            AssetManager.base_stats_library.add(max_dou_qi);
        }
    }
}
