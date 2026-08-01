using GorillaTagScripts;
using HarmonyLib;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(BuilderAttachGridPlane), nameof(BuilderAttachGridPlane.IsConnected))]
    public class OverlapPatch
    {
        public static bool enabled;
        public static void Postfix(BuilderAttachGridPlane __instance, ref bool __result, SnapBounds bounds)
        {
            if (enabled)
                __result = false;
        }
    }
}