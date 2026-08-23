/*
using static GorillaNetworking.GorillaComputer;
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arrakis.Classes;
using Arrakis.Extensions;
using Arrakis.Menu;
using Arrakis.Notifications;
using Arrakis.Patches;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using static Arrakis.Classes.RigManager;
using static Arrakis.Menu.Main;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
                            NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not guardian this mod wont work.");
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
                        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not guardian this mod wont work.");
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
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not guardian this mod wont work.");
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
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not guardian this mod wont work.");
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
                            NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not guardian this mod wont work.");
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
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not guardian this mod wont work.");
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
                            if (zoneManager.IsZoneValid() && idol.manager != null && zoneManager.CurrentGuardian != null && zoneManager.CurrentGuardian == NetworkSystem.Instance.LocalPlayer)
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
                    if (zoneManager.IsZoneValid() && idol.manager != null && zoneManager.CurrentGuardian != null && zoneManager.CurrentGuardian != NetworkSystem.Instance.LocalPlayer)
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

        public static void BreakAudioAll()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
            {
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", Photon.Pun.RpcTarget.Others, new object[] { 110, 99999 });
                GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", Photon.Pun.RpcTarget.Others, new object[] { 111, 99999 });
                Safety.RPCProc();
            }
        }

        public static void BreakAudioGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (lockTarget != null && gunLocked)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", lockTarget.Creator, new object[] { 110, 99999 });
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", lockTarget.Creator, new object[] { 111, 99999 });
                    Safety.RPCProc();
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }
        public static void BarrelFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (lockTarget != null && gunLocked)
                {
                    BarrelFling(lockTarget.transform.position, lockTarget.transform.position + new Vector3(0f, 999f, 0f), Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }
        public static void BarrelBringGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (lockTarget != null && gunLocked)
                {
                    BarrelFling(lockTarget.transform.position + (GorillaTagger.Instance.headCollider.transform.position - lockTarget.headMesh.transform.position).normalized * 0.1f, (GorillaTagger.Instance.bodyCollider.transform.position - lockTarget.transform.position).normalized * 5000f, Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }
        public static void BarrelCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (lockTarget != null && gunLocked)
                {
                    BarrelFling(lockTarget.transform.position + Vector3.down * 0.2f, lockTarget.bodyTransform.up * 10000f, Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }
        public static void CityBarrelKickGun()
        {
            string Map = Main.GetCurrentMapName();
            if (Map == "City")
            {
                if (GetGunInput(false))
                {
                    var GunData = RenderGun();
                    GameObject NewPointer = GunData.NewPointer;
                    RaycastHit Ray = GunData.Ray;

                    if (lockTarget != null && gunLocked)
                    {
                        BarrelFling(lockTarget.transform.position + (lockTarget.transform.position - new Vector3(-71.14215f, 13.73829f, -95.17883f)).normalized * 0.1f, (new Vector3(-71.14215f, 13.73829f, -95.17883f) - lockTarget.transform.position).normalized * 5000f, Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
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
                }
                else
                {
                    lockTarget = null;
                    gunLocked = false;
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=red>[ERROR]</color> You are not in city.");
                GetIndex("City Barrel Kick Gun").enabled = false;
            }
        }
        public static void BarrelPunchMod()
        {
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (!rig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f))
                {
                    Vector3 targetDirection = rig.headMesh.transform.position - GorillaTagger.Instance.headCollider.transform.position;
                    BarrelFling(rig.transform.position + (GorillaTagger.Instance.headCollider.transform.position - rig.headMesh.transform.position).normalized * 0.1f, targetDirection.normalized * 50f, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)), new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
                }
            }
        }
        private const int DeployableBarrelSlot = 618;
        private static Coroutine disableRoutine;
        private static float tpt;
        public static void BarrelFling(Vector3 position, Vector3 velocity, Quaternion rotation, RaiseEventOptions eventOptions = null)
        {
            eventOptions ??= new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            };
            DistancePatch.enabled = true;
            StopPreviousDisableTask();
            disableRoutine = CRunner.instance.StartCoroutine(RestoreBarrelState(DeployableBarrelSlot));
            TransferrableObject barrelObject = VRRig.LocalRig.myBodyDockPositions.allObjects[DeployableBarrelSlot];
            if (!barrelObject.gameObject.activeSelf)
            {
                VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, DeployableBarrelSlot);
                barrelObject.gameObject.SetActive(true);
            }
            barrelObject.storedZone = BodyDockPositions.DropPositions.RightArm;
            barrelObject.currentState = TransferrableObject.PositionState.InRightHand;
            if (barrelObject.gameObject.activeSelf && Time.time > tpt)
            {
                DeployableObject barrel = barrelObject.GetComponent<DeployableObject>();
                object[] data =
                {
                    barrel._deploySignal._signalID, PhotonNetwork.ServerTimestamp, BitPackUtils.PackWorldPosForNetwork(position),
                    BitPackUtils.PackQuaternionForNetwork(rotation), BitPackUtils.PackWorldPosForNetwork(velocity)
                };
                VRRig.LocalRig.transform.position = position;
                PhotonNetwork.RaiseEvent(177, data, eventOptions, new SendOptions { Reliability = false, DeliveryMode = DeliveryMode.ReliableUnsequenced });
                barrel._child.Deploy(barrel, position, rotation, velocity, false);
                barrel.DeployChild();
                Safety.RPCProc();
            }
        }

        private static void StopPreviousDisableTask()
        {
            if (disableRoutine != null)
            {
                CRunner.instance.StopCoroutine(disableRoutine);
            }
        }

        private static IEnumerator RestoreBarrelState(int index)
        {
            yield return new WaitForSeconds(0.3f);

            DistancePatch.enabled = false;
            VRRig.LocalRig.enabled = true;

            TransferrableObject barrel =
                VRRig.LocalRig.myBodyDockPositions.allObjects[index];

            barrel.gameObject.SetActive(true);

            barrel.storedZone = BodyDockPositions.DropPositions.RightArm;
            barrel.currentState = TransferrableObject.PositionState.OnRightArm;
        }
        public static float LagDelay = 0f;
        public static void LagPlayer(VRRig player, int ammount, float delay)
        {
            if (Time.time > LagDelay)
            {
                LagDelay = Time.time + delay;
                for (int i = 0; i < ammount; i++)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(186, new object[] { float.NaN }, new RaiseEventOptions { TargetActors = new[] { player.Creator.ActorNumber }, CachingOption = EventCaching.DoNotCache }, new SendOptions { Reliability = false, DeliveryMode = DeliveryMode.Unreliable });
                }
                Safety.RPCProc();
            }
        }
        public static void LagAll(int ammount, float delay)
        {
            if (Time.time > LagDelay)
            {
                LagDelay = Time.time + delay;
                for (int i = 0; i < ammount; i++)
                {
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(186, new object[] { float.NaN }, new RaiseEventOptions { Receivers = ReceiverGroup.Others }, new SendOptions { Reliability = false, DeliveryMode = DeliveryMode.Unreliable });
                }
                Safety.RPCProc();
            }
        }
        public static void BigStutterGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    LagPlayer(lockTarget, 1992, 4.4f); // set to 1999 if kicks
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }

        public static void StrongLagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    LagPlayer(lockTarget, 625, 1.7f); // set to 600 to 650 if kicks
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }

        public static void LagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    LagPlayer(lockTarget, 250, 0.5f);
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
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
                var rope = Rope.nodes.Skip(1).Select((v, i) => new { index = i, transform = v, distance = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, v.transform.position) }).OrderBy(x => x.distance).First();
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

        public static void DeafenAll()
        {
            for (int i = 0; i < 2; i++)
            {
                DeafenPlayer(ReceiverGroup.All);
            }
        }
        public static void DeafenGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    int[] target =
                    {
                        lockTarget.Creator.ActorNumber
                    };
                    for (int i = 0; i < 2; i++)
                    {
                        DeafenPlayer(target);
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }
        public static void DeafenPlayer(object player)
        {
            RaiseEventOptions raiseOptions;
            switch (player)
            {
                case ReceiverGroup group:
                    raiseOptions = new RaiseEventOptions { Receivers = group };
                    break;
                case int[] actors:
                    raiseOptions = new RaiseEventOptions { TargetActors = actors };
                    break;
                default:
                    return;
            }
            var sendOptions = new SendOptions
            {
                Channel = 0,
                Reliability = true
            };
            var voiceData = new Dictionary<byte, object>();
            voiceData[1] = 255;
            voiceData[2] = 48000;
            voiceData[3] = 2;
            voiceData[4] = 20000;
            voiceData[5] = 30000;
            voiceData[10] = null;
            voiceData[11] = (byte)0;
            voiceData[12] = (byte)11;
            PhotonVoiceNetwork.Instance.Client.OpRaiseEvent(202, new object[] { (byte)0, (byte)1, new object[] { voiceData } }, raiseOptions, sendOptions);
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

        public static bool IsBeingHeld(VRRig rig, VRRig remoteRig = null) => // im lazy so fuck you helper method -sleepy
            rig != null && ((!rig.leftHandLink.CanBeGrabbed() && (remoteRig == null || rig.leftHandLink.grabbedPlayer == remoteRig.GetPlayer())) || (!rig.rightHandLink.CanBeGrabbed() && (remoteRig == null || rig.rightHandLink.grabbedPlayer == remoteRig.GetPlayer())));

        public static void FlingOnGrab()
        {
            if (IsBeingHeld(VRRig.LocalRig))
            {
                Transform transform = VRRig.LocalRig.leftHandLink.IsLinkActive() ? VRRig.LocalRig.leftHandTransform : VRRig.LocalRig.rightHandTransform;
                VRRig rig = RigManager.GetVRRigFromPlayer(VRRig.LocalRig.leftHandLink.grabbedPlayer) ?? RigManager.GetVRRigFromPlayer(VRRig.LocalRig.rightHandLink.grabbedPlayer);
                Vector3 velocity = rig.transform.up * 5f;
                rig.netView.SendRPC("DroppedByPlayer", rig.Creator, velocity);
            }
        }

        public static void CrashOnGrab()
        {
            if (IsBeingHeld(VRRig.LocalRig))
            {
                Transform transform = VRRig.LocalRig.leftHandLink.IsLinkActive() ? VRRig.LocalRig.leftHandTransform : VRRig.LocalRig.rightHandTransform;
                VRRig rig = RigManager.GetVRRigFromPlayer(VRRig.LocalRig.leftHandLink.grabbedPlayer) ?? RigManager.GetVRRigFromPlayer(VRRig.LocalRig.rightHandLink.grabbedPlayer);
                Vector3 velocity = rig.transform.up * -15f;
                rig.netView.SendRPC("DroppedByPlayer", rig.Creator, velocity);
            }
        }
        public static void ForceGrabAll()
        {
            foreach (VRRig rig in VRRigCache.ActiveRigs)
                ForceGrabTestButBetterTrust(rig, VRRig.LocalRig.transform.position);
        }
        public static void ForceGrabGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    ForceGrabTestButBetterTrust(lockTarget, VRRig.LocalRig.transform.position);
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
            }
            else
            {
                lockTarget = null;
                gunLocked = false;
            }
        }
        private static float grabCooldown;
        // tbh might just be the same as old but its working? -sleepy
        public static bool ForceGrabTestButBetterTrust(VRRig target, Vector3 position, bool returnOnGrab = false, bool enableRigAfter = false)
        {
            if (target == null || target.IsLocal()) // yes this SOMEHOW happend to me. -sleepy
                return false;

            bool doleft = target.leftHandLink.CanBeGrabbed();
            bool doright = target.rightHandLink.CanBeGrabbed();
            if (!doleft && !doright)
            {
                VRRig.LocalRig.enabled = true;
                return false;
            }
            VRRig local = VRRig.LocalRig;
            local.enabled = false;
            local.transform.position = target.syncPos;
            var localHand = doleft ? local.leftHandLink : local.rightHandLink;
            var targetHand = doright ? target.leftHandLink : target.rightHandLink;

            if (targetHand.grabbedPlayer == NetworkSystem.Instance.LocalPlayer)
            {
                if (returnOnGrab)
                {
                    if (enableRigAfter)
                        local.enabled = true;

                    return true;
                }

                return false;
            }

            if (grabCooldown <= 0f)
                grabCooldown = Mathf.Max(targetHand.rejectGrabsUntilTimestamp, Time.time + 1f);

            if (Time.time >= grabCooldown)
            {
                localHand.TentacleTryCreateLink(targetHand);
                local.transform.position = position;
                NotificationManager.SendNotification($"<color=grey>[</color><color=cyan>ARRAKIS</color><color=grey>]</color> Attempted to grab {target.GetPlayer().NickName}.");
                grabCooldown = Mathf.Max(targetHand.rejectGrabsUntilTimestamp, Time.time + 1f);
            }
            return false;
        }
    }
}