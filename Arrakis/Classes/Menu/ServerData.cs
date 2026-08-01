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

        private static GameObject motdObj;
        private static GameObject motdTextObj;

        private static GameObject cocText;
        private static GameObject cocHeading;

        private static TextMeshPro motdTMP;
        private static TextMeshPro motdTextTMP;
        private static TextMeshPro cocTextTMP;
        private static TextMeshPro cocHeadingTMP;

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
            motdObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
            motdTextObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
            cocText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
            cocHeading = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText");

            motdTMP = motdObj?.GetComponent<TextMeshPro>();
            motdTextTMP = motdTextObj?.GetComponent<TextMeshPro>();
            cocTextTMP = cocText?.GetComponent<TextMeshPro>();
            cocHeadingTMP = cocHeading?.GetComponent<TextMeshPro>();
        }

        public void Update()
        {
            if (loaded || bypass)
            {
                if (bypass)
                {
                    if (!motdfall)
                    {
                        motd = $"THANK YOU FOR USING ARRAKIS, IF YOU SEE THIS THE SERVER DATA HAS NOT BEEN LOADED OR YOUR NOT ON WIFI.\nMENU VERSION: {PluginInfo.Version}, ALL SERVER ISSUES WILL BE FIXED ONCE NOVA HAS FIXED.\n\nTHIS SHOULD BE ONLY TEMP, MADE BY NOVA, SLEEPY\nJOIN DISCORD: discord.gg/dtQdz59FJG";
                        motdfall = true;
                    }
                }

                if (motdTMP == null || motdTextTMP == null || cocTextTMP == null || cocHeadingTMP == null)
                    return;
                if (!string.IsNullOrEmpty(motd))
                    motdTextTMP.text = string.Format(motd, PluginInfo.Version, Buttons.buttons.SelectMany(list => list).ToArray().Length);
                motdTMP.text = "ARRAKIS";
                cocHeadingTMP.text = "ARRAKIS";
                cocTextTMP.text = $"FPS: {(1f / Time.deltaTime):F0}\nPing: {PhotonNetwork.GetPing()}\nRegion: {(PhotonNetwork.CloudRegion ?? "N/A").Replace("/*", "")}\nConnected: {PhotonNetwork.IsConnected}\nIn Room: {PhotonNetwork.InRoom}\nRoom Name: {(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "N/A")}\nMaster Client: {PhotonNetwork.IsMasterClient}\nPlayer Count: {PhotonNetwork.CountOfPlayers}\nPlayer in Room Count: {PhotonNetwork.CountOfPlayersInRooms}\nRoom Count: {PhotonNetwork.CountOfRooms}\n\nName: {PhotonNetwork.LocalPlayer.NickName}\nUserid: {PhotonNetwork.LocalPlayer.UserId}\nArrakis Users: {UserCount.CurrentUsers}".ToUpper();

                if (!bypass)
                {
                    if (PluginInfo.Version != ServerData.serverversion)
                    {
                        NotificationManager.SendNotification("<color=cyan>[UPDATE]</color> Arrakis Needs a update please update.");
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