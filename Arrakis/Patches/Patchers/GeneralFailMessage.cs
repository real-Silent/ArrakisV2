using GorillaNetworking;
using HarmonyLib;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(GorillaComputer), "GeneralFailureMessage")]
    public class GeneralFailMessage
    {
        [HarmonyPrefix]
        private static bool Prefix() =>
            false;
    }
}