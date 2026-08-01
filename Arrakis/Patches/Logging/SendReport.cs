using HarmonyLib;
using static Arrakis.Settings;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(MonkeAgent), nameof(MonkeAgent.SendReport))]
    internal class SendReport
    {
        private static void Prefix(string susReason, string susId, string susNick)
        {
            bool isSelf = susId == NetworkSystem.Instance.LocalPlayer.UserId;
            if (showanticheatreportself)
            {
                if (isSelf)
                    CustomConsole.Log($"AntiCheat Reported {susNick}, {susId} for {susReason}", CustomConsole.LogType.Info);
            }
            else if (showanticheatreports)
            {
                if (!isSelf)
                    CustomConsole.Log($"AntiCheat Reported {susNick}, {susId} for {susReason}", CustomConsole.LogType.Info);
            }
            susReason = null;
            susId = null;
            susNick = null;
        }
    }
}