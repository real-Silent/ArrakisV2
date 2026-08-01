using Arrakis.Notifications;
using ExitGames.Client.Photon;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using static Arrakis.Menu.Main;

namespace Arrakis.Mods
{
    public class Important
    {
        public static void QuitGame() =>
            Application.Quit();

        public static void Disconnect() =>
            NetworkSystem.Instance.ReturnToSinglePlayer();

        public static void JoinRandomRoom() =>
            PhotonNetworkController.Instance.AttemptToJoinPublicRoom(PhotonNetworkController.Instance.currentJoinTrigger);

        public static void Reconnect()
        {
            if (NetworkSystem.Instance.InRoom)
                NetworkSystem.Instance.ReturnToSinglePlayer();
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(currentRoomName, JoinType.Solo);
        }

        public static void DisableWindBarriers()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(false); 
            GameObject.Find("Environment Objects/LocalObjects_Prefab/ForestToHoverboard/TurnOnInForestAndHoverboard/ForestDome_CollisionOnly").SetActive(false);
        }
        public static void EnableWindBarriers()
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/Forest_ForceVolumes/").SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/ForestToHoverboard/TurnOnInForestAndHoverboard/ForestDome_CollisionOnly").SetActive(true);
        }

        public static void ButtonClick()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Ray ray = TPC.ScreenPointToRay(Mouse.current.position.ReadValue());
                Physics.Raycast(ray, out var Ray, 512f, NoInvisLayerMask());
                GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>().enabled = false;
                GorillaTagger.Instance.rightHandTriggerCollider.transform.position = Ray.point;
            }
        }

        public static void ConnectToRegion(string region) =>
            PhotonNetwork.ConnectToRegion(region);

        public static void Reauth() =>
            MothershipAuthenticator.Instance.BeginLoginFlow();

        public static void JoinRoom(string roomCode) =>
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomCode, JoinType.Solo);

        public static void DisableAFKKick() =>
            PhotonNetworkController.Instance.disableAFKKick = true;
        public static void EnsableAFKKick() =>
            PhotonNetworkController.Instance.disableAFKKick = false;

        public static void FirstPerson()
        {
            TPC.GetComponent<Camera>().fieldOfView = 90f;
            TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = false;
        }
        public static void DisableFirstPerson()
        {
            TPC.GetComponent<Camera>().fieldOfView = 60f;
            TPC.gameObject.transform.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().enabled = true;
        }
        public static void UnlockComp() =>
            GorillaComputer.instance.CompQueueUnlockButtonPress();
        public static void ChangeQueue(string queue) =>
            GorillaComputer.instance.currentQueue = queue;
        public static void ChangeGamemode(string gamemode) =>
            GorillaComputer.instance.SetGameModeWithoutButton(gamemode);
        public static void CLearNotis() =>
            NotificationManager.ClearAllNotifications();

        public static void BuyBarrel() =>
            CosmeticsController.instance.currentCart.Insert(0, CosmeticsController.instance.GetItemFromDict("LMAPE."));

        public static async void CreatePublicLobby(string roomName)
        {
            GorillaNetworkJoinTrigger trigger = PhotonNetworkController.Instance.currentJoinTrigger;

            if (trigger == null || trigger.networkZone == "private")
            {
                float closest = float.MaxValue;

                foreach (GorillaNetworkJoinTrigger joinTrigger in Object.FindObjectsByType<GorillaNetworkJoinTrigger>(FindObjectsSortMode.None))
                {
                    float distance = Vector3.Distance(VRRig.LocalRig.transform.position, joinTrigger.transform.position);

                    if (distance < closest)
                    {
                        closest = distance;
                        trigger = joinTrigger;
                    }
                }
            }

            RoomConfig config = new RoomConfig
            {
                createIfMissing = true,
                isJoinable = true,
                isPublic = true,
                MaxPlayers = 10
            };

            config.CustomProps = new Hashtable
            {
                { "gameMode", trigger.GetFullDesiredGameModeString() },
                { "platform", Traverse.Create(PhotonNetworkController.Instance).Field("platformTag").GetValue<string>() },
                { "queueName", GorillaComputer.instance.currentQueue },
                { "language", LocalisationManager.CurrentLanguage.ToString() },
                { "fan_club", SubscriptionManager.IsLocalSubscribed().ToString().ToLower() }
            };

            await NetworkSystem.Instance.ConnectToRoom(roomName, config, -1);
        }
        public static string GenerateRandomString(int a = 4)
        {
            string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            string roomName = "";
            for (int i = 0; i < a; i++)
                roomName += chars[UnityEngine.Random.Range(0, chars.Length)];
            return roomName;
        }
    }
}