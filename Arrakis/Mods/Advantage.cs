/*
 * Arrakis | Mods/Advantage.cs
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

using System.Collections.Generic;
using System.Linq;
using Arrakis.Classes;
using Arrakis.Extensions;
using Arrakis.Notifications;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Arrakis.Menu.Main;

namespace Arrakis.Mods
{
    public class Advantage
    {
        public static void TagAll()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (GorillaTagManager tag in GameObject.FindObjectsByType<GorillaTagManager>(FindObjectsSortMode.None))
                {
                    foreach (NetPlayer plr in NetworkSystem.Instance.PlayerListOthers)
                    {
                        if (!tag.currentInfected.Contains(plr))
                                tag.AddInfectedPlayer(plr);
                    }
                }
            }
            else
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        TagPlayer(rig.Creator);
                    }
                }
            }
        }

        public static void TagSelf()
        {
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                foreach (GorillaTagManager tag in GameObject.FindObjectsByType<GorillaTagManager>(FindObjectsSortMode.None))
                {
                    if (!tag.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
                        tag.AddInfectedPlayer(NetworkSystem.Instance.LocalPlayer);
                }
            }
            else
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        TagPlayer(rig.Creator);
                    }
                }
            }
        }

        public static void TagGun()
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
                        TagPlayer(rig.Creator);
                    }
                }
                else
                    VRRig.LocalRig.enabled = true;
            }
        }

        public static void FlickTagGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    GTPlayer.Instance.RightHand.controllerTransform.position = NewPointer.transform.position;
                }
            }
        }

        private static GameObject hitboxleft;
        private static GameObject hitboxright;
        public static void Hitboxes()
        {
            if (hitboxleft == null)
            {
                hitboxleft = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitboxleft.transform.parent = GorillaTagger.Instance.leftHandTransform;
                hitboxleft.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                hitboxleft.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                hitboxleft.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                GameObject.Destroy(hitboxleft.GetComponent<SphereCollider>());
                hitboxleft.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                hitboxleft.GetComponent<Renderer>().material.color = new Color(0.5f, 0f, 0f, 0.5f);
            }
            if (hitboxright == null)
            {
                hitboxright = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitboxright.transform.parent = GorillaTagger.Instance.rightHandTransform;
                hitboxright.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                hitboxright.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                hitboxright.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                GameObject.Destroy(hitboxright.GetComponent<SphereCollider>());
                hitboxright.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
            }

            hitboxleft.GetComponent<Renderer>().material.color = new Color(Settings.backgroundColor.GetCurrentColor().r, Settings.backgroundColor.GetCurrentColor().g, Settings.backgroundColor.GetCurrentColor().b, 0.2f);
            hitboxright.GetComponent<Renderer>().material.color = new Color(Settings.backgroundColor.GetCurrentColor().r, Settings.backgroundColor.GetCurrentColor().g, Settings.backgroundColor.GetCurrentColor().b, 0.2f);
        }

        public static void DestroyHitboxes()
        {
            if (hitboxleft != null)
            {
                GameObject.Destroy(hitboxleft);
                hitboxleft = null;
            }
            if (hitboxright != null)
            {
                GameObject.Destroy(hitboxright);
                hitboxright = null;
            }
        }

        public static void NoTagOnJoin()
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("didTutorial", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
            PlayerPrefs.SetString("didTutorial", "");
            PlayerPrefs.Save();
        }

        public static void TagOnJoin()
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("didTutorial", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
            PlayerPrefs.SetString("didTutorial", "done");
            PlayerPrefs.Save();
        }
        public static void AntiTag()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                foreach (GorillaTagManager tag in GameObject.FindObjectsByType<GorillaTagManager>(FindObjectsSortMode.None))
                {
                    if (!tag.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
                    {
                        PhotonHandler handler = GameObject.Find("PhotonMono").GetComponent<PhotonHandler>();
                        Traverse.Create(handler).Field("nextSendTickCountOnSerialize").SetValue((int)(Time.realtimeSinceStartup * 9999));
                        List<int> Targets = new List<int>();
                        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
                        {
                            if (player.IsMasterClient)
                                continue;

                            Targets.Add(player.ActorNumber);
                        }
                        Experimental.SendSerialize(GorillaTagger.Instance.myVRRig.GetView, new RaiseEventOptions { TargetActors = Targets.ToArray() });
                    }
                }
            }
            else
            {
                if (VRRig.LocalRig.IsTagged() && !VRRig.LocalRig.IsTagged())
                {
                    foreach (VRRig rig in VRRigCache.ActiveRigs)
                    {
                        if (rig.IsTagged() && !VRRig.LocalRig.IsTagged())
                        {
                            if (Vector3.Distance(VRRig.LocalRig.transform.position, rig.transform.position) < 3f)
                            {
                                VRRig.LocalRig.enabled = false;
                                VRRig.LocalRig.transform.position = new Vector3(999f, 999f, 999f);
                            }
                            else
                                VRRig.LocalRig.enabled = true;
                        }
                    }
                }
            }
        }

        public static void TagPlayer(NetPlayer player)
        {
            VRRig[] target = VRRigCache.ActiveRigs.Where(x => x != null && x.Creator == player).ToArray();
            foreach (var rig in target)
            {
                if (!VRRig.LocalRig.IsTagged())
                {
                    NotificationManager.SendNotification("<color=grey>[</color><color=cyan>ARRAKIS</color><color=grey>]</color> You are not tagged.");
                    VRRig.LocalRig.enabled = true;
                    return;
                }
                if (VRRig.LocalRig.IsTagged() && !rig.IsTagged())
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = rig.transform.position;
                    GameMode.ReportTag(player);
                }
            }
        }
    }
}