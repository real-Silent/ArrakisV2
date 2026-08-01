using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Arrakis.Notifications
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerLeftRoom")]
    public class LeavePatch : MonoBehaviour
    {
        private static void Prefix(Player otherPlayer)
        {
            if (otherPlayer != PhotonNetwork.LocalPlayer && otherPlayer != a)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=cyan>LEAVE</color><color=grey>]</color> <color=white>Name: " + otherPlayer.NickName + "</color>");
                a = otherPlayer;
            }
        }
        private static Player a;
    }
}