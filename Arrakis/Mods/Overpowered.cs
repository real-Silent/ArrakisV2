using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Arrakis.Classes;
using Arrakis.Notifications;
using Arrakis.Patches;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using Photon.Realtime;
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
        public static void BarrelCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (lockTarget != null && gunLocked)
                {
                    BarrelFling(lockTarget.transform.position, lockTarget.bodyTransform.up * 1000f, Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
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
        public static void BarrelExucuteGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;

                if (lockTarget != null && gunLocked)
                {
                    BarrelFling(lockTarget.transform.position, new Vector3(800f, 800f, 800f), Quaternion.identity, new RaiseEventOptions { TargetActors = new[] { RigManager.GetNetPlayerFromVRRig(lockTarget).ActorNumber } });
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

        private const int DeployableBarrelSlot = 618;
        private static Coroutine disableRoutine;
        private static float tpt;
        public static void BarrelFling(Vector3 position, Vector3 velocity, Quaternion rotation, RaiseEventOptions eventOptions = null) // made better
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
        private static float nextgrabtime;
        public static bool CheckHandLinks(VRRig monkey)
        {
            if (monkey == null) return false;
            return monkey.leftHandLink.CanBeGrabbed() || monkey.rightHandLink.CanBeGrabbed();
        }
        private static void UpdateGrabStatus(bool isActive)
        {
            GrabPatch.enabled = isActive;
            if (!isActive && !VRRig.LocalRig.enabled)
            {
                VRRig.LocalRig.enabled = true;
            }
        }
        public static void ForceGrabTest(VRRig target, Vector3 tpTarget)
        {
            if (target == null || target == VRRig.LocalRig) return;

            if (!CheckHandLinks(target))
            {
                UpdateGrabStatus(false);
                VRRig.LocalRig.BreakHandLinks();
                VRRig.LocalRig.enabled = true;
                return;
            }
            UpdateGrabStatus(true);
            VRRig.LocalRig.enabled = false;
            VRRig.LocalRig.transform.position = tpTarget;
            bool preferLeft = target.leftHandLink.CanBeGrabbed();
            TakeMyHand_HandLink theirHand = preferLeft ? target.leftHandLink : target.rightHandLink;
            TakeMyHand_HandLink ourHand = preferLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink;
            if (theirHand.grabbedPlayer == NetworkSystem.Instance.LocalPlayer) return;
            if (nextgrabtime <= 0f)
            {
                nextgrabtime = theirHand.rejectGrabsUntilTimestamp > Time.time ? theirHand.rejectGrabsUntilTimestamp : Time.time + 0.2f;
            }
            if (Time.time <= nextgrabtime) return;
            VRRig.LocalRig.transform.position = target.syncPos;
            ourHand.TentacleTryCreateLink(theirHand);
            nextgrabtime = theirHand.rejectGrabsUntilTimestamp > Time.time ? theirHand.rejectGrabsUntilTimestamp : Time.time + 0.2f;
        }
        public static void GrabFlingGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    float randX = UnityEngine.Random.Range(0, 2) == 0 ? -95000f : 95000f;
                    float randZ = UnityEngine.Random.Range(0, 2) == 0 ? -95000f : 95000f;
                    ForceGrabTest(lockTarget, new Vector3(randX, 95000f, randZ));
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
            bool noInput = !ControllerInputPoller.instance.rightGrab && !ControllerInputPoller.instance.leftGrab && !ControllerInputPoller.instance.rightControllerTriggerButton && !ControllerInputPoller.instance.leftControllerTriggerButton;
            if (noInput && Patches.GrabPatch.enabled)
            {
                UpdateGrabStatus(false);
                VRRig.LocalRig.BreakHandLinks();
                VRRig.LocalRig.enabled = true;
            }
        }

        public static void GrabCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    ForceGrabTest(lockTarget, new Vector3(0, -900f, 0));
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
            bool noInput = !ControllerInputPoller.instance.rightGrab && !ControllerInputPoller.instance.leftGrab && !ControllerInputPoller.instance.rightControllerTriggerButton && !ControllerInputPoller.instance.leftControllerTriggerButton;
            if (noInput && Patches.GrabPatch.enabled)
            {
                UpdateGrabStatus(false);
                VRRig.LocalRig.BreakHandLinks();
                VRRig.LocalRig.enabled = true;
            }
        }

        public static void CityGrabKickGun()
        {
            string mapName = GetCurrentMapName();
            if (mapName == "City")
            {
                if (GetGunInput(false))
                {
                    var GunData = RenderGun();
                    GameObject NewPointer = GunData.NewPointer;
                    RaycastHit Ray = GunData.Ray;
                    if (lockTarget != null && gunLocked)
                    {
                        ForceGrabTest(lockTarget, lockTarget.transform.position - new Vector3(-88.1699f, 141.4335f, -161.3968f));
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
                bool noInput = !ControllerInputPoller.instance.rightGrab && !ControllerInputPoller.instance.leftGrab && !ControllerInputPoller.instance.rightControllerTriggerButton && !ControllerInputPoller.instance.leftControllerTriggerButton;
                if (noInput && Patches.GrabPatch.enabled)
                {
                    UpdateGrabStatus(false);
                    VRRig.LocalRig.BreakHandLinks();
                    VRRig.LocalRig.enabled = true;
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=red>[ERROR]</color> You are not in city.");
                GetIndex("Grab City Kick Gun").enabled = false;
            }
        }
        public static void MetroGrabKickGun()
        {
            string mapName = GetCurrentMapName();
            if (mapName == "Metropolis")
            {
                if (GetGunInput(false))
                {
                    var GunData = RenderGun();
                    GameObject NewPointer = GunData.NewPointer;
                    RaycastHit Ray = GunData.Ray;
                    if (lockTarget != null && gunLocked)
                    {
                        ForceGrabTest(lockTarget, lockTarget.transform.position - new Vector3(97.4061f, -551.25f, -294.1043f));
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
                bool noInput = !ControllerInputPoller.instance.rightGrab && !ControllerInputPoller.instance.leftGrab && !ControllerInputPoller.instance.rightControllerTriggerButton && !ControllerInputPoller.instance.leftControllerTriggerButton;
                if (noInput && Patches.GrabPatch.enabled)
                {
                    UpdateGrabStatus(false);
                    VRRig.LocalRig.BreakHandLinks();
                    VRRig.LocalRig.enabled = true;
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=red>[ERROR]</color> You are not in Metropolis.");
                GetIndex("Grab Metro Kick Gun").enabled = false;
            }
        }

        public static void GrabBreakMovementGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    ForceGrabTest(lockTarget, new Vector3(lockTarget.transform.position.x, UnityEngine.Random.Range(-3f, 3f), lockTarget.transform.position.z));
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
            bool noInput = !ControllerInputPoller.instance.rightGrab && !ControllerInputPoller.instance.leftGrab && !ControllerInputPoller.instance.rightControllerTriggerButton && !ControllerInputPoller.instance.leftControllerTriggerButton;
            if (noInput && Patches.GrabPatch.enabled)
            {
                UpdateGrabStatus(false);
                VRRig.LocalRig.BreakHandLinks();
                VRRig.LocalRig.enabled = true;
            }
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
                    LagPlayer(lockTarget, 1992, 4.1f); // set to 1999 if kicks
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
                    LagPlayer(lockTarget, 625, 1.5f); // set to 600 to 650 if kicks
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
        public static void ChangeLavaState(InfectionLavaController.RisingLavaState state) // master ss? -sleepy
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient) NotificationManager.SendNotification("<color=cyan>[ARRAKIS]</color> You are not master client this mod will have a huge delay to be ss.");
            var lava = InfectionLavaController.ActiveControllers.FirstOrDefault();
            if (lava == null)
                return;
            double startTime = NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble;
            lava.JumpToState(state);
            lava.reliableState.stateStartTime = startTime;
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
    }
}