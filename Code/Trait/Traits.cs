using DdouPoCangPong.Code.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code.Trait
{
    internal class Traits : ExtendLibrary<ActorTrait, Traits>
    {
        public static HashSet<string> all_enabled_traits;
        public static HashSet<string> passive_traits;
        public static HashSet<string> active_traits;
        public static HashSet<string> all_traits;

        protected override void OnInit()
        {
            all_traits = assets_added.Select(x => x.id).ToHashSet();
        }
    }
}
