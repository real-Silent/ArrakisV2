using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(GTPlayer), nameof(GTPlayer.ApplyKnockback))]
    public class KnockbackPatch
    {
        public static bool enabled;
        public static bool Prefix(Vector3 direction, float speed) =>
            !enabled;
    }
}