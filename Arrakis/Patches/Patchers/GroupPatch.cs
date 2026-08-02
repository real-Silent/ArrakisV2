using HarmonyLib;

namespace Arrakis.Patches.Patchers
{
    [HarmonyPatch(typeof(RoomSystem), nameof(RoomSystem.SearchForNearby))] // i think working? -sleepy
    public class GroupPatch
    {
        public static bool enabled;
        public static bool Prefix() =>
            !enabled;
    }
}
