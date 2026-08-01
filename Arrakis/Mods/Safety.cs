using System;
using System.Reflection;
using Arrakis.Classes;
using Arrakis.Menu;
using Arrakis.Notifications;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using static Arrakis.Menu.Main;

namespace Arrakis.Mods
{
    public class Safety
    {
        public static void AntiReportDisconnect()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
                {
                    if (line.linePlayer == NetworkSystem.Instance.LocalPlayer)
                    {
                        Transform reportButton = line.reportButton.transform;
                        foreach (VRRig rig in VRRigCache.ActiveRigs)
                        {
                            if (rig != null && rig != VRRig.LocalRig)
                            {
                                float disR = Vector3.Distance(reportButton.position, rig.rightHandTransform.position);
                                float disL = Vector3.Distance(reportButton.position, rig.leftHandTransform.position);
                                if (disR < 0.55f || disL < 0.55f)
                                {
                                    NotificationManager.SendNotification($"<color=yellow>[ANTIREPORT]</color> {rig.Creator.NickName} Attempted to report you.");
                                    NetworkSystem.Instance.ReturnToSinglePlayer();
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Panic()
        {
            foreach (ButtonInfo[] b in Buttons.buttons)
            {
                foreach (ButtonInfo v in b)
                {
                    if (v.enabled)
                        Toggle(v.buttonText);
                }
            }
        }

        public static void NoFingerMovement()
        {
            ControllerInputPoller.instance.rightControllerPrimaryButton = false;
            ControllerInputPoller.instance.rightControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.rightControllerSecondaryButton = false;
            ControllerInputPoller.instance.rightControllerSecondaryButtonTouch = false;
            ControllerInputPoller.instance.rightGrab = false;
            ControllerInputPoller.instance.rightControllerGripFloat = 0f;
            ControllerInputPoller.instance.rightControllerTriggerButton = false;
            ControllerInputPoller.instance.rightControllerIndexFloat = 0f;

            ControllerInputPoller.instance.leftControllerPrimaryButton = false;
            ControllerInputPoller.instance.leftControllerPrimaryButtonTouch = false;
            ControllerInputPoller.instance.leftControllerSecondaryButton = false;
            ControllerInputPoller.instance.leftControllerSecondaryButtonTouch = false;
            ControllerInputPoller.instance.leftGrab = false;
            ControllerInputPoller.instance.leftControllerGripFloat = 0f;
            ControllerInputPoller.instance.leftControllerTriggerButton = false;
            ControllerInputPoller.instance.leftControllerIndexFloat = 0f;
        }

        public static void RPCProc()
        {
            MonkeAgent.instance.rpcErrorMax = int.MaxValue;
            MonkeAgent.instance.rpcCallLimit = int.MaxValue;
            MonkeAgent.instance.logErrorMax = int.MaxValue;
            MonkeAgent.instance.userRPCCalls.Clear();
            PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
            PhotonNetwork.QuickResends = int.MaxValue;
            PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);
            PhotonNetwork.OpCleanRpcBuffer(GorillaTagger.Instance.myVRRig.GetView);
            PhotonNetwork.RemoveBufferedRPCs(GorillaTagger.Instance.myVRRig.ViewID, null, null);
            PhotonNetwork.RemoveRPCsInGroup(int.MaxValue);
            PhotonNetwork.SendAllOutgoingCommands();
            MonkeAgent.instance.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer);
        }
        public static void BypassVCBan()
        {
            GorillaTagger.moderationMutedTime = -1f;
            GorillaTelemetry.PostNotificationEvent("Unmute");
            GorillaTagger.Instance.myRecorder.TransmitEnabled = true;
            if (KIDManager.Instance != null)
            {
                GameObject.Destroy(KIDManager.Instance);
            }
        }
        private static float lastRpcClear = 0f;
        private static float rpcClearInterval = 5f;

        public static void AntiCrash() // nerd shit no clue if it works but blehhh :33 -sleepy
        {
            try
            {
                if (Time.time > lastRpcClear + rpcClearInterval)
                {
                    lastRpcClear = Time.time;
                    if (PhotonNetwork.NetworkingClient != null)
                    {
                        var peer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
                        var outgoingQueueField = peer.GetType().GetField("outgoingStreamQueue", BindingFlags.Instance | BindingFlags.NonPublic);
                        var queue = outgoingQueueField?.GetValue(peer) as System.Collections.IList;
                        if (queue != null && queue.Count > 1000)
                        {
                            queue.Clear();
                            CustomConsole.Log("Cleared outgoing RPC queue to prevent crash", CustomConsole.LogType.Info);
                        }
                    }
                }
                if (GorillaTagger.Instance == null || GorillaTagger.Instance.myVRRig == null)
                    return;
            }
            catch { }
        }
        public static bool spopofing; // best name ever please dont change it -sleepy
        public static void SpoofPlatform(bool enabled)
        {
            spopofing = enabled;
            try
            {
                VRRig.LocalRig.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, 0, enabled ? 1 : 0, enabled ? 0 : 1);
            }
            catch { }
        }
        public static void SpoofSupportPage() =>
            GorillaComputer.instance.screenText.Set(GorillaComputer.instance.screenText.stringBuilder.ToString().Replace("STEAM", "QUEST").Replace(GorillaComputer.instance.buildDate,$"{GorillaComputer.instance.buildDate}\nBUILD CODE 4893\nMANAGED ACCOUNT: NO"));

        private static float AntiMemoryLeakDelay;
        public static void AntiMemoryLeak()
        {
            if (Time.time > AntiMemoryLeakDelay)
            {
                AntiMemoryLeakDelay = Time.time + 45f;
                GC.Collect();
            }
        }
    }
}