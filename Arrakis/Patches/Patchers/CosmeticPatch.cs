using HarmonyLib;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.IsItemAllowed))]
    public class CosmeticPatch
    {
        public static bool enabled = false; // this is a stupid fix but it SOMEHOW fixed tpc breaking??
        public static void Postfix(VRRig __instance, ref bool __result) =>
            __result = enabled;
    }
}