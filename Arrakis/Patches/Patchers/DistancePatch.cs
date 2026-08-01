using HarmonyLib;
using UnityEngine;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.IsPositionInRange))]
    public class DistancePatch // what the fuck did i just made dude it looks so ass its 12:30 am -sleepy
    {
        public static bool enabled;

        public static void Postfix(VRRig __instance, ref bool __result, Vector3 position, float range)
        {
            NetPlayer player = g(__instance) ?? null;
            if ((enabled && __instance.isLocal) || (player != null && b(player)))
                __result = true;
        }
        public static bool b(NetPlayer Player) => Player == NetworkSystem.Instance.LocalPlayer;
        public static NetPlayer g(VRRig p) => p.Creator ?? NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(p.rigSerializer.gameObject));
    }
}