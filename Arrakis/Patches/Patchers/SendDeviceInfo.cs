using HarmonyLib;
using PlayFab.Internal;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
    public class PlayFabInfoPatch
    {
        [HarmonyPrefix]
        private static bool Prefix() =>
            false;
    }
}