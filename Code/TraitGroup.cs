using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdouPoCangPong.Code
{
    internal class TraitGroup
    {
        public static void Init()
        {
            ActorTraitGroupAsset interesting1 = new ActorTraitGroupAsset();
            interesting1.id = "interesting1";
            interesting1.name = "trait_group_interesting1";
            interesting1.color = "#00FFFF";
            AssetManager.trait_groups.add(interesting1);

            ActorTraitGroupAsset interesting2 = new ActorTraitGroupAsset();
            interesting2.id = "interesting2";
            interesting2.name = "trait_group_interesting2";
            interesting2.color = "#D02090";
            AssetManager.trait_groups.add(interesting2);

            ActorTraitGroupAsset interesting3 = new ActorTraitGroupAsset();
            interesting3.id = "interesting3";
            interesting3.name = "trait_group_interesting3";
            interesting3.color = "#FFC20E";
            AssetManager.trait_groups.add(interesting3);
        }
    }
}
