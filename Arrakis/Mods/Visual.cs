/*
 * Arrakis | Mods/Visual.cs
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

using Arrakis.Classes;
using Arrakis.Classes.Mods;
using Arrakis.Extensions;
using Arrakis.Menu;
using GorillaExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using static Arrakis.Menu.Main;
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
                                rig.mainSkin.material.color = followmenutheme ? backgroundColor.GetCurrentColor() : (rig.IsTagged() || 
                                    rig.mainSkin.material.name.ToLower().Contains("it")) ? new Color(0.6f, 0f, 0f, 0.6f) : new Color(0.46f, 0.6f, 0.6f, 0.6f);
                                break;
                            case 1:
                                if (BehindStuff(rig))
                                {
                                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                                    rig.mainSkin.material.color = followmenutheme ? backgroundColor.GetCurrentColor() : 
                                        (rig.IsTagged() ? new Color(0.6f, 0f, 0f, 0.6f) : new Color(0.46f, 0.6f, 0.6f, 0.6f));
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
            if (!NetworkSystem.Instance.InRoom)
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
                        holder.transform.parent = rig.transform;
                        LineRenderer line = holder.AddComponent<LineRenderer>();
                        line.useWorldSpace = true;
                        line.material = new Material(Shader.Find("GUI/Text Shader"));
                        line.positionCount = 2;
                        line.startWidth = 0.02f;
                        line.endWidth = 0.02f;
                        tracersPool[rig] = holder;
                    }
                    LineRenderer lr = holder.GetComponent<LineRenderer>();
                    Color color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.IsTagged() ? new Color(0.6f, 0f, 0f) : rig.playerColor;
                    color.a = 0.7f;
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


        private static Dictionary<VRRig, List<LineRenderer>> boneEspPool = new Dictionary<VRRig, List<LineRenderer>>();
        private static Dictionary<VRRig, List<LineRenderer>> selfBoneEspPool = new Dictionary<VRRig, List<LineRenderer>>();
        private static readonly int[] bones = {
            4, 3, 5, 4, 19, 18, 20, 19, 3, 18, 21, 20, 22, 21, 25, 21, 29, 21, 31, 29, 27, 25, 24, 22, 6, 5, 7, 6, 10, 6, 14, 6, 16, 14, 12, 10, 9, 7
        };
        public static void BoneESP()
        {
            if (!NetworkSystem.Instance.InRoom)
                return;
            List<VRRig> remove = new List<VRRig>();
            foreach (var bone in boneEspPool.Where(x => !VRRigCache.ActiveRigs.Contains(x.Key)))
            {
                remove.Add(bone.Key);
                foreach (LineRenderer line in bone.Value)
                    GameObject.Destroy(line);
            }
            foreach (VRRig rig in remove)
                boneEspPool.Remove(rig);
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                if (rig != null && rig != VRRig.LocalRig)
                {
                    if (!boneEspPool.TryGetValue(rig, out List<LineRenderer> lines))
                    {
                        lines = new List<LineRenderer>();
                        LineRenderer head = rig.head.rigTarget.AddComponent<LineRenderer>();
                        head.material.shader = Shader.Find("GUI/Text Shader");
                        lines.Add(head);
                        for (int i = 0; i < 19; i++)
                        {
                            LineRenderer line = rig.mainSkin.bones[bones[i * 2]].gameObject.AddComponent<LineRenderer>();
                            line.material.shader = Shader.Find("GUI/Text Shader");
                            lines.Add(line);
                        }
                        boneEspPool.Add(rig, lines);
                    }
                    LineRenderer linee = lines[0];
                    Color boneColor = followmenutheme ? backgroundColor.GetCurrentColor() : rig.IsTagged() ? new Color(0.6f, 0f, 0f) : rig.playerColor;
                    boneColor.a = 0.9f;
                    linee.startWidth = 0.02f;
                    linee.endWidth = 0.02f;
                    linee.startColor = boneColor;
                    linee.endColor = boneColor;
                    linee.SetPosition(0, rig.head.rigTarget.transform.position + new Vector3(0f, 0.16f, 0f));
                    linee.SetPosition(1, rig.head.rigTarget.transform.position - new Vector3(0f, 0.4f, 0f));
                    for (int i = 0; i < 19; i++)
                    {
                        linee = lines[i + 1];
                        linee.startWidth = 0.02f;
                        linee.endWidth = 0.02f;
                        linee.startColor = boneColor;
                        linee.endColor = boneColor;
                        linee.material.shader = Shader.Find("GUI/Text Shader");
                        linee.SetPosition(0, rig.mainSkin.bones[bones[i * 2]].position);
                        linee.SetPosition(1, rig.mainSkin.bones[bones[i * 2 + 1]].position);
                    }
                }
            }
        }
        public static void DisableBoneESP()
        {
            foreach (var line in boneEspPool.SelectMany(bones => bones.Value))
                Object.Destroy(line);
            boneEspPool.Clear();
        }

        public static void SelfBoneESP()
        {
            if (!selfBoneEspPool.TryGetValue(GorillaTagger.Instance.offlineVRRig, out List<LineRenderer> lines))
            {
                lines = new List<LineRenderer>();
                LineRenderer head = GorillaTagger.Instance.offlineVRRig.head.rigTarget.AddComponent<LineRenderer>();
                head.material.shader = Shader.Find("GUI/Text Shader");
                lines.Add(head);
                for (int i = 0; i < 19; i++)
                {
                    LineRenderer line = GorillaTagger.Instance.offlineVRRig.mainSkin.bones[bones[i * 2]].gameObject.AddComponent<LineRenderer>();
                    line.material.shader = Shader.Find("GUI/Text Shader");
                    lines.Add(line);
                }
                selfBoneEspPool.Add(GorillaTagger.Instance.offlineVRRig, lines);
            }
            LineRenderer linee = lines[0];
            Color boneColor = followmenutheme ? backgroundColor.GetCurrentColor() : GorillaTagger.Instance.offlineVRRig.playerColor;
            boneColor.a = 0.7f;
            linee.startWidth = 0.02f;
            linee.endWidth = 0.02f;
            linee.startColor = boneColor;
            linee.endColor = boneColor;
            linee.SetPosition(0, GorillaTagger.Instance.offlineVRRig.headMesh.transform.position);
            linee.SetPosition(1, GorillaTagger.Instance.offlineVRRig.headMesh.transform.position);
            for (int i = 0; i < 19; i++)
            {
                linee = lines[i + 1];
                linee.startWidth = 0.02f;
                linee.endWidth = 0.02f;
                linee.startColor = boneColor;
                linee.endColor = boneColor;
                linee.material.shader = Shader.Find("GUI/Text Shader");
                linee.SetPosition(0, GorillaTagger.Instance.offlineVRRig.mainSkin.bones[bones[i * 2]].position);
                linee.SetPosition(1, GorillaTagger.Instance.offlineVRRig.mainSkin.bones[bones[i * 2 + 1]].position);
            }
        }
        public static void DisableSelfBoneESP()
        {
            foreach (var line in selfBoneEspPool.SelectMany(bones => bones.Value))
                Object.Destroy(line);
            selfBoneEspPool.Clear();
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
                    box = CreateObject(rig.transform, PrimitiveType.Cube, new Vector3(0.5f, 0.5f, 0.1f), 
                        followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, Shader.Find("GUI/Text Shader"));
                    boxEspPool[rig] = box;
                }
                Color color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor;
                color.a = 0.4f;
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

        private static Dictionary<VRRig, GameObject> hollowBoxPool = new Dictionary<VRRig, GameObject>();
        public static void HollowBoxESP()
        {
            if (!NetworkSystem.Instance.InRoom)
                return;
            foreach (VRRig rig in VRRigCache.ActiveRigs)
            {
                List<VRRig> remove = null;
                foreach (var pair in hollowBoxPool)
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
                        hollowBoxPool.Remove(vrrig);
                }
                if (rig != null && rig != VRRig.LocalRig)
                {
                    if (!hollowBoxPool.TryGetValue(rig, out GameObject box))
                    {
                        box = new GameObject("HollowBoxESP");
                        TextMesh text = box.AddComponent<TextMesh>();
                        text.alignment = TextAlignment.Center;
                        text.anchor = TextAnchor.MiddleCenter;
                        text.text = "□";
                        text.fontSize = 300;
                        text.characterSize = 0.05f;
                        hollowBoxPool[rig] = box;
                    }
                    Color color = followmenutheme ? backgroundColor.GetCurrentColor() : rig.IsTagged() ? Color.red : rig.playerColor;
                    color.a = 0.7f;
                    box.GetComponent<TextMesh>().color = color;
                    box.transform.position = rig.transform.position;
                    if (Camera.main != null)
                    {
                        box.transform.LookAt(box.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                    }
                }
            }
        }
        public static void DisableHollowBoxESP()
        {
            foreach (var obj in hollowBoxPool.Values)
            {
                if (obj != null)
                    Object.Destroy(obj);
            }
            hollowBoxPool.Clear();
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
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, rig.Creator.NickName, 
                                followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 0);
                            nameTagPool.Add(rig, tag);
                        }
                        tag.text = rig.Creator.NickName;
                        Transform parent = followheadmesh ? rig.headMesh.transform : rig.transform;
                        if (tag.transform.parent != parent)
                            tag.transform.SetParent(parent, false);
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
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, rig.Creator.UserId, 
                                followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 1);
                            IDnameTagPool.Add(rig, tag);
                        }
                        Transform parent = followheadmesh ? rig.headMesh.transform : rig.transform;
                        if (tag.transform.parent != parent)
                            tag.transform.SetParent(parent, false);
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
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, platform, 
                                followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 2);
                            PlatformnameTagPool.Add(rig, tag);
                        }
                        Transform parent = followheadmesh ? rig.headMesh.transform : rig.transform;
                        if (tag.transform.parent != parent)
                            tag.transform.SetParent(parent, false);
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
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, "72", 
                                followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 4);
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
                        Transform parent = followheadmesh ? rig.headMesh.transform : rig.transform;
                        if (tag.transform.parent != parent)
                            tag.transform.SetParent(parent, false);
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
                            tag = CreateText(followheadmesh ? rig.headMesh.transform : rig.transform, TextAlignmentOptions.Center, "Not Tagged", 
                                followmenutheme ? backgroundColor.GetCurrentColor() : rig.playerColor, 0.7f, 5);
                            TaggednametagPool.Add(rig, tag);
                        }
                        Transform parent = followheadmesh ? rig.headMesh.transform : rig.transform;
                        if (tag.transform.parent != parent)
                            tag.transform.SetParent(parent, false);
                        if (GorillaGameManager.instance.GameModeName() != "CASUAL")
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
        public static GameObject ACam;
        public static void SpectateGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                RaycastHit Ray = GunData.Ray;
                if (lockTarget != null && gunLocked)
                {
                    if (ACam == null)
                    {
                        ACam = new GameObject("Arrakis Camera");
                        var c = ACam.AddComponent<Camera>();
                        c.fieldOfView = 120;
                        c.depth = 4;
                        c.nearClipPlane = 0.1f;
                        c.cameraType = CameraType.Game;
                        ACam.transform.position = GorillaTagger.Instance.offlineVRRig.headConstraint.transform.position;
                        Object.DontDestroyOnLoad(ACam);
                    }
                    ACam.transform.rotation = lockTarget.head.rigTarget.rotation;
                    ACam.transform.position = lockTarget.head.rigTarget.position;
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
                Object.Destroy(ACam);
            }
        }
        public static float EmissionSpeed = 1f;
        public static TrailRenderer leftTrail;
        public static TrailRenderer rightTrail;
        public static Vector3 lastLeftPosition;
        public static Vector3 lastRightPosition;
        public static void EnableHandTrails()
        {
            Transform leftHand = GorillaTagger.Instance.leftHandTransform;
            Transform rightHand = GorillaTagger.Instance.rightHandTransform;
            if (leftHand != null && leftTrail == null)
            {
                leftTrail = CreateHandTrail(leftHand);
            }
            if (rightHand != null && rightTrail == null)
            {
                rightTrail = CreateHandTrail(rightHand);
            }
            lastLeftPosition = leftHand != null ? leftHand.position : Vector3.zero;
            lastRightPosition = rightHand != null ? rightHand.position : Vector3.zero;
        }
        private static Shader _espShader; // c# is fucking retarded -sleepy
        public static Shader EspShader // c# is fucking retarded -sleepy
        {
            get
            {
                if (_espShader == null)
                {
                    _espShader = Shader.Find("GUI/Text Shader");
                    if (_espShader == null)
                    {
                        _espShader = Shader.Find("Sprites/Default");
                    }

                    if (_espShader == null)
                    {
                        _espShader = Cached;
                    }
                }

                return _espShader;
            }
        }
        public static Shader Cached { get; private set; } // c# is fucking retarded -sleepy
        public static Material CreateTransparentMaterial(Color color) // c# is fucking retarded -sleepy
        {
            Material material = new Material(EspShader)
            {
                color = color
            };
            material.SetInt("_ZTest", 8);
            material.SetFloat("_ZWrite", 0f);
            material.renderQueue = 4000;
            return material;
        }
        private static TrailRenderer CreateHandTrail(Transform parent)
        {
            GameObject trailObject = new GameObject("Arrakis_HandTrail");
            trailObject.transform.SetParent(parent, false);
            trailObject.transform.localPosition = Vector3.zero;
            TrailRenderer trail = trailObject.AddComponent<TrailRenderer>();
            trail.time = 0.3f;
            trail.startWidth = 0.05f;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.01f;
            Gradient gradient = new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(new Color(1f, 0.5f, 0f), 0.5f),
                    new GradientColorKey(new Color(1f, 0.3f, 0f), 1f)
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.5f, 0.5f),
                    new GradientAlphaKey(0f, 1f)
                }
            };
            trail.colorGradient = gradient;
            ((Renderer)trail).material = CreateTransparentMaterial(Color.white);
            trail.emitting = false;
            return trail;
        }
        public static void HandTrails()
        {
            Transform leftHand = GorillaTagger.Instance.leftHandTransform;
            Transform rightHand = GorillaTagger.Instance.rightHandTransform;
            float frameTime = Mathf.Max(Time.deltaTime, 0.0001f);
            if (leftHand != null && leftTrail != null)
            {
                leftTrail.emitting = (leftHand.position - lastLeftPosition).magnitude / frameTime > EmissionSpeed;
                lastLeftPosition = leftHand.position;
            }
            if (rightHand != null && rightTrail != null)
            {
                rightTrail.emitting = (rightHand.position - lastRightPosition).magnitude / frameTime > EmissionSpeed;
                lastRightPosition = rightHand.position;
		    }
        }
        public static void DestroyHandTrails()
        {
            DestroyHandTrail(leftTrail);
            DestroyHandTrail(rightTrail);
        }

        public static void DestroyHandTrail(TrailRenderer trail)
        {
            if (trail == null)
            {
                return;
            }
            Object.Destroy(trail.gameObject);
            trail = null;
        }
        private static GameObject hudInstance; // pasted RIGHT from my plugin so code might be ass -sleepy
        public static void PlayerInfo()
        {
            if (hudInstance == null)
            {
                hudInstance = new GameObject("Arrakis_InfoHud"); // only changed this name bcz before it was the fslur 25 times :3 -sleepy
                hudInstance.AddComponent<PlayerInfoBehavior>();
            }
        }
        public static void CleanupPlayerInfo() // pasted RIGHT from my plugin so code might be ass -sleepy
        {
            if (hudInstance != null)
            {
                UnityEngine.Object.Destroy(hudInstance);
                hudInstance = null;
            }
        }

        public static void EntityESP(ThrowableBug.BugName bugType)
        {
            ThrowableBug[] bugs = GetThrowableBugs();
            if (bugs == null)
                return;
            foreach (ThrowableBug entity in bugs)
            {
                if (entity == null || entity.bugName != bugType)
                    continue;
                SkinnedMeshRenderer rend = entity.GetComponentInChildren<SkinnedMeshRenderer>();
                if (rend == null)
                    continue;
                rend.material.shader = Shader.Find("GUI/Text Shader");
                rend.material.color = new Color(0.6f, 0.6f, 0f, 0.6f);
            }
        }

        public static void DisableEntityESP(ThrowableBug.BugName bugType)
        {
            ThrowableBug[] bugs = GetThrowableBugs();
            if (bugs == null)
                return;
            foreach (ThrowableBug entity in bugs)
            {
                if (entity == null || entity.bugName != bugType)
                    continue;
                SkinnedMeshRenderer rend = entity.GetComponentInChildren<SkinnedMeshRenderer>();
                if (rend == null)
                    continue;
                rend.material.shader = Shader.Find("GorillaTag/UberShader");
            }
        }

        private static List<ThrowableBug> cachedBugs = new List<ThrowableBug>();
        private static bool done = false;
        private static ThrowableBug[] GetThrowableBugs()
        {
            if (done)
                return cachedBugs.ToArray();
            done = true;
            foreach (ThrowableBug entity in GameObject.FindObjectsByType<ThrowableBug>(FindObjectsSortMode.None))
            {
                if (entity != null && !cachedBugs.Contains(entity))
                    cachedBugs.Add(entity);
            }
            return cachedBugs.ToArray();
        }

        public static void ChamRig(VRRig rig, bool enable, Color color)
        {
            if (rig != null)
            {
                rig.mainSkin.material.shader = enable ? Shader.Find("GUI/Text Shader") : Shader.Find("GorillaTag/UberShader");
                rig.mainSkin.material.color = enable ? color : rig.playerColor;
            }
        }
    }
}
