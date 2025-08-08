using HarmonyLib;
using NeoModLoader.api.attributes;

namespace CHANGEME;

public class Patches
{
    public static void Init()
    {
        Harmony.CreateAndPatchAll(typeof(Patches));
    }

    [HarmonyPatch(typeof(Actor), "updateAge")]
    public static void UpdateAge()
    {

    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Actor), nameof(Actor.getHit))]
    private static void Actor_getHit_postfix(Actor __instance, BaseSimObject pAttacker = null)
    {
        
    }

}