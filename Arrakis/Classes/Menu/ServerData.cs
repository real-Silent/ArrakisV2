using Arrakis.Classes.Menu;
using Arrakis.Menu;
using Arrakis.Notifications;
using Meta.WitAi.Json;
using Photon.Pun;
using System.Collections;
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
                bypass = false;
                loaded = true;
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
                            motd = $"THANK YOU FOR USING ARRAKIS, IF YOU SEE THIS THE SERVER DATA HAS NOT BEEN LOADED OR YOUR NOT ON WIFI.\nMENU VERSION: {PluginInfo.Version}, ALL SERVER ISSUES WILL BE FIXED ONCE NOVA HAS FIXED.\n\nTHIS SHOULD BE ONLY TEMP, MADE BY NOVA, SLEEPY\nJOIN DISCORD: discord.gg/dtQdz59FJG";
                            motdfall = true;
                        }
                    }

                    if (motdTextTMP == null)
                        return;
                    if (!string.IsNullOrEmpty(motd))
                        motdTextTMP.text = string.Format(motd, PluginInfo.Version, Buttons.buttons.SelectMany(list => list).ToArray().Length);

                    if (!bypass)
                    {
                        if (PluginInfo.Version != ServerData.serverversion)
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
        }
    }
}