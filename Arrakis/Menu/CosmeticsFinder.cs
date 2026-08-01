using Arrakis.Notifications;
using GorillaNetworking;
using UnityEngine;

namespace Arrakis.Menu
{
    public class CosmeticsFinder : MonoBehaviour
    {
        private string input = "";
        public void OnGUI()
        {
            if (!Settings.cosmeticfinder) return;

            GUILayout.Label($"CosmeticId: {input}");
            input = GUILayout.TextField(input);
            if (GUILayout.Button("Get Cosmetic Data"))
            {
                NotificationManager.SendNotification($"<color=yellow>[ARRAKIS]</color> Check console for info on {input}");
                CustomConsole.Log($"Got {input} name {CosmeticsController.instance.GetItemNameFromDisplayName(input)}", CustomConsole.LogType.Info);
                CustomConsole.Log($" Got {input} SO {CosmeticsController.instance.GetCosmeticSOFromDisplayName(input)}", CustomConsole.LogType.Info);
            }
        }
    }
}