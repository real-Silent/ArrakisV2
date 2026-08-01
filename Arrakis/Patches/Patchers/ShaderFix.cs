using HarmonyLib;
using UnityEngine;

namespace Arrakis.Patches.Menu
{
    [HarmonyPatch(typeof(GameObject), nameof(GameObject.CreatePrimitive))]
    public class ShaderFix
    {
        private static void Postfix(GameObject __result)
        {
            __result.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
            __result.GetComponent<Renderer>().material.color = Color.black;
        }
    }
}