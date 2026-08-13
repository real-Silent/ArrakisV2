/*
 * Arrakis | Mods/Experimental.cs
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
using System.IO;
using System.Linq;
using Arrakis.Classes;
using Arrakis.Classes.Menu;
using Arrakis.Notifications;
using Arrakis.Patches.Patchers;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using UnityEngine;
using UnityEngine.InputSystem;
using static Arrakis.Classes.RigManager;
using static Arrakis.Menu.Main;
using JoinType = GorillaNetworking.JoinType;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using HarmonyLib;

namespace Arrakis.Mods
{
    public class Experimental
    {
        public static void GetRPCData()
        {
            File.WriteAllLines($"{PluginInfo.BaseDirectory}/RPCData.txt", PhotonNetwork.PhotonServerSettings.RpcList);
        }

        public static void GetTryonCosmetics()
        {
            foreach (CosmeticsController.CosmeticItem item in CosmeticsController.instance.allCosmetics)
            {
                if (item.canTryOn && item.cost != 0)
                {
                    CustomConsole.Log($"Found all items {(item.itemName != "" ? item.itemName : item.overrideDisplayName)} , {item.appliedCosmeticPlayFabID} , {item.cost}", CustomConsole.LogType.Info);
                }
            }
        }

        public static void ElevatorKickAll() =>
           GRElevatorManager._instance.SendRPC("RemoteElevatorButtonPress", RpcTarget.MasterClient, new[] { 3, (int)GRElevatorManager._instance.currentLocation });

        public static void KickAllInParty()
        {
            if (FriendshipGroupDetection.Instance.IsInParty)
            {
                PhotonNetworkController.Instance.AttemptToJoinSpecificRoom("ARRAKIS ON TOP", JoinType.ForceJoinWithParty);
                NotificationManager.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> Kicking party members, please be patient..");
            }
            else
                NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a party.");
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
        public static void DeafenPlayer(object player) // this looks so ass -sleepy
        {
            RaiseEventOptions raiseOptions;

            switch (player)
            {
                case ReceiverGroup group:
                    raiseOptions = new RaiseEventOptions
                    {
                        Receivers = group
                    };
                    break;
                case int[] actors:
                    raiseOptions = new RaiseEventOptions
                    {
                        TargetActors = actors
                    };
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
        private static float instantPartyDelay;
        public static void InstantParty() // ai genned looking ahh code 🙏 -sleepy
        {
            if (Time.time <= instantPartyDelay)
                return;
            instantPartyDelay = Time.time + 0.1f;
            var detector = FriendshipGroupDetection.Instance;
            detector.suppressPartyCreationUntilTimestamp = 0f;
            detector.groupCreateAfterTimestamp = 0f;
            if (detector.playersInProvisionalGroup.Count == 0)
                return;
            Color braceletColor = GTColor.RandomHSV(detector.braceletRandomColorHSVRanges);
            Color myBraceletColor = Traverse.Create(detector).Field("myBraceletColor").GetValue<Color>(); // Shitty fix but might work -nova
            myBraceletColor = braceletColor;
            //detector.myBraceletColor = braceletColor;
            List<int> partyMembers = new List<int>();
            partyMembers.Add(PhotonNetwork.LocalPlayer.ActorNumber);
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
            {
                bool isMember = detector.IsInMyGroup(player.UserId) || detector.playersInProvisionalGroup.Contains(player.ActorNumber);
                if (isMember)
                    partyMembers.Add(player.ActorNumber);
            }
            detector.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(braceletColor), partyMembers.ToArray(), false);
            Safety.RPCProc();
        }

        private static float breakDelay;
        public static void PartyBreakNetworkTriggers()
        {
            var detector = FriendshipGroupDetection.Instance;
            if (!detector.IsInParty)
                return;
            if (Time.time <= breakDelay)
                return;
            breakDelay = Time.time + 0.4f;
            detector.photonView.RPC("PartyMemberIsAboutToGroupJoin", RpcTarget.All, Array.Empty<object>());
        }
        private static bool antiKickEvents;
        public static bool AntiKickEvents // kidnda works? -sleepy
        {
            get
            {
                return antiKickEvents;
            }

            set
            {
                if (antiKickEvents == value)
                    return;

                antiKickEvents = value;

                if (value)
                {
                    PhotonNetwork.SerializationRate = 2;

                    AntiEventThingIgIdfk.FilteredRPCs["OnHandTapRPC"] = () => false;
                    AntiEventThingIgIdfk.FilteredRPCs["RPC_UpdateCosmeticsWithTryonPacked"] = () => false;

                    EventPatches.Override = delegate
                    {
                        SendSerialize(VRRig.LocalRig.netView.GetView);
                        return true;
                    };
                }
                else
                {
                    PhotonNetwork.SerializationRate = 10;

                    EventPatches.Override = null;

                    AntiEventThingIgIdfk.FilteredRPCs.Remove("OnHandTapRPC");
                    AntiEventThingIgIdfk.FilteredRPCs.Remove("RPC_UpdateCosmeticsWithTryonPacked");
                }
            }
        }
        public static void SendSerialize(PhotonView view, RaiseEventOptions options = null, int timeOffset = 0, float delay = 0f) // used chatgpt to clean up this shit of a function, it looked so ass before -sleepy
        {
            if (!NetworkSystem.Instance.InRoom)
                return;

            if (view == null)
            {
                CustomConsole.Log("PhotonView was null.", CustomConsole.LogType.Error);
                return;
            }

            List<object> data = PhotonNetwork.OnSerializeWrite(view);

            if (data == null || data.Count == 0)
                return;

            bool reliable = view.Synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable;

            var batchKey = new PhotonNetwork.RaiseEventBatch
            {
                Reliable = reliable,
                Group = view.Group
            };

            var batches = PhotonNetwork.serializeViewBatches;

            if (!batches.ContainsKey(batchKey))
            {
                batches.Add(batchKey, new PhotonNetwork.SerializeViewBatch(batchKey, 2));
            }

            PhotonNetwork.SerializeViewBatch batch =
                (PhotonNetwork.SerializeViewBatch)batches[batchKey];

            batch.Add(data);

            RaiseEventOptions raiseOptions;

            if (options == null)
            {
                raiseOptions = PhotonNetwork.serializeRaiseEvOptions;
            }
            else
            {
                raiseOptions = new RaiseEventOptions
                {
                    CachingOption = PhotonNetwork.serializeRaiseEvOptions.CachingOption,
                    Flags = PhotonNetwork.serializeRaiseEvOptions.Flags,
                    InterestGroup = PhotonNetwork.serializeRaiseEvOptions.InterestGroup,
                    Receivers = options.Receivers,
                    TargetActors = options.TargetActors
                };
            }

            List<object> payload = batch.ObjectUpdates;

            payload[0] = PhotonNetwork.ServerTimestamp + timeOffset;
            payload[1] = PhotonNetwork.currentLevelPrefix == 0 ? null : (object)PhotonNetwork.currentLevelPrefix;

            byte eventCode = (byte)(reliable ? Photon.Pun.PunEvent.SendSerializeReliable : Photon.Pun.PunEvent.SendSerialize);

            SendOptions send = reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable;

            if (delay > 0f)
            {
                List<object> copiedPayload = new List<object>(payload);

                CRunner.instance.StartCoroutine(
                    SerializationDelay(() =>
                    {
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(eventCode, copiedPayload, raiseOptions, send);
                    }, delay));
            }
            else
            {
                PhotonNetwork.NetworkingClient.OpRaiseEvent(eventCode, payload, raiseOptions, send);
            }

            batch.Clear();
        }

        public static IEnumerator SerializationDelay(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);

            callback?.Invoke();
        }
        public static void PartyLagGun() // kicks after a while? -sleepy
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked && FriendshipGroupDetection.Instance.IsInMyGroup(lockTarget.Creator.GetPlayerRef().UserId))
                {
                    AntiKickEvents = true;
                    for (int i = 0; i < 4000; i++)
                        FriendshipGroupDetection.Instance.photonView.RPC("RequestPartyGameMode", lockTarget.Creator.GetPlayerRef(), new object[] { GameMode.gameModeKeyByName.Keys.ToArray()[Random.Range(0, GameMode.gameModeKeyByName.Keys.Count)] });

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
                AntiKickEvents = false;
            }
        }
        public static float plagDelay;
        public static void PartyLagAll()
        {
            if (FriendshipGroupDetection.Instance.IsInParty && Time.time > plagDelay)
            {
                plagDelay = Time.time + 10f;
                var partyPlayers = NetworkSystem.Instance.PlayerListOthers.Where(plr => FriendshipGroupDetection.Instance.IsInMyGroup(plr.UserId)).ToList();
                for (int i = 0; i < 4000; i++)
                {
                    foreach (var plr in partyPlayers)
                    {
                        var photonPlayer = PhotonNetwork.CurrentRoom.GetPlayer(plr.ActorNumber);
                        FriendshipGroupDetection.Instance.photonView.RPC("RequestPartyGameMode", photonPlayer, new object[] { GameMode.gameModeKeyByName.Keys.ToArray()[Random.Range(0, GameMode.gameModeKeyByName.Keys.Count)] });
                    }
                }
                Safety.RPCProc();
            }
        }
        public static readonly string prop = "arrakis v" + PluginInfo.Version;
        public static bool HasProp(Player player)
        {
            return player.CustomProperties.ContainsKey(prop);
        }
        public static List<Player> GetAllArrakisUsers()
        {
            List<Player> Users = new List<Player>();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (HasProp(p))
                    Users.Add(p);
            }
            return Users;
        }
        public static void SpamPrideCube()
        {
            if (ControllerInputPoller.instance.rightIndexPressed || Mouse.current.leftButton.isPressed)
            {
                Texture2D texture = AssetBundleLoader.LoadTexture("pride.png");
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit")); // fixed -nova
                mat.mainTexture = texture;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                cube.transform.localScale = Vector3.one * 0.25f;
                Renderer renderer = cube.GetComponent<Renderer>();
                renderer.material = mat;
                Rigidbody rb = cube.AddComponent<Rigidbody>();
                rb.mass = 1f;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                Object.Destroy(cube, 5f);
            }
        }

        public static void VStumpCrashPlayer(int actorNumber) => // map locked? 180 worked once -sleepy
            PhotonNetwork.RaiseEvent((byte)UnityEngine.Random.Range(180, 190), new object[] { "leaveGame", (double)actorNumber, false, (double)actorNumber }, new RaiseEventOptions { TargetActors = new[] { actorNumber } }, SendOptions.SendReliable);

        public static void VStumpCrashGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    VStumpCrashPlayer(lockTarget.Creator.ActorNumber);
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
        public static void SwitchToTcp()
        {
            var loadBalancingPeer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
            loadBalancingPeer.TransportProtocol = ConnectionProtocol.Tcp;
            loadBalancingPeer.peerBase = new TPeer
            {
                photonPeer = loadBalancingPeer,
                usedTransportProtocol = ConnectionProtocol.Tcp,
                DoFraming = true
            };
        }

        public static void SwitchToUdp()
        {
            var loadBalancingPeer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
            loadBalancingPeer.TransportProtocol = ConnectionProtocol.Udp;
            loadBalancingPeer.peerBase = new EnetPeer
            {
                photonPeer = loadBalancingPeer,
                usedTransportProtocol = ConnectionProtocol.Udp
            };

            NetworkSystem.Instance.ReturnToSinglePlayer();
            Important.Reauth();
        }
        // Admin Mods
        public static float admindelay;
        public static void AdminKickAll() =>
            Admin.ExecuteCommand("kickall", ReceiverGroup.All);
        public static void AdminKickGun()
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
                        if (Time.time > admindelay)
                        {
                            admindelay = Time.time + 0.1f;
                            Admin.ExecuteCommand("kick", ReceiverGroup.All, GetNetPlayerFromVRRig(rig).UserId);
                        }
                    }
                }
            }
        }

        public static void AdminBringGun()
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
                        if (Time.time > admindelay)
                        {
                            admindelay = Time.time + 0.1f;
                            Admin.ExecuteCommand("bring", GetNetPlayerFromVRRig(rig).ActorNumber, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
                        }
                    }
                }
            }
        }

        public static void AdminBringAll()
        {
            if (Time.time > admindelay)
            {
                admindelay = Time.time + 0.05f;
                Admin.ExecuteCommand("bring", ReceiverGroup.Others, GorillaTagger.Instance.headCollider.transform.position + new Vector3(0f, 1.5f, 0f));
            }
        }

        public static void AdminLightningStrikeGun()
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
                        if (Time.time > admindelay)
                        {
                            admindelay = Time.time + 0.1f;
                            Admin.ExecuteCommand("lightningstrike", ReceiverGroup.All, NewPointer.transform.position);
                        }
                    }
                }
            }
        }

        public static void GetMenuUsers() // if i just take the seraylth code it will work -nova
        {
            Admin.indicatorDelay = Time.time + 2f;
            Admin.ExecuteCommand("isusing", ReceiverGroup.All);
        }
    }
}