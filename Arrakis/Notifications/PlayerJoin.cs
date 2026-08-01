using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Arrakis.Notifications
{
    [HarmonyPatch(typeof(MonoBehaviourPunCallbacks), "OnPlayerEnteredRoom")]
    public class JoinPatch : MonoBehaviour
    {
        private static void Prefix(Player newPlayer)
        {
            if (newPlayer != oldnewplayer)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=cyan>JOIN</color><color=grey>] </color><color=white>Name: " + newPlayer.NickName + "</color>");
                oldnewplayer = newPlayer;
            }
        }
        private static Player oldnewplayer;
    }
}