/*
 * Arrakis | Mods/Master.cs
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

using Arrakis.Managers;
using Arrakis.Notifications;
using GorillaTagScripts;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;
using static Arrakis.Menu.Main;

namespace Arrakis.Mods
{
    public class Master
    {
        public static void UntagAll()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (NetPlayer plr in NetworkSystem.Instance.PlayerListOthers)
                {
                    foreach (GorillaTagManager tag in GameObject.FindObjectsByType<GorillaTagManager>(FindObjectsSortMode.None))
                    {
                        tag.currentInfected.Remove(plr);
                        tag.InfectionRoundEnd();
                    }
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
            }
        }

        public static void UntagSelf()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (GorillaTagManager tag in GameObject.FindObjectsByType<GorillaTagManager>(FindObjectsSortMode.None))
                {
                    tag.currentInfected.Remove(NetworkSystem.Instance.LocalPlayer);
                    tag.InfectionRoundEnd();
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
            }
        }

        public static void NoGuardian()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (GorillaGuardianZoneManager zone in GuardianZMan())
                {
                    if (zone.IsZoneValid() && zone.CurrentGuardian == NetworkSystem.Instance.LocalPlayer)
                        zone.SetGuardian(null);
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
            }
        }

        public static void BecomeGuardian()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (TappableGuardianIdol t in GetAllTappables())
                {
                    if (t.manager != null && !t.isChangingPositions)
                    {
                        GorillaGuardianZoneManager zoneManager = t.zoneManager;
                        if (zoneManager.IsZoneValid() && t.manager != null && zoneManager.CurrentGuardian == null)
                            zoneManager.SetGuardian(NetworkSystem.Instance.LocalPlayer);
                    }
                }
            }
            else
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
            }
        }

        public static void SetGuardianGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        VRRig rig = Ray.collider.GetComponentInParent<VRRig>();
                        if (rig != null && rig != VRRig.LocalRig)
                        {
                            foreach (TappableGuardianIdol t in GetAllTappables())
                            {
                                if (t.manager != null && !t.isChangingPositions)
                                {
                                    GorillaGuardianZoneManager zoneManager = t.zoneManager;
                                    if (zoneManager.IsZoneValid() && t.manager != null && zoneManager.CurrentGuardian == null)
                                        zoneManager.SetGuardian(rig.Creator);
                                }
                            }
                        }
                    }
                    else
                    {
                        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
                    }
                }
            }
        }

        public static void GetAllPhotonViews() // Dictionary
        {
            foreach (PhotonView view in GetPhotonViews())
            {
                Color tracerColor = Color.cyan;
                GameObject holder = new GameObject("PhotonViewTracer");
                LineRenderer line = holder.AddComponent<LineRenderer>();
                line.useWorldSpace = true;
                line.material = new Material(Shader.Find("GUI/Text Shader"));
                line.positionCount = 2;
                line.startWidth = 0.02f;
                line.endWidth = 0.02f;
                line.startColor = tracerColor;
                line.endColor = tracerColor;
                line.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                line.SetPosition(1, view.transform.position);
                Object.Destroy(holder, Time.deltaTime);
            }
        }

        public static void DestroyViewGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        PhotonView photonView = Ray.collider.GetComponentInParent<PhotonView>();
                        PhotonNetwork.Destroy(photonView);
                    }
                    else
                    {
                        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
                    }
                }
            }
        }

        public static void UnGuardianGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        VRRig rig = Ray.collider.GetComponentInParent<VRRig>();
                        if (rig != null && rig != VRRig.LocalRig)
                        {
                            foreach (GorillaGuardianZoneManager zone in GuardianZMan())
                            {
                                if (zone.IsZoneValid() && zone.CurrentGuardian == rig.Creator)
                                    zone.SetGuardian(null);
                            }
                        }
                    }
                    else
                    {
                        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
                    }
                }
            }
        }

        public static void UntagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        VRRig rig = Ray.collider.GetComponentInParent<VRRig>();
                        if (rig != null && rig != VRRig.LocalRig)
                        {
                            foreach (GorillaTagManager tag in GameObject.FindObjectsByType<GorillaTagManager>(FindObjectsSortMode.None))
                            {
                                if (tag.currentInfected.Contains(rig.Creator))
                                {
                                    tag.currentInfected.Remove(rig.Creator);
                                    tag.InfectionRoundEnd();
                                }
                            }
                        }
                    }
                    else
                    {
                        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
                    }
                }
            }
        }
        public static List<int> blockIds = new List<int> // NOT MINE CREDS TO SOME RANDOM KID IN A MODDING SERVER -sleepy
        {
            857098599, -2063561053, 1510110959, 1848143946, 866161220,
            -604999206, 1844542113, -1514335082, 868696147, -460092905,
            1000122295, 757678001, 1513669651, -1537067750, 944837962,
            -1499829961, -604288536, -709037470, -1724151965, -350300979,
            1858470402, 1794855203, -1326806786, 724396559,
            1755661147, 1459635109, -566818631, 1925587737, -1441835191,
            -1324502924, 111152940, 798264081, -1821684029, 1895524638,
            1961336659, -1059201160, 1051576141, 539529939, -1535427925,
            1210710592, 1228919111, 252298128, -648273975, 1120512569,
            532163265, -845420418, 1834228748, 1063967233, 1700948013,
            2059548340, -1447051713, 1134055607, 1700655257, -1724819324,
            -1218055069, 251444537, -1446121736, -1927069002, -385891195,
            -196038879, -993249117, 1145900217, 1859614656, 1821589092,
            661312857, 1701825380, -1621444201, 1924370326, -1193326485,
            -1194390666, -751675075, -933358727, 24270440,
        };
        public static System.Random random = new System.Random();
        public static int GetRandomId()
        {
            int index = random.Next(blockIds.Count);
            return blockIds[index];
        }
        public static void SpawnBlock(int id, Vector3 position, Quaternion rotation, RpcTarget target = RpcTarget.All) // if nova says this looks bad im gonna cry bcz i think it looks good -sleepy
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                BuilderTable table = GameObject.Find("Environment Objects/MonkeBlocksRoomPersistent/BuilderTable").GetComponent<BuilderTable>();
                GameObject.Find("Environment Objects/MonkeBlocksRoomPersistent/BuilderNetworking").GetComponent<BuilderTableNetworking>().photonView.RPC("PieceCreatedByShelfRPC", target, new object[] { id, table.CreatePieceId(), BitPackUtils.PackWorldPosForNetwork(position), BitPackUtils.PackQuaternionForNetwork(rotation), 0, (byte)4, 1, PhotonNetwork.LocalPlayer });
            }
        }
        public static void DestroyBlock(int id, Vector3 position, Quaternion rotation, bool PlaySfx)
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                BuilderTableNetworking network = GameObject.Find("Environment Objects/MonkeBlocksRoomPersistent/BuilderNetworking").GetComponent<BuilderTableNetworking>();
                network.photonView.RPC("PieceDestroyedRPC", RpcTarget.All, new object[] { id, BitPackUtils.PackWorldPosForNetwork(position), BitPackUtils.PackQuaternionForNetwork(rotation), PlaySfx, (short)1 });
            }
        }
        public static void SpawnBlockGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    SpawnBlock(GetRandomId(), NewPointer.gameObject.transform.position, RandomQuaternion());
                }
            }
        }
        public static void BlockTrapGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    for (int i = 0; i < 12; i++)
                        SpawnBlock(-1447051713, lockTarget.transform.position, RandomQuaternion());
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
        public static void ChangeLavaState(InfectionLavaController.RisingLavaState state)
        {
            if (!PhotonNetwork.LocalPlayer.IsMasterClient) { NotificationManager.SendNotification("<color=cyan>[ARRAKIS]</color> You are not master client this mod will not work."); return; }
            var lava = InfectionLavaController.ActiveControllers.FirstOrDefault();
            if (lava == null)
                return;
            double startTime = NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble;
            lava.JumpToState(state);
            lava.reliableState.stateStartTime = startTime;
        }
        public static BuilderPiece piece = null;
        public static void BlockCrashAll()
        {
            for (int i = 0; i < 2; i++)
            {
                SpawnBlock(-1447051713, VRRig.LocalRig.transform.position, Quaternion.identity, RpcTarget.Others);
                if (piece == null)
                    piece = GameObject.FindObjectOfType<BuilderPiece>();
                if (piece.pieceType == -1447051713 || piece.pieceId == -1447051713)
                    piece.gameObject.SetActive(false);
            }
        }
        public static void BlockSphere(float radius = 2f, int density = 40)
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            Vector3 center = GorillaTagger.Instance.offlineVRRig.transform.position;
            for (int i = 0; i < density * 2; i++)
            {
                float theta = 2 * Mathf.PI * i / ((1 + Mathf.Sqrt(5)) / 2);
                float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / (density * 2));
                Vector3 pos = center + new Vector3(radius * Mathf.Sin(phi) * Mathf.Cos(theta), radius * Mathf.Sin(phi) * Mathf.Sin(theta), radius * Mathf.Cos(phi));
                SpawnBlock(GetRandomId(), pos, Quaternion.identity);
            }

        }
        public static GhostReactorManager _GRM;
        public static void GetGRM()
        {
            if (_GRM == null)
            {
                _GRM = GameObject.FindAnyObjectByType<GhostReactorManager>();
            }
        }
        /*
            -298662477  | orb
            931983585   | id badge    
            -20887423   | flow
            373410214   | create
            -1724683316 | barrel
            -398809473  | large orb?
            -175001459  | GhostReactorToolClub
            -298662477  | GhostReactorCollectibleCoreFlowerVariant
            1115277044  | GhostReactorToolLantern
            1989693521  | GhostReactorToolCollector
            1165678479  | GhostReactorToolRevive
            -1495476618 | GhostReactorToolShieldGun
            -531028875  | GhostReactorToolDirectionalShield
            166197108   | GhostReactorCollectibleCore
            -2120233750 | GhostReactorToolHockeyStick
            -1949194188 | GhostReactorBreakableCrateTool
            418765863   | GhostReactorCollectibleSentientCore
            -697215200  | GhostReactorHazardTower
            1978670241  | GhostReactorHazardTowerProjectile
            -1086813702 | GhostReactorWallLight01
            -830469733  | GhostReactorToolClubSuper
            -1235906457 | GhostReactorToolDockWrist
            225241881   | GhostReactorToolFlash
            400631847   | GhostReactorToolFlashSuper
            252997812   | GhostReactorToolSmallBackpack
            -502169320  | GhostReactorToolStatusWatch
            -687044547  | GRLanternFlare
            -1229767062 | GRUBatonDamage1
            270266995   | GRUBatonDamage2
            1907002827  | GRUBatonDamage3
            -426726616  | GRUCollectorBonus1
            769995104   | GRUCollectorBonus2
            -1686893135 | GRUCollectorBonus3
            1735390800  | GRUDirectionalShieldSize1
            -922807515  | GRUDirectionalShieldSize2
            -1503477586 | GRUDirectionalShieldSize3
            220032407   | GRUFlashDamage1
            -1183128012 | GRUFlashDamage2
            1203104201  | GRUFlashDamage3
            -1314917544 | GRULanternIntensity1
            -228409232  | GRULanternIntensity2
            1477289365  | GRULanternIntensity3
            -311546458  | GRUPowerEff1
            337645388   | GRUPowerEff2
            -1709004991 | GRUPowerEff3
            -1339298990 | GRUShieldGunStrength1
            672910098   | GRUShieldGunStrength2
            -1517122485 | GRUShieldGunStrength3
            -735557236 | Door
        */
        public static void DoorTrapGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    for (int i = 0; i < 4; i++)
                        SpawnThingGhostReact(-735557236, lockTarget.transform.position, RandomQuaternion());
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
        public static void SpawnThingGhostReact(int hash, Vector3 position, Quaternion rotation, long[] createData = null)
        {
            GetGRM();

            bool inGhostReactor = ZoneManagement.instance.IsZoneActive(GTZone.ghostReactor) || ZoneManagement.instance.IsZoneActive(GTZone.ghostReactorDrill) || ZoneManagement.instance.IsZoneActive(GTZone.ghostReactorTunnel);

            if (!inGhostReactor)
                return;
            if (!InputManager.GetInput(InputManager.InputType.Grip, InputManager.Hand.Right, !XRSettings.isDeviceActive))
                return;
            MethodInfo createNetId = typeof(GameEntityManager).GetMethod("CreateNetId", BindingFlags.Instance | BindingFlags.NonPublic);
            int netId = (int)createNetId.Invoke( _GRM.gameEntityManager,  new object[] { 1 });
            createData ??= new long[] { 0L };
            _GRM.gameEntityManager.photonView.RPC("CreateItemRPC", RpcTarget.AllBuffered, new int[] { netId }, new int[] { hash }, new long[] { BitPackUtils.PackWorldPosForNetwork(position) }, new int[] { BitPackUtils.PackQuaternionForNetwork(rotation) }, createData, new int[] { 0 });
            Safety.RPCProc();
        }
        public static void VIMKickAll()
        {
            if (!SubscriptionManager.Instance.subData[VRRig.LocalRig.Creator].active)
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color>You are not a VIM subscriber this mod will not work.");
                return;
            }
            if (!PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color>You are not master this mod will not work.");
                return;
            }
            if (!NetworkSystem.Instance.CurrentRoom.isPublic)
            {
                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color>Room is not private this mod will not work.");
                return;
            }
            NetworkSystem.Instance.PlayerListOthers.ForEach(p => RoomControls.KickPlayer(p.ActorNumber));
        }

        // Lucy mods so we can release when october i know its early but yea -nova
        //public static HalloweenGhostChaser Lucy
        //{
        //    get
        //    {
        //        return GameObject.Find("idk lucys 2026 halloween path yet since its not even october").GetComponent<HalloweenGhostChaser>();
        //    }
        //}
        //
        //public static void SpawnBlueLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        Lucy.timeGongStarted = 0f;
        //        Lucy.isSummoned = false;
        //        Lucy.currentState = HalloweenGhostChaser.ChaseState.Gong;
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //public static void SpawnRedLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        Lucy.timeGongStarted = 0f;
        //        Lucy.isSummoned = true;
        //        Lucy.currentState = HalloweenGhostChaser.ChaseState.Gong;
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //public static void DespawnLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        Lucy.isSummoned = false;
        //        Lucy.currentState = HalloweenGhostChaser.ChaseState.Dormant;
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //public static void FastLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        Lucy.currentSpeed = 3f;
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //public static void SlowLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        Lucy.currentSpeed = 0.1f;
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //
        //private static float lucyspaztimething = 0f;
        //public static void SpazLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        if (Time.time > lucyspaztimething)
        //        {
        //            Lucy.currentState = Lucy.currentState == HalloweenGhostChaser.ChaseState.InitialRise ? HalloweenGhostChaser.ChaseState.Gong : HalloweenGhostChaser.ChaseState.InitialRise;
        //            lucyspaztimething = Time.time + 0.5f;
        //        }
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //
        //private static float lucyspaztarget = 0f;
        //public static void SpazLucyTarget()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        if (Time.time > lucyspaztarget)
        //        {
        //            Lucy.currentState = HalloweenGhostChaser.ChaseState.Chasing;
        //            Lucy.targetPlayer = RigManager.GetRandomVRRig(true).Creator;
        //            lucyspaztarget = Time.time + 0.5f;
        //        }
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
        //
        //public static void MoveLucyGun()
        //{
        //    if (GetGunInput(false))
        //    {
        //        var GunData = RenderGun();
        //        GameObject NewPointer = GunData.NewPointer;
        //        RaycastHit Ray = GunData.Ray;
        //        if (GetGunInput(true))
        //        {
        //            if (Lucy.IsMine)
        //            {
        //                Lucy.transform.position = NewPointer.transform.position;
        //            }
        //            else
        //            {
        //                NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //            }
        //        }
        //    }
        //}
        //
        //public static void GrabLucy()
        //{
        //    if (Lucy.IsMine)
        //    {
        //        if (InputManager.GetInput(InputManager.InputType.Grip, InputManager.Hand.Right, !XRSettings.isDeviceActive))
        //        {
        //            Lucy.targetPlayer = null;
        //            Lucy.currentState = HalloweenGhostChaser.ChaseState.Chasing;
        //            Lucy.transform.position = VRRig.LocalRig.rightHandTransform.position;
        //        }
        //        if (InputManager.GetInput(InputManager.InputType.Grip, InputManager.Hand.Right, !XRSettings.isDeviceActive))
        //        {
        //            Lucy.targetPlayer = null;
        //            Lucy.currentState = HalloweenGhostChaser.ChaseState.Chasing;
        //            Lucy.transform.position = VRRig.LocalRig.leftHandTransform.position;
        //        }
        //    }
        //    else
        //    {
        //        NotificationManager.SendNotification("<color=yellow>[ARRAKIS]</color> You are not master client this mod wont work.");
        //    }
        //}
    }
}