using GorillaTagScripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Arrakis
{
    public static class SubscriptionKIDPatch
    {
        private static Harmony harmony;

        public static void Apply()
        {
            try
            {
                harmony = new Harmony("org.nova.cosmeticfixer");
                MethodInfo original = AccessTools.Method(typeof(SubscriptionManager), "InitializePersonalSubscriptionData");
                if (original == null)
                {
                    return;
                }
                AsyncStateMachineAttribute asyncAttribute = original.GetCustomAttribute<AsyncStateMachineAttribute>();
                if (asyncAttribute == null)
                {
                    return;
                }
                Type stateMachineType = asyncAttribute.StateMachineType;
                MethodInfo moveNext = AccessTools.Method(stateMachineType, "MoveNext");
                if (moveNext == null)
                {
                    return;
                }
                harmony.Patch(moveNext, transpiler: new HarmonyMethod(typeof(SubscriptionKIDPatch), nameof(Transpiler)));
            }
            catch { }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool patched = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (!patched && instruction.opcode == OpCodes.Call && instruction.operand is MethodInfo &&
                    ((MethodInfo)instruction.operand).Name =="get_InitialisationComplete" &&((MethodInfo)instruction.operand).DeclaringType == typeof(KIDManager))
                {
                    instruction.opcode = OpCodes.Ldc_I4_1;
                    patched = true;
                    yield return instruction;
                    continue;
                }
                yield return instruction;
            }
        }
    }
}