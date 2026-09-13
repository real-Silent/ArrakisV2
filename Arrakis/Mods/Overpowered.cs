/*
 * Arrakis | Mods/Overpowered.cs
 *
 * Copyright (C) 2026 Arrakis
 * https://github.com/real-Silent/ArrakisV2
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arrakis.Classes;
using Arrakis.Extensions;
using Arrakis.Menu;
using Arrakis.Notifications;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using static Arrakis.Classes.RigManager;
using static Arrakis.Menu.Main;
using static BodyDockPositions;
using static TransferrableObject;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace Arrakis.Mods
{
    public class Overpowered
    {
        public static void GuardianFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    VRRig rig = Ray.collider.GetComponentInParent<VRRig>();
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        GorillaGuardianManager guard = (GorillaGuardianManager)GorillaGameManager.instance;
                        if (guard.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GetNetworkViewFromVRRig(GetVRRigFromPlayer(rig.Creator)).SendRPC("GrabbedByPlayer", rig.Creator, true, false, false);
                            GetNetworkViewFromVRRig(GetVRRigFromPlayer(rig.Creator)).SendRPC("DroppedByPlayer", rig.Creator, new Vector3(0f, 25f, 0f));
                        }
                        else
                        {
                            NotificationManager.SendNotification("<color=grey>[<color=yellow>ARRAKIS</color><color=grey>]</color> You are not guardian this mod wont work.");
                        }
                    }
                }
            }
        }

        public static void GuardianWallGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (gunLocked && lockTarget != null)
                {
                    GorillaGuardianManager guard = (GorillaGuardianManager)GorillaGameManager.instance;
                    if (guard.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                    {
                        GetNetworkViewFromVRRig(lockTarget).SendRPC("GrabbedByPlayer", lockTarget.Creator, true, false, false);
                        GetNetworkViewFromVRRig(lockTarget).SendRPC("DroppedByPlayer", lockTarget.Creator, Vector3.right * 50f - (Vector3.down * 2f) + (lockTarget.transform.position * 2f / 21f) - 2f * Vector3.forward); // shitty numbers was testing random shit and it worked so -nova
                    }
                    else
                    {
                        NotificationManager.SendNotification("<color=grey>[<color=yellow>ARRAKIS</color><color=grey>]</color> You are not guardian this mod wont work.");
                    }
                }

                if (GetGunInput(true))
                {
                    VRRig rig = Ray.collider.GetComponentInParent<VRRig>();
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        lockTarget = rig;
                        gunLocked = true;
                    }
                }
                else
                {
                    lockTarget = null;
                    gunLocked = false;
                }
            }
        }


        private static float delaytimething = 0f;
        public static void GuardianWallAll()
        {
            GorillaGuardianManager guard = (GorillaGuardianManager)GorillaGameManager.instance;
            if (guard.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        if (Time.time > delaytimething)
                        {
                            GetNetworkViewFromVRRig(rig).SendRPC("GrabbedByPlayer", rig.Creator, true, false, false);
                            GetNetworkViewFromVRRig(rig).SendRPC("DroppedByPlayer", rig.Creator, Vector3.right * 50f - (Vector3.down * 2f) + (rig.transform.position * 2f / 21f) - 2f * Vector3.forward); // shitty numbers was testing random shit and it worked so -nova
                            delaytimething = Time.time + 0.6f;
                        }
                        Safety.RPCProc();
                    }
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=grey>[<color=yellow>ARRAKIS</color><color=grey>]</color> You are not guardian this mod wont work.");
            }
        }


        public static void GuardianFlingAll()
        {
            GorillaGuardianManager guard = (GorillaGuardianManager)GorillaGameManager.instance;
            if (guard.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        GetNetworkViewFromVRRig(rig).SendRPC("GrabbedByPlayer", rig.Creator, true, false, false);
                        GetNetworkViewFromVRRig(rig).SendRPC("DroppedByPlayer", rig.Creator, new Vector3(0f, 25f, 0f));
                    }
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=grey>[<color=yellow>ARRAKIS</color><color=grey>]</color> You are not guardian this mod wont work.");
            }
        }

        public static void GuardianBringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    VRRig rig = Ray.collider.GetComponentInParent<VRRig>();
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        GorillaGuardianManager guard = (GorillaGuardianManager)GorillaGameManager.instance;
                        if (guard.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
                        {
                            GetNetworkViewFromVRRig(GetVRRigFromPlayer(rig.Creator)).SendRPC("GrabbedByPlayer", rig.Creator, true, false, false);
                            GetNetworkViewFromVRRig(GetVRRigFromPlayer(rig.Creator)).SendRPC("DroppedByPlayer", rig.Creator, (GorillaTagger.Instance.bodyCollider.transform.position - lockTarget.transform.position).normalized * 50f);
                        }
                        else
                        {
                            NotificationManager.SendNotification("<color=grey>[<color=yellow>ARRAKIS</color><color=grey>]</color> You are not guardian this mod wont work.");
                        }
                    }
                }
            }
        }

        public static void GuardianBringAll()
        {
            GorillaGuardianManager guard = (GorillaGuardianManager)GorillaGameManager.instance;
            if (guard.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        GetNetworkViewFromVRRig(rig).SendRPC("GrabbedByPlayer", rig.Creator, true, false, false);
                        GetNetworkViewFromVRRig(rig).SendRPC("DroppedByPlayer", rig.Creator, (GorillaTagger.Instance.bodyCollider.transform.position - lockTarget.transform.position).normalized * 50f);
                    }
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=grey>[<color=yellow>ARRAKIS</color><color=grey>]</color> You are not guardian this mod wont work.");
            }
        }

        public static void GuardianProtector()
        {
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig != null && rig != GorillaTagger.Instance.offlineVRRig)
                {
                    foreach (TappableGuardianIdol idol in GetAllTappables())
                    {
                        if (idol.manager != null && !idol.isChangingPositions)
                        {
                            GorillaGuardianZoneManager zoneManager = idol.zoneManager;
                            if (zoneManager.IsZoneValid() && idol.manager != null && zoneManager.CurrentGuardian != null && 
                                zoneManager.CurrentGuardian == NetworkSystem.Instance.LocalPlayer)
                            {
                                float DR = Vector3.Distance(idol.transform.position, rig.rightHandTransform.position);
                                float DL = Vector3.Distance(idol.transform.position, rig.leftHandTransform.position);
                                if (DR < 2.55f || DL < 2.55f)
                                {
                                    GetNetworkViewFromVRRig(rig).SendRPC("GrabbedByPlayer", rig.Creator, true, false, false);
                                    GetNetworkViewFromVRRig(rig).SendRPC("DroppedByPlayer", rig.Creator, new Vector3(500f, 500f, 500f));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void AlwaysGuardian()
        {
            foreach (TappableGuardianIdol idol in GetAllTappables())
            {
                if (idol.manager != null && !idol.isChangingPositions)
                {
                    GorillaGuardianZoneManager zoneManager = idol.zoneManager;
                    if (zoneManager.IsZoneValid() && idol.manager != null && zoneManager.CurrentGuardian != null &&
                        zoneManager.CurrentGuardian != NetworkSystem.Instance.LocalPlayer)
                    {
                        VRRig.LocalRig.enabled = false;
                        VRRig.LocalRig.transform.position = idol.transform.position;
                        idol.OnTap(10f);
                    }
                    else
                    {
                        VRRig.LocalRig.enabled = true;
                    }
                }
            }
        }
        public static float LagDelay;
        public static void LagAll()
        {
            if (PhotonNetwork.InRoom && Time.time > LagDelay)
            {
                LagDelay = Time.time + 3f;
                for (int i = 0; i < 425; i++)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(186, new object[] {float.NaN}, new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.Others
                    }, SendOptions.SendUnreliable);
                }
            }
        }
        public static void SetRoomStatus(bool status)
        {
            var roomProperties = new Hashtable();
            roomProperties[GamePropertyKey.IsOpen] = status;
            roomProperties[GamePropertyKey.IsVisible] = status;
            roomProperties[GamePropertyKey.MaxPlayers] = status ? 0 : 10;
            var parameters = new Dictionary<byte, object>();
            parameters.Add(OperationCode.GetProperties, roomProperties);
            parameters.Add(OperationCode.AuthenticateOnce, null);
            var peer = PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer;
            peer.SendOperation(OperationCode.SetProperties, parameters, SendOptions.SendReliable);
            GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
        }
        public static float RopeDelay;
        public static void RopeFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    GorillaRopeSwing gunTarget = GunData.Ray.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (gunTarget && Time.time > RopeDelay)
                    {
                        RopeDelay = Time.time + 0.25f;
                        RopeThing(gunTarget, RandomVector3(360));
                    }
                }
            }
        }
        public static void FreezeRopeGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    GorillaRopeSwing gunTarget = GunData.Ray.collider.GetComponentInParent<GorillaRopeSwing>();
                    if (gunTarget && Time.time > RopeDelay)
                    {
                        RopeDelay = Time.time + 0.25f;
                        RopeThing(gunTarget, Vector3.zero);
                    }
                }
            }
        }

        public static Coroutine RopeCoroutine;
        public static IEnumerator RopeEnableRig()
        {
            yield return new WaitForSeconds(0.4f);
            VRRig.LocalRig.enabled = true;
        }
        public static void RopeThing(GorillaRopeSwing Rope, Vector3 Velocity) =>
            RopeThing(RopeSwingManager.instance.ropes.FirstOrDefault(x => x.Value == Rope).Key, Velocity);
        public static void RopeThing(int RopeId, Vector3 Velocity)
        {
            Velocity = Velocity.ClampMagnitudeSafe(15f);
            if (RopeSwingManager.instance.ropes.TryGetValue(RopeId, out GorillaRopeSwing Rope))
            {
                var rope = Rope.nodes.Skip(1).Select((v, i) => new { index = i, transform = v, 
                    distance = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, v.transform.position) }).OrderBy(x => x.distance).First();
                if (rope.distance > 5f)
                {
                    if (RopeCoroutine != null)
                        CRunner.instance.StopCoroutine(RopeCoroutine);
                    RopeCoroutine = CRunner.instance.StartCoroutine(RopeEnableRig());
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = rope.transform.position;
                }
                if (Vector3.Distance(VRRig.LocalRig.transform.position, rope.transform.position) < 5f)
                    RopeSwingManager.instance.SendSetVelocity_RPC(RopeId, rope.index, Velocity, true);
                else
                    RopeDelay = 0f;
                Safety.RPCProc();
            }
        }
        public static void StumpKickAll()
        {
            if (!NetworkSystem.Instance.SessionIsPrivate)
            {
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be in a private room.");
                return;
            }
            GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Remove(PhotonNetwork.LocalPlayer.UserId);
            GorillaComputer.instance.OnGroupJoinButtonPress(0, GorillaComputer.instance.friendJoinCollider);
        }
        public static void DestroyCacheAll()
        {
            foreach (Player p in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(p.ActorNumber);
            }
        }
    }
}