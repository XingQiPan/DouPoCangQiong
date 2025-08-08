using ModTemplate.Code;
using ModTemplate.Code.Trait;
using NeoModLoader.api;

namespace CHANGEME
{
    public class Main : BasicMod<Main>
    {
        protected override void OnModLoad()
        {
            Config.isEditor = true;
            Stats.init();
            LevelTrait.init();
            TraitGroup.init();
            Patches.Init();

        }
    }
}