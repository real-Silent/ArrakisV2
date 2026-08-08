using System.Reflection;
using HarmonyLib;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(RoomControls), nameof(RoomControls.CanModerate), new System.Type[0])]
    public static class RoomControlPatch
    {
        public static bool Prefix()
        {
            return true;
        }
    }
    [HarmonyPatch(typeof(RoomSystem), nameof(RoomSystem.WasRoomSubscription), MethodType.Getter)]
    public static class WasRoomSubscriptionPatch
    {
        public static bool Prefix()
        {
            return true;
        }
    }
}