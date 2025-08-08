using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModTemplate.Code.Trait
{
    public class BaseTraitTool
    {
        public static ActorTrait CreateTrait(string id, string path_icon, string group_id)
        {
            ActorTrait trait = new ActorTrait();
            trait.id = id;
            trait.path_icon = path_icon;
            trait.needs_to_be_explored = false;
            trait.group_id = group_id;
            trait.base_stats = new BaseStats();

            return trait;
        }
        public static ClanTrait CclanTrait(string id, string path_icon, string group_id)
        {
            ClanTrait trait = new ClanTrait();
            trait.id = id;
            trait.path_icon = path_icon;
            trait.needs_to_be_explored = false;
            trait.group_id = group_id;
            trait.base_stats = new BaseStats();

            return trait;
        }


    }
}
