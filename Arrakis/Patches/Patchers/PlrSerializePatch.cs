using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace Arrakis.Patches.Patchers
{    [HarmonyPatch(typeof(VRRig), nameof(VRRig.SerializeReadShared))]
    public class PlrSerializePatch
    {
        public static bool stopSerialization;
        public static float? delay;
        public static event Action<VRRig> OnPlayerSerialize;
        public static bool Prefix(VRRig __instance, InputStruct data)
        {
            if (stopSerialization)
                return false;

            if (delay != null)
            {
                Arrakis.Classes.CRunner.instance.StartCoroutine(
                    SerializationDelay(() =>
                    {
                        float oldDelay = delay.Value;
                        delay = null;
                        try
                        {
                            __instance.SerializeReadShared(data);
                        }
                        catch { }
                        delay = oldDelay;
                    }, delay.Value)
                );

                return false;
            }

            return true;
        }
        public static IEnumerator SerializationDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        public static void Postfix(VRRig __instance) =>
            OnPlayerSerialize?.Invoke(__instance);
    }
}
