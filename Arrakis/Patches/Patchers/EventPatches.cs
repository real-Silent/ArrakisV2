using System;
using System.Collections.Generic;
using HarmonyLib;
using Modio;
using Photon.Pun;
using Photon.Realtime;

namespace Arrakis.Patches.Patchers
{
    [HarmonyPatch(typeof(PhotonNetwork), nameof(PhotonNetwork.RunViewUpdate))]
    public class EventPatches
    {
        public static event Action OnSerialize;
        public static Func<bool> Override;

        public static bool Prefix()
        {
            if (!NetworkSystem.Instance.InRoom)
                return true;

            try
            {
                OnSerialize?.Invoke();
            }
            catch (Exception e)
            {
                CustomConsole.Log($"Error in OnSerialize: {e}", CustomConsole.LogType.Error);
            }

            if (Override == null)
                return true;

            try
            {
                return Override();
            }
            catch (Exception e)
            {
                CustomConsole.Log($"Error in OverrideSerialization: {e}", CustomConsole.LogType.Error);
                return false;
            }
        }
    }
    [HarmonyPatch(typeof(PhotonNetwork), nameof(PhotonNetwork.RPC), typeof(PhotonView), typeof(string), typeof(RpcTarget), typeof(Player), typeof(bool), typeof(object[]))]
    public class AntiEventThingIgIdfk // fuck you this is going here -sleepy
    {
        public static Dictionary<string, Func<bool>> FilteredRPCs = new Dictionary<string, Func<bool>>();

        public static bool Prefix(PhotonView view, string methodName, RpcTarget target, Player player, bool encrypt, params object[] parameters)
        {
            if (FilteredRPCs.Count <= 0)
                return true;

            try
            {
                if (FilteredRPCs.TryGetValue(methodName, out var function))
                    return function?.Invoke() ?? true;
            }
            catch (Exception e)
            {
                CustomConsole.Log($"Error in rpc filter.{methodName}: {e}", CustomConsole.LogType.Error);
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(MonkeAgent), "ShouldDisconnectFromRoom")]
    public class ShouldDisconnectFromRoom
    {
        public static bool Prefix() => false;
    }
}
