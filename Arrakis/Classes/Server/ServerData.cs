/*
 * Arrakis | Classes/Server/ServerData.cs
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

using Arrakis.Menu;
using Arrakis.Notifications;
using GorillaNetworking;
using Meta.WitAi.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Arrakis.Classes
{
    public class ServerData : MonoBehaviour
    {
        public static bool lockdown = false;
        public static string serverversion;
        public static string motd;
        public static string menustate;
        public static string[] detectedmods;

        private static GameObject motdTextObj;
        private static TextMeshPro motdTextTMP;

        private static bool loaded = false;
        private static bool bypass = false;
        private static bool motdfall = false;

        public void Awake()
        {
            CustomConsole.Log("Getting server data.", CustomConsole.LogType.Info);
            StartCoroutine(GetServerData());
        }

        private IEnumerator GetServerData()
        {
            using (UnityWebRequest webRequest = new UnityWebRequest($"{PluginInfo.ServerApi}/serverdata", "GET"))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return webRequest.SendWebRequest();
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    CustomConsole.Log("Error getting server data loading boards.", CustomConsole.LogType.Error);
                    bypass = true;
                    yield break; 
                }
                ServerDataResponse data = JsonConvert.DeserializeObject<ServerDataResponse>(webRequest.downloadHandler.text);
                CustomConsole.Log("Got server data.", CustomConsole.LogType.Info);
                lockdown = data.lockdown;
                menustate = data.menustate;
                motd = data.motd;
                serverversion = data.serverversion;
                detectedmods = data.detectedmods;

                CustomConsole.Log($"Got detected mods {detectedmods.Length}", CustomConsole.LogType.Debug);
                if (detectedmods.Length > 0)
                    StartCoroutine(SetDetectedMods());

                bypass = false;
                loaded = true;
            }
        }

        private List<string> allDetected = new List<string>();
        private IEnumerator SetDetectedMods()
        {
            if (detectedmods == null || detectedmods.Length == 0)
                yield break;
            while (GorillaComputer.instance == null || !GorillaComputer.instance.isConnectedToMaster)
                yield return null;
            yield return new WaitForSeconds(2f);
            foreach (string name in detectedmods)
            {
                if (string.IsNullOrWhiteSpace(name) || allDetected.Contains(name))
                    continue;
                ButtonInfo button = Main.GetIndex(name);
                if (button != null)
                {
                    string overlap = string.IsNullOrEmpty(button.overlapText) ? button.buttonText : button.overlapText;
                    button.detected = true;
                    button.overlapText = overlap + " <color=red>[DETECTED]</color>";
                    button.isTogglable = false;
                    button.method = () => NotificationManager.SendNotification("<color=cyan>[ARRAKIS]</color> This mod is <color=red>detected</color>.");
                    button.enableMethod = button.method;
                    button.disableMethod = button.method;
                }
                allDetected.Add(name);
            }
        }

        public void Start()
        {
            motdTextObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
            motdTextTMP = motdTextObj?.GetComponent<TextMeshPro>();
        }

        public void Update()
        {
            if (loaded || bypass)
            {
                if (Settings.disablecustomboards)
                {
                    if (motdTextTMP != null)
                        motdTextTMP.text = motdTextTMP.gameObject.GetComponent<PlayFabTitleDataTextDisplay>()._cachedText;
                }
                else 
                {
                    if (bypass)
                    {
                        if (!motdfall)
                        {
                            motd = $"THANK YOU FOR USING ARRAKIS, IF YOU SEE THIS THE SERVER DATA HAS NOT BEEN LOADED OR YOUR NOT ON WIFI.\nMENU VERSION: {PluginInfo.Version}, ALL SERVER ISSUES WILL BE FIXED ONCE NOVA HAS FIXED.\n\nTHIS SHOULD BE ONLY TEMP, MADE BY NOVA, SLEEPY\nJOIN DISCORD: {PluginInfo.DiscordLink}";
                            motdfall = true;
                        }
                    }

                    if (motdTextTMP == null)
                        return;
                    if (!string.IsNullOrEmpty(motd))
                        motdTextTMP.text = string.Format(motd, PluginInfo.Version, Buttons.buttons.SelectMany(list => list).ToArray().Length);

                    if (!bypass)
                    {
                        if (PluginInfo.Version != serverversion)
                        {
                            NotificationManager.SendNotification("<color=cyan>[UPDATE]</color> Arrakis Needs a update please update.");
                        }
                    }
                }
            }
        }

        public class ServerDataResponse
        {
            public bool lockdown;
            public string menustate;
            public string motd;
            public string serverversion;
            public string[] detectedmods;
        }
    }
}