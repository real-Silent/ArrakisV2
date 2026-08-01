using HarmonyLib;
using UnityEngine;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(VRRig), nameof(VRRig.GrabbedByPlayer))]
    public class GardGrabPatch
    {
        public static bool enabled;
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand) =>
            !enabled;
    }

    [HarmonyPatch(typeof(VRRig), nameof(VRRig.DroppedByPlayer))]
    public class DropPatch
    {
        public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity) =>
            !GardGrabPatch.enabled;
    }

    [HarmonyPatch(typeof(GuardianRPCs), nameof(GuardianRPCs.GuardianLaunchPlayer))]
    public class LaunchPatch
    {
        public static bool Prefix(Vector3 velocity) =>
            !GardGrabPatch.enabled;
    }
}