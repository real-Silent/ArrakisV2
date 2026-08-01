using HarmonyLib;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(GorillaQuitBox), nameof(GorillaQuitBox.OnBoxTriggered))]
    public class AntiQuitBox
    {
        public static bool disable;
        public static bool Prefix() =>
            !disable;
    }
}