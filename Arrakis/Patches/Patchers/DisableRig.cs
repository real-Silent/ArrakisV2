using HarmonyLib;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable")]
    internal class DisableRig
    {
        public static bool Prefix(VRRig __instance)
        {
            return !(__instance == VRRig.LocalRig);
        }
    }

    [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
    public static class DisableRigBypass
    {
        public static bool Prefix(VRRigJobManager __instance, VRRig rig)
        {
            return !(__instance == VRRig.LocalRig);
        }
    }

    [HarmonyPatch(typeof(VRRig), "PostTick")]
    public class RigPatch3
    {
        public static bool Prefix(VRRig __instance) =>
            !__instance.isLocal || __instance.enabled;
    }
}