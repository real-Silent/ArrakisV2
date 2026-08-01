using System.IO;
using Arrakis.Classes;
using Arrakis.Classes.Menu;
using Arrakis.Menu;
using Arrakis.Notifications;
using BepInEx;
using UnityEngine;

namespace Arrakis
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            HarmonyLoader.ApplyPatches();
            CustomConsole.LoadStart();
            GorillaTagger.OnPlayerSpawned(OnPlayerSpawned);
        }

        public void OnPlayerSpawned()
        {
            GameObject holder = new GameObject("Arrakis"); // This has alot of shit but like yea idrc it works -nova
            holder.AddComponent<ServerData>();
            holder.AddComponent<NotificationManager>();
            holder.AddComponent<CRunner>();
            holder.AddComponent<CosmeticsFinder>();
            holder.AddComponent<PcGui>();
            holder.AddComponent<Admin>();
            holder.AddComponent<UserCount>();

            if (!Directory.Exists("Arrakis"))
                Directory.CreateDirectory("Arrakis");

            Settings.LoadSettings();
            Patches.CosmeticPatch.enabled = true;
            if (Main.OneIn(500))
                Main.RareChance = true;
        }

        private void OnDisable() =>
            HarmonyLoader.RemovePatches();
    }
}