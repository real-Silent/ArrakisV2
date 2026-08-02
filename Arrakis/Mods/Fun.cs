using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using Liv.Lck.GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Voxels;
using static Arrakis.Menu.Main;

namespace Arrakis.Mods
{
    public class Fun
    {
        public static async void UnlockAllCosmetics() // Fixed by sleepy bcz nova dosnt know how to make cosmeticx
        {
            await Task.Delay(500);
            var cosmetics = CosmeticsController.instance;
            if (cosmetics == null) return;
            foreach (var item in cosmetics.allCosmetics)
            {
                if (!item.isNullItem && !cosmetics.unlockedCosmetics.Contains(item))
                {
                    cosmetics.unlockedCosmetics.Add(item);
                    switch (item.itemCategory)
                    {
                        case CosmeticsController.CosmeticCategory.Hat:
                            if (!cosmetics.unlockedHats.Contains(item))
                                cosmetics.unlockedHats.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Face:
                            if (!cosmetics.unlockedFaces.Contains(item))
                                cosmetics.unlockedFaces.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Badge:
                            if (!cosmetics.unlockedBadges.Contains(item))
                                cosmetics.unlockedBadges.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Paw:
                            if (!item.isThrowable)
                            {
                                if (!cosmetics.unlockedPaws.Contains(item))
                                    cosmetics.unlockedPaws.Add(item);
                            }
                            else
                            {
                                if (!cosmetics.unlockedThrowables.Contains(item))
                                    cosmetics.unlockedThrowables.Add(item);
                            }
                            break;
                        case CosmeticsController.CosmeticCategory.Fur:
                            if (!cosmetics.unlockedFurs.Contains(item))
                                cosmetics.unlockedFurs.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Shirt:
                            if (!cosmetics.unlockedShirts.Contains(item))
                                cosmetics.unlockedShirts.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Back:
                            if (!cosmetics.unlockedBacks.Contains(item))
                                cosmetics.unlockedBacks.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Arms:
                            if (!cosmetics.unlockedArms.Contains(item))
                                cosmetics.unlockedArms.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Chest:
                            if (!cosmetics.unlockedChests.Contains(item))
                                cosmetics.unlockedChests.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.Pants:
                            if (!cosmetics.unlockedPants.Contains(item))
                                cosmetics.unlockedPants.Add(item);
                            break;
                        case CosmeticsController.CosmeticCategory.TagEffect:
                            if (!cosmetics.unlockedTagFX.Contains(item))
                                cosmetics.unlockedTagFX.Add(item);
                            break;
                    }
                }
            }
            cosmetics.UpdateWardrobeModelsAndButtons();
            cosmetics.OnCosmeticsUpdated?.Invoke();
        }

        public static void FixHead()
        {
            VRRig.LocalRig.head.trackingRotationOffset.x = 0f;
            VRRig.LocalRig.head.trackingRotationOffset.y = 0f;
            VRRig.LocalRig.head.trackingRotationOffset.z = 0f;
        }
        public static void UpsidedownHead() =>
            VRRig.LocalRig.head.trackingRotationOffset.z = 180f;

        public static void GrabBug()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                GameObject.Find("Floating Bug Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
                GameObject.Find("Floating Bug Holdable").transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                GameObject.Find("Floating Bug Holdable").transform.position = GorillaTagger.Instance.leftHandTransform.position;
                GameObject.Find("Floating Bug Holdable").transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            }
        }
        public static void GrabBat()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                GameObject.Find("Cave Bat Holdable").transform.position = GorillaTagger.Instance.rightHandTransform.position;
                GameObject.Find("Cave Bat Holdable").transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                GameObject.Find("Cave Bat Holdable").transform.position = GorillaTagger.Instance.leftHandTransform.position;
                GameObject.Find("Cave Bat Holdable").transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            }
        }

        private static void SpawnWater(Vector3 pos, Quaternion rot, float scale, float radius, bool big, bool enter) =>
            GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", Photon.Pun.RpcTarget.All, new object[] { pos, rot, scale, radius, big, enter });

        public static void WaterSplashSelf()
        {
            if (ControllerInputPoller.instance.rightGrab)
                SpawnWater(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 5f, 100f, true, false);
            if (ControllerInputPoller.instance.leftGrab)
                SpawnWater(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.rotation, 5f, 100f, true, false);
        }

        public static void WaterGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = NewPointer.transform.position;
                    SpawnWater(VRRig.LocalRig.transform.position, VRRig.LocalRig.transform.rotation, 5f, 100f, true, false);
                }
                else
                {
                    VRRig.LocalRig.enabled = true;
                }
            }
        }

        public static void MaxQuestScore() =>
            VRRig.LocalRig.SetQuestScore(int.MaxValue);

        public static void OpenBasementDoor() =>
            GameObject.Find("Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab").GetComponent<PhotonView>().RPC("ChangeDoorState", RpcTarget.AllViaServer, GTDoor.DoorState.Opening);
        public static void CloseBasementDoor() =>
            GameObject.Find("Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab").GetComponent<PhotonView>().RPC("ChangeDoorState", RpcTarget.AllViaServer, GTDoor.DoorState.Closing);
        public static void OpenElevatorDoor() =>
            GRElevatorManager.ElevatorButtonPressed(GRElevator.ButtonType.Open, GRElevatorManager._instance.currentLocation);
        public static void CloseElevatorDoor() =>
            GRElevatorManager.ElevatorButtonPressed(GRElevator.ButtonType.Close, GRElevatorManager._instance.currentLocation);


        public static void HoldGlider()
        {
            foreach (GliderHoldable glider in GetGliders())
            {
                if (ControllerInputPoller.instance.rightGrab)
                {
                    if (!glider.IsMine)
                        glider.OnHover(null, null);
                    else
                        glider.gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                }
                if (ControllerInputPoller.instance.leftGrab)
                {
                    if (!glider.IsMine)
                        glider.OnHover(null, null);
                    else
                        glider.gameObject.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                }
            }
        }

        public static void GliderGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    foreach (GliderHoldable glider in GetGliders())
                    {
                        if (!glider.IsMine)
                            glider.OnHover(null, null);
                        else
                            glider.gameObject.transform.position = NewPointer.transform.position;
                    }
                }
            }
        }

        public static void SpawnHoverboard()
        {
            FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.rightHandTransform.transform.position, Quaternion.identity, Vector3.zero, Vector3.zero, VRRig.LocalRig.playerColor);
            Safety.RPCProc();
        }
        public static void SpawnHoverboardSpam()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.rightHandTransform.transform.position, Quaternion.identity, Vector3.zero, Vector3.zero, VRRig.LocalRig.playerColor);
                Safety.RPCProc();
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                FreeHoverboardManager.instance.SendDropBoardRPC(GorillaTagger.Instance.leftHandTransform.transform.position, Quaternion.identity, Vector3.zero, Vector3.zero, VRRig.LocalRig.playerColor);
                Safety.RPCProc();
            }
        }

        public static void BoardGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = NewPointer.transform.position;
                    FreeHoverboardManager.instance.SendDropBoardRPC(NewPointer.transform.position, Quaternion.identity, Vector3.zero, Vector3.zero, VRRig.LocalRig.playerColor);
                    Safety.RPCProc();
                }
                else
                {
                    VRRig.LocalRig.enabled = true;
                }
            }
        }
        public static void GiveAllResources()
        {
            foreach (SIResource.ResourceType type in Enum.GetValues(typeof(SIResource.ResourceType)))
            {
                SIProgression.Instance.resourceDict[type] = 999999;
            }
            SIPlayer.SetAndBroadcastProgression();
        }
        public static void SIUnlockAll()
        {
            foreach (bool[] gadget in SIProgression.Instance.unlockedTechTreeData)
                Array.Fill(gadget, true);
        }
        public static float flashTimer = 0f;

        public static void FlashVIMNameTag()
        {
            if (Time.time > flashTimer)
            {
                flashTimer = Time.time + 0.1f;

                if (VRRig.LocalRig.ShowGoldNameTag)
                {
                    VRRig.LocalRig.ShowGoldNameTag = false;
                    VRRig.LocalRig.playerText1.color = Color.white;
                }
                else
                {
                    VRRig.LocalRig.ShowGoldNameTag = true;
                    VRRig.LocalRig.playerText1.color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
                }
            }
        }
        private static List<TransferrableObject> cachedHoldables = new List<TransferrableObject>();
        private static float lastCacheTime = 0f;
        private static float cacheInterval = 0.5f;

        private static void RHC() // Refreshs holdables cache so game dosnt kill its self
        {
            if (Time.time - lastCacheTime > cacheInterval)
            {
                cachedHoldables.Clear();
                var found = Resources.FindObjectsOfTypeAll<TransferrableObject>();
                foreach (var obj in found)
                {
                    if (obj != null)
                    {
                        cachedHoldables.Add(obj);
                    }
                }
                lastCacheTime = Time.time;
            }
        }
        public static void StickyHoldables()
        {
            try
            {
                foreach (TransferrableObject tobj in GetHoldables())
                {
                    if (tobj.IsMyItem())
                    {
                        if (tobj.InRightHand())
                        {
                            tobj.currentState = TransferrableObject.PositionState.InRightHand;
                            tobj.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
                        }
                        if (tobj.InLeftHand())
                        {
                            tobj.currentState = TransferrableObject.PositionState.InLeftHand;
                            tobj.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
                        }
                    }
                }
            }
            catch { }
        }
        public static void SpinHoldables()
        {
            try
            {
                RHC();
                foreach (var holdable in cachedHoldables)
                {
                    try
                    {
                        if (holdable == null || holdable.transform == null)
                            continue;

                        if (holdable.currentState == TransferrableObject.PositionState.InLeftHand || holdable.currentState == TransferrableObject.PositionState.InRightHand)
                        {
                            holdable.transform.rotation = RandomQuaternion(360);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
        private static float nextJuggleTime = 0f;
        private static float juggleInterval = 0.3f;
        private static int positionIndex = 0;
        private static readonly TransferrableObject.PositionState[] allPositions = new TransferrableObject.PositionState[]
        {
            TransferrableObject.PositionState.InLeftHand,
            TransferrableObject.PositionState.InRightHand,
            TransferrableObject.PositionState.OnLeftArm,
            TransferrableObject.PositionState.OnRightArm,
            TransferrableObject.PositionState.OnLeftShoulder,
            TransferrableObject.PositionState.OnRightShoulder,
            TransferrableObject.PositionState.OnChest,
            TransferrableObject.PositionState.Dropped
        };
        public static void JuggleHoldables()
        {
            try
            {
                if (Time.time < nextJuggleTime) return;
                nextJuggleTime = Time.time + juggleInterval;
                RHC();
                positionIndex = (positionIndex + 1) % allPositions.Length;
                foreach (var holdable in cachedHoldables)
                {
                    try
                    {
                        if (holdable == null || !holdable.gameObject.activeInHierarchy) continue;
                        if (holdable.currentState != TransferrableObject.PositionState.None)
                        {
                            holdable.currentState = allPositions[positionIndex];
                            if (allPositions[positionIndex] == TransferrableObject.PositionState.InLeftHand && holdable.canAutoGrabLeft)
                            {
                                holdable.OnGrab(holdable.gripInteractor, EquipmentInteractor.instance.leftHand);
                            }
                            else if (allPositions[positionIndex] == TransferrableObject.PositionState.InRightHand && holdable.canAutoGrabRight)
                            {
                                holdable.OnGrab(holdable.gripInteractor, EquipmentInteractor.instance.rightHand);
                            }
                            else if (allPositions[positionIndex] == TransferrableObject.PositionState.Dropped)
                            {
                                holdable.DropItem();
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
        public static void OrbitHoldables()
        {
            try
            {
                if (cachedHoldables == null || cachedHoldables.Count == 0)
                    RHC();
                int count = cachedHoldables.Count;
                float time = Time.time * 2f;
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        var holdable = cachedHoldables[i];
                        if (holdable == null || !holdable.gameObject.activeInHierarchy)
                            continue;
                        float angle = (360f / count) * i + time * Mathf.Rad2Deg;
                        float rad = angle * Mathf.Deg2Rad;
                        Vector3 offset = new Vector3(Mathf.Cos(rad) * 1.2f, 0.2f, Mathf.Sin(rad) * 1.2f);
                        holdable.DropItem();
                        holdable.transform.position = VRRig.LocalRig.transform.position + offset;
                        holdable.transform.rotation = Quaternion.LookRotation((holdable.transform.position - VRRig.LocalRig.transform.position).normalized, Vector3.up );
                    }
                    catch { }
                }
            }
            catch { }
        }

        public static IEnumerator ProcessCosmetics(int mode = 0) // prolly coded like shitg but it functions so :p -sleepy | sleepys hell hole of code fixed -nova
        {
            bool inCity = GameObject.Find("City_Pretty") != null;
            bool inMountain = GameObject.Find("Mountain") != null;

            if (!inCity && !inMountain)
            {
                yield break;
            }
            Vector3 targetPosition = inCity ? new Vector3(-52f, 17.50f, -120f) : new Vector3(-14.62f, 18.11f, -111.72f);
            if (!VRRig.LocalRig.inTryOnRoom)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = targetPosition;
            }
            CosmeticsController cosmetics = CosmeticsController.instance;
            string cosmeticId = (mode == -1 || mode == 2) ? "NOTHING" : "LBAAI.";
            string[] cosmeticArray = Enumerable.Repeat(cosmeticId, 16).ToArray();
            CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(cosmeticArray, cosmetics);
            cosmetics.currentWornSet = cosmeticSet;
            VRRig.LocalRig.cosmeticSet = cosmeticSet;
            if (!PhotonNetwork.InRoom)
            {
                VRRig.LocalRig.LocalUpdateCosmeticsWithTryon(cosmeticSet, cosmetics.tryOnSet, false);
            }
            else
            {
                GorillaTagger.Instance.myVRRig.SendRPC( "RPC_UpdateCosmeticsWithTryonPacked", RpcTarget.All, new object[] { cosmeticSet.ToPackedIDArray(), cosmetics.tryOnSet.ToPackedIDArray(), false });
            }
            yield return new WaitForSeconds(0.1f);
            if (mode == -1)
            {
                VRRig.LocalRig.enabled = true;
                yield break;
            }
            if (mode != 2)
            {
                VRRig.LocalRig.enabled = false;
                Vector3 abovePosition = targetPosition + Vector3.up * 7f;
                VRRig.LocalRig.transform.position = abovePosition;
                yield return new WaitForSeconds(0.1f);
                VRRig.LocalRig.transform.position = targetPosition;
                yield return new WaitForSeconds(0.1f);
                VRRig.LocalRig.transform.position = abovePosition;
                yield return new WaitForSeconds(0.1f);
                VRRig.LocalRig.transform.position = targetPosition;
            }

            if (mode > 0)
            {
                List<CosmeticsController.CosmeticItem> cosmeticsList = new List<CosmeticsController.CosmeticItem>();
                if (mode == 1 || mode == 2)
                {
                    cosmeticsList = cosmetics.allCosmetics.Where(item => item.canTryOn && (int)item.itemCategory != 3 && !item.isHoldable && !item.isThrowable && (int)item.itemCategory != 6 && (int)item.itemCategory != 2 && (int)item.itemCategory != 11).ToList();
                }
                else if (mode == 3)
                {
                   cosmeticsList = cosmetics.allCosmetics.Where(item => item.canTryOn && item.isHoldable).ToList();
                }
                foreach (var item in cosmeticsList)
                {
                    cosmetics.ApplyCosmeticItemToSet(cosmetics.tryOnSet, item, false, false);
                    cosmetics.UpdateWornCosmetics(true);
                    yield return new WaitForSeconds(0.03f);
                    if (mode != 2)
                    {
                        cosmetics.ApplyCosmeticItemToSet(cosmetics.tryOnSet, item, false, false);
                        cosmetics.UpdateWornCosmetics(true);
                    }
                }
            }
            VRRig.LocalRig.enabled = true;
        }

        private static int chi = 0;
        private static float hatTryOnDelay = 0f;
        public static void SpazTryonHats()
        {
            if (Time.time > hatTryOnDelay)
            {
                List<CosmeticsController.CosmeticItem> hats = CosmeticsController.instance.allCosmetics.Where(x => x.itemCategory == CosmeticsController.CosmeticCategory.Hat && x.canTryOn).ToList();
                CosmeticsController.CosmeticItem item = hats[chi];
                foreach (FittingRoomButton button in GameObject.FindObjectsByType<FittingRoomButton>(FindObjectsSortMode.None))
                {
                    button.currentCosmeticItem = item;
                    button.ButtonActivationWithHand(false);
                }
                chi = (chi + 1) % hats.Count;
                hatTryOnDelay = Time.time + 0.5f;
            }
        }

        public static void SpazTryonBadges()
        {
            if (Time.time > hatTryOnDelay)
            {
                List<CosmeticsController.CosmeticItem> hats = CosmeticsController.instance.allCosmetics.Where(x => x.itemCategory == CosmeticsController.CosmeticCategory.Badge && x.canTryOn).ToList();
                CosmeticsController.CosmeticItem item = hats[chi];
                foreach (FittingRoomButton button in GameObject.FindObjectsByType<FittingRoomButton>(FindObjectsSortMode.None))
                {
                    button.currentCosmeticItem = item;
                    button.ButtonActivationWithHand(false);
                }
                chi = (chi + 1) % hats.Count;
                hatTryOnDelay = Time.time + 0.5f;
            }
        }

        public static void SpazTryonFace()
        {
            if (Time.time > hatTryOnDelay)
            {
                List<CosmeticsController.CosmeticItem> hats = CosmeticsController.instance.allCosmetics.Where(x => x.itemCategory == CosmeticsController.CosmeticCategory.Face && x.canTryOn).ToList();
                CosmeticsController.CosmeticItem item = hats[chi];
                foreach (FittingRoomButton button in GameObject.FindObjectsByType<FittingRoomButton>(FindObjectsSortMode.None))
                {
                    button.currentCosmeticItem = item;
                    button.ButtonActivationWithHand(false);
                }
                chi = (chi + 1) % hats.Count;
                hatTryOnDelay = Time.time + 0.5f;
            }
        }

        public static void SpazTryonHoldables()
        {
            if (Time.time > hatTryOnDelay)
            {
                List<CosmeticsController.CosmeticItem> hats = CosmeticsController.instance.allCosmetics.Where(x => x.itemCategory == CosmeticsController.CosmeticCategory.Arms && x.canTryOn).ToList();
                CosmeticsController.CosmeticItem item = hats[chi];
                foreach (FittingRoomButton button in GameObject.FindObjectsByType<FittingRoomButton>(FindObjectsSortMode.None))
                {
                    button.currentCosmeticItem = item;
                    button.ButtonActivationWithHand(false);
                }
                chi = (chi + 1) % hats.Count;
                hatTryOnDelay = Time.time + 0.5f;
            }
        }

        public static void UnlockSubscription() // By sleepy
        {
            if (SubscriptionManager.Instance == null) return;
            Type subscriptionDetailsType = typeof(SubscriptionManager.SubscriptionDetails);
            object details = Activator.CreateInstance(subscriptionDetailsType);
            subscriptionDetailsType.GetField("active").SetValue(details, true);
            subscriptionDetailsType.GetField("daysAccrued").SetValue(details, int.MaxValue);
            subscriptionDetailsType.GetField("tier").SetValue(details, int.MaxValue);
            subscriptionDetailsType.GetField("autoRenew").SetValue(details, true);
            subscriptionDetailsType.GetField("autoRenewMonths").SetValue(details, int.MaxValue);
            subscriptionDetailsType.GetField("subscriptionActiveUntilDate").SetValue(details, DateTime.MaxValue);
            FieldInfo subscriptionFeatureSettingsField = subscriptionDetailsType.GetField("subscriptionFeatureSettings");
            if (subscriptionFeatureSettingsField != null)
            {
                subscriptionFeatureSettingsField.SetValue(details, new[] { true, true });
            }
            typeof(SubscriptionManager).GetField("localSubscriptionDetails", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, details);
            typeof(SubscriptionManager).GetField("_localSubscriptionDataInitialized", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, true);
            if (NetworkSystem.Instance != null && NetworkSystem.Instance.LocalPlayer != null)
            {
                MethodInfo updateMethod = typeof(SubscriptionManager).GetMethod("UpdatePlayerSubsDetails", BindingFlags.NonPublic | BindingFlags.Instance);
                if (updateMethod != null)
                {
                    updateMethod.Invoke(SubscriptionManager.Instance, new object[] { NetworkSystem.Instance.LocalPlayer, true, int.MaxValue });
                }
            }
        }

        private static List<SocialCoconutCamera> coconutCameras = new List<SocialCoconutCamera>();
        private static SocialCoconutCamera coconutCamera;
        private static float cameraDelay = 0f;

        private static void FindCoconutCamera() // coconut :3 -sleepy | better -nova
        {
            coconutCameras.RemoveAll(x => x == null);
            foreach (SocialCoconutCamera cam in GameObject.FindObjectsByType<SocialCoconutCamera>(FindObjectsSortMode.None))
            {
                if (!coconutCameras.Contains(cam))
                    coconutCameras.Add(cam);
            }
            coconutCamera = coconutCameras.FirstOrDefault();
        }
        public static void GrabCamera()
        {
            FindCoconutCamera();
            if (coconutCamera == null) return;

            if (ControllerInputPoller.instance.rightGrab)
            {
                coconutCamera.SetVisualsActive(true);
                coconutCamera.SetRecordingState(true);
                coconutCamera.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                coconutCamera.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                coconutCamera.transform.SetParent(GorillaTagger.Instance.rightHandTransform);
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                coconutCamera.SetVisualsActive(true);
                coconutCamera.SetRecordingState(true);
                coconutCamera.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                coconutCamera.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                coconutCamera.transform.SetParent(GorillaTagger.Instance.leftHandTransform);
            }
        }
        public static void OrbitCamera()
        {
            FindCoconutCamera();
            if (coconutCamera == null) 
                return;
            coconutCamera.SetVisualsActive(true);
            coconutCamera.SetRecordingState(true);
            float angle = Time.time * 10f;
            Vector3 orbitPos = GTPlayer.Instance.transform.position +
                new Vector3(Mathf.Cos(angle) * 2f, 1f, Mathf.Sin(angle) * 2f);
            coconutCamera.transform.position = orbitPos;
            coconutCamera.transform.LookAt(GTPlayer.Instance.transform.position);
        }
        public static void DestroyCamera()
        {
            FindCoconutCamera();
            if (coconutCamera == null) 
                return;
            coconutCamera.SetVisualsActive(false);
            coconutCamera.SetRecordingState(false);
            coconutCamera.transform.position = new Vector3(999f, 999f, 999f);
            coconutCamera.transform.SetParent(null);
        }
        public static void FlashCameraRecording()
        {
            FindCoconutCamera();
            if (coconutCamera == null) 
                return;
            if (Time.time > cameraDelay)
            {
                cameraDelay = Time.time + 0.5f;
                var isRecordingField = typeof(SocialCoconutCamera).GetField("_isActive", BindingFlags.NonPublic | BindingFlags.Instance);
                if (isRecordingField != null)
                {
                    bool current = (bool)isRecordingField.GetValue(coconutCamera);
                    coconutCamera.SetRecordingState(!current);
                    coconutCamera.SetVisualsActive(true);
                }
            }
        }

        public static void GrabTablet()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                LckSocialCamera camera = LckSocialCameraManager.Instance._networkedTablet;
                if (camera != null)
                {
                    camera.visible = true;
                    camera.recording = true;
                    camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                    camera.m_CameraVisuals.SetRecordingState(true);
                    camera.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    camera.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    camera.transform.SetParent(GorillaTagger.Instance.rightHandTransform);
                }
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                LckSocialCamera camera = LckSocialCameraManager.Instance._networkedTablet;
                if (camera != null)
                {
                    camera.visible = true;
                    camera.recording = true;
                    camera.m_CameraVisuals.SetNetworkedVisualsActive(true);
                    camera.m_CameraVisuals.SetRecordingState(true);
                    camera.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    camera.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                    camera.transform.SetParent(GorillaTagger.Instance.leftHandTransform);
                }
            }
        }
        public static void OrbitTablet()
        {
            LckSocialCamera tablet = LckSocialCameraManager.Instance._networkedTablet;
            if (tablet != null)
            {
                tablet.visible = true;
                tablet.recording = true;
                tablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
                tablet.m_CameraVisuals.SetRecordingState(true);
                float angle = Time.time * 10f;
                Vector3 orbitPos = GTPlayer.Instance.transform.position +
                new Vector3(Mathf.Cos(angle) * 2f, 1f, Mathf.Sin(angle) * 2f);
                tablet.transform.position = orbitPos;
                tablet.transform.LookAt(GTPlayer.Instance.transform.position);
            }
        }

        public static void DestroyTablet()
        {
            LckSocialCamera tablet = LckSocialCameraManager.Instance._networkedTablet;
            if (tablet != null)
            {
                tablet.visible = false;
                tablet.TurnOff();
                GameObject.Destroy(tablet);
            }
        }
        public static void VIMDimGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (GetGunInput(true))
                {
                    ChunkComponent c = Ray.collider.GetComponent<ChunkComponent>();
                    if (c != null)
                    {
                        VoxelWorld world = c.World;
                        VoxelAction action = default;
                        action.strength = Settings.digsize;
						action.radius = Settings.digsize;
                        action.operation = 0;
                        VoxelExtensions.Mine(world, Ray, action);
                    }
                }
            }
        }

        public static void ForestSnowGround(bool growing) =>
            GameObject.Find("pit ground bottom").GetComponent<GorillaSurfaceOverride>().overrideIndex = !growing ? 32 : 339;
        public static void DisableForestSnowGround() => 
            GameObject.Find("pit ground bottom").GetComponent<GorillaSurfaceOverride>().overrideIndex = 7;

        public static void RandomColorSnowballs(bool enabled)
        {
			foreach (SnowballThrowable snowballThrowable in GameObject.FindObjectsByType<SnowballThrowable>(FindObjectsSortMode.None))
			{
				snowballThrowable.randomizeColor = enabled;
                snowballThrowable.ApplyColor(UnityEngine.Random.ColorHSV());
			}
        }

        public static void BraceletToggle(bool enable, bool Lefthand)
        {
            if (!PhotonNetwork.InRoom) 
                return;
            GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", RpcTarget.All, enable, Lefthand);
            Safety.RPCProc();
        }
        public static void InvisableRig(bool invisible, GorillaBodyType bodyType = GorillaBodyType.Invisible) // Might have been patched to cs :<<< -sleepy 
        {
            var player = NetworkSystem.Instance.LocalPlayer;
            var actor = player.ActorNumber;
            GhostReactor.instance.grManager.GetView.RPC("PlayerStateChangeRPC", RpcTarget.All, new object[] { actor, actor, GRPlayer.GRPlayerState.Ghost });
            var localRig = VRRig.LocalRig;
            localRig.bodyRenderer.SetGameModeBodyType( invisible ? bodyType : GorillaBodyType.Default);
            localRig.SetInvisibleToLocalPlayer(invisible);
        }

        public static void RainbowMonkey()
        {
            Vector3 color = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            CosmeticsController.instance.UpdateMonkeColor(color, false);
        }
        public static void MonkeyBlocksSizeChanger()
        {
            if (ControllerInputPoller.instance.rightControllerTriggerButton || Keyboard.current.lKey.isPressed)
                VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;
            if (ControllerInputPoller.instance.leftControllerTriggerButton || Keyboard.current.pKey.isPressed)
                VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 2;
        }
        public static void MultiBlock()
        {
            BuilderPieceInteractor.instance.handState[1] = BuilderPieceInteractor.HandState.Empty;
            BuilderPieceInteractor.instance.heldPiece[1] = null;
        }
    }
}