using HarmonyLib;
using System.Reflection;

namespace Arrakis
{
    public class HarmonyLoader
    {
        private static Harmony harmony = null;
        private static bool patched = false;
        public static void ApplyPatches()
        {
            if (patched)
                return;
            patched = true;
            CustomConsole.Log("Applying patches");
            harmony = new Harmony(PluginInfo.GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            CustomConsole.Log("Patched");
        }

        public static void RemovePatches()
        {
            if (!patched) 
                return;
            patched = false;
            CustomConsole.Log("Removing patches");
            harmony.UnpatchSelf();
            CustomConsole.Log("Removed harmony patches");
        }
    }
}