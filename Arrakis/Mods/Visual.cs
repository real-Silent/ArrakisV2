using Arrakis.Classes;
using Arrakis.Extensions;
using Arrakis.Menu;
using GorillaExtensions;
using Pathfinding.RVO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Arrakis.Settings;

namespace Arrakis.Mods
{
    public class Visual
    {
        private static bool BehindStuff(VRRig rig)
        {
            if (VRRig.LocalRig == null || rig == null) return false;
            Vector3 local = VRRig.LocalRig.transform.position + Vector3.up * 0.5f;
            Vector3[] points = new Vector3[]
            {
                rig.transform.position + Vector3.up * 0.5f, rig.transform.position + Vector3.up * 1.2f, rig.transform.position,
                rig.transform.position + Vector3.up * 0.5f + rig.transform.right * 0.3f, rig.transform.position + Vector3.up * 0.5f - rig.transform.right * 0.3f
            };
            int layerMask = 1 << LayerMask.NameToLayer("Gorilla Object");
            int count = 0;
            foreach (Vector3 target in points)
            {
                float distance = Vector3.Distance(local, target);
                if (distance < 0.5f) continue;
                Vector3 direction = (target - local).normalized;
                RaycastHit hit;
                if (Physics.Raycast(local, direction, out hit, distance, layerMask))
                    count++;
            }
            return count >= 3;
        }

        public static void Chams(int type)
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        switch (type)
                        {
                            case 0:
                                rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                                rig.mainSkin.material.color = followmenutheme ? backgroundColor.GetCurrentColor() : (rig.IsTagged() || rig.mainSkin.material.name.ToLower().Contains("it")) ? new Color(0.6f, 0f, 0f, 0.6f) : new Color(0.46f, 0.6f, 0.6f, 0.6f);
                                break;
                            case 1:
                                if (BehindStuff(rig))
                                {
                                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                                    rig.mainSkin.material.color = followmenutheme ? backgroundColor.GetCurrentColor() : (rig.IsTagged() ? new Color(0.6f, 0f, 0f, 0.6f) : new Color(0.46f, 0.6f, 0.6f, 0.6f));
                                }
                                else
                                {
                                    rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                                    rig.mainSkin.material.color = rig.playerColor;
                                }
                                break;
                        }
                    }
                }
            }
        }
        public static void DisableChams()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                        rig.mainSkin.material.color = rig.playerColor;
                    }
                }
            }
        }

        private static Dictionary<VRRig, GameObject> tracersPool = new Dictionary<VRRig, GameObject>();
        public static void Tracers()
        {
            if (!NetworkSystem.Instance.InRoom) // if it doesnt work the first time just return -nova
                return;
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig != null && rig != VRRig.LocalRig)
                {
                    List<VRRig> remove = null;
                    foreach (var pair in tracersPool)
                    {
                        if (pair.Key == null || !VRRigCache.ActiveRigs.Contains(pair.Key))
                        {
                            remove ??= new List<VRRig>();
                            remove.Add(pair.Key);
                            if (pair.Value != null)
                                Object.Destroy(pair.Value);
                        }
                    }
                    if (remove != null)
                    {
                        foreach (var vrrig in remove)
                            tracersPool.Remove(vrrig);
                    }
                    if (!tracersPool.TryGetValue(rig, out GameObject holder))
                    {
                        holder = new GameObject();
                        holder.transform.parent = rig.transform; // stupid fix -nova
                        LineRenderer line = holder.AddComponent<LineRenderer>();
                        line.useWorldSpace = true;
                        line.material = new Material(Shader.Find("GUI/Text Shader"));
                        line.positionCount = 2;
                        line.startWidth = 0.02f;
                        line.endWidth = 0.02f;
                        tracersPool[rig] = holder;
                    }
                    LineRenderer lr = holder.GetComponent<LineRenderer>();
                    Color color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.IsTagged() ? new Color(0.6f, 0f, 0f, 0.6f) : new Color(0.46f, 0.6f, 0.6f, 0.6f);
                    lr.startColor = color;
                    lr.endColor = color;
                    lr.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
                    lr.SetPosition(1, rig.transform.position);
                }
            }
        }
        public static void DisableTracers()
        {
            foreach (var obj in tracersPool.Values)
            {
                if (obj != null)
                    Object.Destroy(obj);
            }
            tracersPool.Clear();
        }

        private static readonly Dictionary<VRRig, GameObject> boxEspPool = new Dictionary<VRRig, GameObject>();
        private static GameObject CreateObject(Transform parent, PrimitiveType type, Vector3 scale, Color color, Shader shader)
        {
            GameObject obj = GameObject.CreatePrimitive(type);

            obj.transform.SetParent(parent, false);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = scale;
            if (obj.TryGetComponent(out Collider col))
                Object.Destroy(col);
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material.shader = shader;
            renderer.material.color = color;
            return obj;
        }

        public static void BoxESP()
        {
            if (!NetworkSystem.Instance.InRoom)
                return;
            List<VRRig> remove = null;
            foreach (var pair in boxEspPool)
            {
                if (pair.Key == null || !VRRigCache.ActiveRigs.Contains(pair.Key))
                {
                    remove ??= new List<VRRig>();
                    remove.Add(pair.Key);
                    if (pair.Value != null)
                        Object.Destroy(pair.Value);
                }
            }
            if (remove != null)
            {
                foreach (var rig in remove)
                    boxEspPool.Remove(rig);
            }
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig == null || rig == VRRig.LocalRig)
                    continue;
                if (!boxEspPool.TryGetValue(rig, out GameObject box))
                {
                    box = CreateObject(rig.transform, PrimitiveType.Cube, new Vector3(0.5f, 0.5f, 0.1f), followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, Shader.Find("GUI/Text Shader"));
                    boxEspPool[rig] = box;
                }
                Color color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                color.a = 0.3f;
                box.GetComponent<Renderer>().material.color = color;
                float distance = Vector3.Distance(Camera.main.transform.position, rig.transform.position);
                float scale = Mathf.Clamp(distance * 0.05f, 0.25f, 3f);
                box.transform.localScale = new Vector3(scale, scale, 0.02f);
                box.transform.LookAt(Camera.main.transform);
            }
        }

        public static void DisableBoxESP()
        {
            foreach (var obj in boxEspPool.Values)
            {
                if (obj != null)
                    Object.Destroy(obj);
            }
            boxEspPool.Clear();
        }


        private static Dictionary<VRRig, TextMeshPro> nameTagPool = new Dictionary<VRRig, TextMeshPro>();
        public static void NameTags()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var rig in nameTagPool.Keys.Where(r => r == null || !VRRigCache.ActiveRigs.Contains(r)).ToList())
                {
                    if (nameTagPool[rig] != null)
                        GameObject.Destroy(nameTagPool[rig].gameObject);
                    nameTagPool.Remove(rig);
                }
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        if (!nameTagPool.TryGetValue(rig, out var tag))
                        {
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, rig.Creator.NickName, followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 0);
                            nameTagPool.Add(rig, tag);
                        }
                        tag.text = rig.Creator.NickName;
                        tag.color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                        tag.transform.LookAt(Camera.main.transform);
                        tag.transform.Rotate(0f, 180f, 0f);
                    }
                }
            }
        }
        public static void DisableNameTags()
        {
            foreach (var tag in nameTagPool.Values)
                GameObject.Destroy(tag.gameObject);
            nameTagPool.Clear();
        }


        private static Dictionary<VRRig, TextMeshPro> IDnameTagPool = new Dictionary<VRRig, TextMeshPro>();
        public static void IdNameTags()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var rig in IDnameTagPool.Keys.Where(r => r == null || !VRRigCache.ActiveRigs.Contains(r)).ToList())
                {
                    if (IDnameTagPool[rig] != null)
                        GameObject.Destroy(IDnameTagPool[rig].gameObject);
                    IDnameTagPool.Remove(rig);
                }
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        if (!IDnameTagPool.TryGetValue(rig, out var tag))
                        {
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, rig.Creator.UserId, followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 1);
                            IDnameTagPool.Add(rig, tag);
                        }
                        tag.color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                        tag.transform.LookAt(Camera.main.transform);
                        tag.transform.Rotate(0f, 180f, 0f);
                    }
                }
            }
        }
        public static void DisableIdNameTags()
        {
            foreach (var tag in IDnameTagPool.Values)
                GameObject.Destroy(tag.gameObject);
            IDnameTagPool.Clear();
        }

        private static Dictionary<VRRig, TextMeshPro> PlatformnameTagPool = new Dictionary<VRRig, TextMeshPro>();
        public static void PlatformNameTags()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var rig in PlatformnameTagPool.Keys.Where(r => r == null || !VRRigCache.ActiveRigs.Contains(r)).ToList())
                {
                    if (PlatformnameTagPool[rig] != null)
                        GameObject.Destroy(PlatformnameTagPool[rig].gameObject);
                    PlatformnameTagPool.Remove(rig);
                }
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        string platform = rig.Cosmetics().Contains("S. FIRST LOGIN") ? "Steam" : "Quest";
                        if (!PlatformnameTagPool.TryGetValue(rig, out var tag))
                        {
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, platform, followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 2);
                            PlatformnameTagPool.Add(rig, tag);
                        }
                        tag.color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                        tag.transform.LookAt(Camera.main.transform);
                        tag.transform.Rotate(0f, 180f, 0f);
                    }
                }
            }
        }
        public static void DisablePlatformNameTags()
        {
            foreach (var tag in PlatformnameTagPool.Values)
                GameObject.Destroy(tag.gameObject);
            PlatformnameTagPool.Clear();
        }
        private static Dictionary<VRRig, TextMeshPro> FpsnametagPool = new Dictionary<VRRig, TextMeshPro>();
        public static void FpsNameTags()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var rig in FpsnametagPool.Keys.Where(r => r == null || !VRRigCache.ActiveRigs.Contains(r)).ToList())
                {
                    if (FpsnametagPool[rig] != null)
                        GameObject.Destroy(FpsnametagPool[rig].gameObject);
                    FpsnametagPool.Remove(rig);
                }
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        if (!FpsnametagPool.TryGetValue(rig, out var tag))
                        {
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, "72", followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 4);
                            FpsnametagPool.Add(rig, tag);
                        }
                        if (rig.fps < 30)
                        {
                            tag.text = $"<color=red>{rig.fps}</color>";
                        }
                        if (rig.fps > 30 && rig.fps < 60)
                        {
                            tag.text = $"<color=yellow>{rig.fps}</color>";
                        }
                        if (rig.fps > 60)
                        {
                            tag.text = $"<color=green>{rig.fps}</color>";
                        }
                        if (rig.fps > 100)
                        {
                            tag.text = $"<color=#00ffff>{rig.fps}</color>";
                        }
                        tag.color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                        tag.transform.LookAt(Camera.main.transform);
                        tag.transform.Rotate(0f, 180f, 0f);
                    }
                }
            }
        }
        public static void DisableFpsNameTags()
        {
            foreach (var tag in FpsnametagPool.Values)
                GameObject.Destroy(tag.gameObject);
            FpsnametagPool.Clear();
        }

        private static Dictionary<VRRig, TextMeshPro> TaggednametagPool = new Dictionary<VRRig, TextMeshPro>();
        public static void TaggedNameTags()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var rig in TaggednametagPool.Keys.Where(r => r == null || !VRRigCache.ActiveRigs.Contains(r)).ToList())
                {
                    if (TaggednametagPool[rig] != null)
                        GameObject.Destroy(TaggednametagPool[rig].gameObject);
                    TaggednametagPool.Remove(rig);
                }
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        if (!TaggednametagPool.TryGetValue(rig, out var tag))
                        {
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, "Not Tagged", followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 5);
                            TaggednametagPool.Add(rig, tag);
                        }
                        tag.text = rig.IsTagged() ? "Tagged" : "Not Tagged";
                        tag.color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                        tag.transform.LookAt(Camera.main.transform);
                        tag.transform.Rotate(0f, 180f, 0f);
                    }
                }
            }
        }
        public static void DisableTaggedNameTags()
        {
            foreach (var tag in TaggednametagPool.Values)
                GameObject.Destroy(tag.gameObject);
            TaggednametagPool.Clear();
        }

        private static Dictionary<VRRig, TextMeshPro> GrabTagsPool = new Dictionary<VRRig, TextMeshPro>();
        public static void GrabTags()
        {
            if (NetworkSystem.Instance.InRoom)
            {
                foreach (var rig in GrabTagsPool.Keys.Where(r => r == null || !VRRigCache.ActiveRigs.Contains(r)).ToList())
                {
                    if (GrabTagsPool[rig] != null)
                        GameObject.Destroy(GrabTagsPool[rig].gameObject);
                    GrabTagsPool.Remove(rig);
                }
                foreach (VRRig rig in VRRigCache.ActiveRigs)
                {
                    if (rig != null && rig != VRRig.LocalRig)
                    {
                        string grabbing = "";
                        if (Overpowered.CheckHandLinks(rig))
                        {
                            grabbing = "Grabbing";
                        }
                        else { grabbing = ""; };

                        if (!GrabTagsPool.TryGetValue(rig, out var tag))
                        {
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, grabbing, followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 3);
                            GrabTagsPool.Add(rig, tag);
                        }
                        tag.color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                        tag.transform.LookAt(Camera.main.transform);
                        tag.transform.Rotate(0f, 180f, 0f);
                    }
                }
            }
        }
        public static void DisableGrabTags()
        {
            foreach (var tag in GrabTagsPool.Values)
                GameObject.Destroy(tag.gameObject);
            GrabTagsPool.Clear();
        }

        private static TextMeshPro CreateText(Transform parent, TextAlignmentOptions alignment, string text, Color color, float scale, int index = 0)
        {
            GameObject textHolder = new GameObject("nametag_arrakis");
            TextMeshPro textmesh = textHolder.GetOrAddComponent<TextMeshPro>();
            textmesh.text = text;
            textmesh.color = color;
            textmesh.alignment = alignment;
            textmesh.fontSize = scale;
            textmesh.transform.SetParent(parent, false);
            textmesh.transform.localPosition = new Vector3(0f, 1f - (0.1f * index), 0f);
            return textmesh;
        }

        static readonly List<GameObject> allLeaves = new List<GameObject>();
        public static void NoLeaves()
        {
            GameObject Forest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest");
            if (Forest != null)
            {
                for (int i = 0; i < Forest.transform.childCount; i++)
                {
                    GameObject v = Forest.transform.GetChild(i).gameObject;
                    if (v.name.Contains(LeavesName))
                    {
                        v.SetActive(false);
                        allLeaves.Add(v);
                    }
                }
            }
        }
        public static void DisableNoLeaves()
        {
            foreach (GameObject l in allLeaves)
                l.SetActive(true);
            allLeaves.Clear();
        }

        public static string _leavesName;
        public static string LeavesName
        {
            get
            {
                if (_leavesName == null)
                {
                    var forest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest");
                    _leavesName = forest.GetComponentsInChildren<Transform>(true)
                    .Where(t => t.name.StartsWith("UnityTempFile") && t.parent != null && t.parent == forest.transform)
                    .GroupBy(t => t.name).Where(g => g.Count() == 3)
                    .OrderByDescending(g => g.First().GetSiblingIndex()).FirstOrDefault()?.Key ?? "UnityTempFile";
                }
                return _leavesName;
            }
        }

        public static void ClearWeather()
        {
            for (int i = 0; i < BetterDayNightManager.instance.weatherCycle.Length; i++)
                BetterDayNightManager.instance.weatherCycle[i] = BetterDayNightManager.WeatherType.None;
        }

        public static void Snowfall(bool toggle) =>
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/snow/").SetActive(toggle);
        public static void Rain(bool toggle) =>
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight/rain/").SetActive(toggle);

        private static GameObject arraylistHub; // Did you know that ccmv2 was actually the best cheat menu -nova
        private static Text arraylistText; // Did you know that ccmv2 was actually the best cheat menu -nova
        private static Material mat = new Material(Shader.Find("GUI/Text Shader"));
        public static (GameObject, Text) CreateTextGUI(string text, string name, TextAnchor alignment, Vector3 loctrans) // Credits colossus
        {
            GameObject HUDObj = new GameObject(name);
            Canvas canvas = HUDObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            HUDObj.AddComponent<CanvasScaler>().dynamicPixelsPerUnit = 20f;
            HUDObj.AddComponent<GraphicRaycaster>();
            RectTransform rectTransform = HUDObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(5, 5);
            HUDObj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
            GameObject menuTextObj = new GameObject();
            menuTextObj.transform.SetParent(HUDObj.transform);
            Text MenuText = menuTextObj.AddComponent<Text>();
            MenuText.text = text;
            MenuText.fontSize = 10;
            MenuText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            MenuText.rectTransform.sizeDelta = new Vector2(260, 180);
            MenuText.rectTransform.localScale = new Vector3(0.01f, 0.01f, 1f);
            MenuText.rectTransform.localPosition = loctrans;
            MenuText.material = mat;
            MenuText.alignment = alignment;
            HUDObj.transform.SetParent(Camera.main.transform);
            HUDObj.transform.position = Camera.main.transform.position;
            HUDObj.transform.rotation = Camera.main.transform.rotation;
            return (HUDObj, MenuText);
        }
        public static void Arraylist()
        {
            if (arraylistHub == null)
            {
                (arraylistHub, arraylistText) = CreateTextGUI("", "ArraylistHud", TextAnchor.UpperLeft, new Vector3(0f, 0.2f, 3.6f));
            }
            if (arraylistText != null)
                arraylistText.alignment = fliparraylist ? TextAnchor.UpperRight : TextAnchor.UpperLeft;
            StringBuilder sb = new StringBuilder();

            foreach (ButtonInfo[] category in Buttons.buttons)
            {
                foreach (ButtonInfo button in category)
                {
                    if (button != null && button.enabled && button.ShowInArraylist)
                        sb.AppendLine(button.buttonText);
                }
            }
            arraylistText.text = sb.ToString().TrimEnd();
        }
        public static void DisableArraylist()
        {
            GameObject.Destroy(arraylistHub);
            arraylistHub = null;
            GameObject.Destroy(arraylistText);
            arraylistText = null;
        }
    }
}