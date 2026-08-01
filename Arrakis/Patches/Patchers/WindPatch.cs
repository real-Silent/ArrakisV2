using HarmonyLib;

namespace Arrakis.Patches
{
    public class WindPatch
    {
        public static bool enabled;

        [HarmonyPatch(typeof(ForceVolume), nameof(ForceVolume.OnTriggerEnter))]
        public class WindPatch1
        {
            public static bool Prefix() =>
                !enabled;
        }

        [HarmonyPatch(typeof(ForceVolume), nameof(ForceVolume.OnTriggerExit))]
        public class WindPatch2
        {
            public static bool Prefix() =>
                !enabled;
        }

        [HarmonyPatch(typeof(ForceVolume), nameof(ForceVolume.OnTriggerStay))]
        public class WindPatch3
        {
            public static bool Prefix() =>
                !enabled;
        }
    }
}