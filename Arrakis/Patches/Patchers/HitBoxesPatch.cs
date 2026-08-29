using HarmonyLib;
using static Arrakis.Mods.Advantage;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(GorillaTagger), "get_sphereCastRadius")]
    public class HitBoxesPatch
    {
        public static bool enabled;
        public static void Postfix(GorillaTagger __instance, ref float __result)
        {
            if (enabled)
            {
                __result = hitboxScale;
            }
            else if (__result != 0.03f)
                __result = 0.03f;
        }
    }
}