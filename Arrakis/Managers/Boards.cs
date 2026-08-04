using Arrakis.Classes.Menu;
using Arrakis.Extensions;
using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Arrakis.Managers
{
    public class Boards : MonoBehaviour
    {
        private GameObject motdObj;
        private static TextMeshPro motdTMP;
        private GameObject cocText;
        private GameObject cocHeading;
        private TextMeshPro cocTextTMP;
        private TextMeshPro cocHeadingTMP;
        private GameObject monitorObj;
        private MeshRenderer boardRenders;
        private Material boardmat;
        private string ogcocText;
        private Material ogboardmat;

        public void Awake()
        {
            boardmat = new Material(Shader.Find("GorillaTag/UberShader"));
            boardmat.color = Settings.backgroundColor.GetCurrentColor();
        }

        public void Start()
        {
            motdObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
            cocText = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
            cocHeading = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText");
            motdTMP = motdObj?.GetComponent<TextMeshPro>();
            cocTextTMP = cocText?.GetComponent<TextMeshPro>();
            cocHeadingTMP = cocHeading?.GetComponent<TextMeshPro>();
            boardRenders = boards.GetComponent<MeshRenderer>();
            ogboardmat = boardRenders.material;

            ogcocText = cocTextTMP.text;

            monitorObj = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
        }

        public void Update()
        {
            if (cocHeadingTMP != null && cocTextTMP != null && motdTMP != null)
            {
                if (Settings.disablecustomboards) 
                {
                    cocTextTMP.text = ogcocText;
                    cocHeadingTMP.text = "GORILLA CODE OF CONDUCT";
                    motdTMP.text = "MESSAGE OF THE DAY";
                }
                else
                {
                    motdTMP.text = "ARRAKIS";
                    cocHeadingTMP.text = "ARRAKIS";
                    cocTextTMP.text = $"FPS: {(1f / Time.deltaTime):F0}\nPing: {PhotonNetwork.GetPing()}\nRegion: {(PhotonNetwork.CloudRegion ?? "N/A").Replace("/*", "")}\nConnected: {PhotonNetwork.IsConnected}\nIn Room: {PhotonNetwork.InRoom}\nRoom Name: {(PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "N/A")}\nMaster Client: {PhotonNetwork.IsMasterClient}\nPlayer Count: {PhotonNetwork.CountOfPlayers}\nPlayer in Room Count: {PhotonNetwork.CountOfPlayersInRooms}\nRoom Count: {PhotonNetwork.CountOfRooms}\n\nName: {PhotonNetwork.LocalPlayer.NickName}\nUserid: {PhotonNetwork.LocalPlayer.UserId}\nArrakis Users: {UserCount.CurrentUsers}".ToUpper();
                }
            }
            boardmat.color = Settings.backgroundColor.GetCurrentColor();
            if (monitorObj != null && boardRenders != null)
            {
                if (Settings.disablecustomboards)
                {
                    monitorObj.GetComponent<MeshRenderer>().material = ogboardmat;
                    boardRenders.material = ogboardmat;
                }
                else
                {
                    monitorObj.GetComponent<MeshRenderer>().material = boardmat;
                    boardRenders.material = boardmat;
                }
            }
        }

        public static GameObject _boards;
        public static GameObject boards
        {
            get
            {
                if (_boards == null)
                {
                    var stumpChildren = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.Children().Where(x => x.name.Contains("UnityTempFile")).ToList();
                    if (3 >= 0 && 3 < stumpChildren.Count)
                    {
                        var stumpBoard = stumpChildren[3];
                        if (stumpBoard != null)
                            _boards = stumpBoard;
                    }
                }
                return _boards;
            }
        }
    }
}