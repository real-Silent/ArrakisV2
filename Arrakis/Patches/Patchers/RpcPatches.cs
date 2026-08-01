using HarmonyLib;
using Photon.Pun;

namespace Arrakis.Patches
{
    public class RpcPatches
    {
        [HarmonyPatch(typeof(VRRig), nameof(VRRig.IncrementRPC), typeof(PhotonMessageInfoWrapped), typeof(string))]
        public class NoIncrementRPC
        {
            private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall) =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.IncrementRPCCall), typeof(PhotonMessageInfo), typeof(string))]
        public class NoIncrementRPCCall
        {
            private static bool Prefix(PhotonMessageInfo info, string callingMethod = "") =>
                false;
        }

        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.IncrementRPCCallLocal))]
        public class NoIncrementRPCCallLocal
        {
            private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction) =>
                false;
        }
        [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.ShouldDisconnectFromRoom))]
        public class NoShouldDisconnectFromRoom
        {
            private static bool Prefix() =>
                false;
        }
    }
}