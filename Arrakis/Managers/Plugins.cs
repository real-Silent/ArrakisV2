using Arrakis.Classes;
using Arrakis.Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Arrakis.Managers
{
    public class Plugins
    {
        public class Plugin
        {
            public string fileName;
            public bool enabled;
            public string name;
            public string description;
            public Assembly assembly;
        }
        private class Hooks
        {
            public MethodInfo OnEnable;
            public MethodInfo OnDisable;
            public MethodInfo[] OnGUI;
            public MethodInfo[] Update;
        }

        public static readonly List<Plugin> plugins = new List<Plugin>();
        public static void LoadPlugin()
        {
            if (plugins.Count > 0)
            {
                foreach (var plugin in plugins.Where(plugin => plugin.enabled))
                    DisablePlugin(plugin.assembly);
            }

            if (!Directory.Exists($"{PluginInfo.BaseDirectory}/Plugins"))
                Directory.CreateDirectory($"{PluginInfo.BaseDirectory}/Plugins");

            string[] disabled = { };
            if (!File.Exists($"{PluginInfo.BaseDirectory}/Plugins/disabled.txt"))
                File.WriteAllText($"{PluginInfo.BaseDirectory}/Plugins/disabled.txt", "");
            else
            {
                string text = File.ReadAllText($"{PluginInfo.BaseDirectory}/Plugins/disabled.txt");
                if (text.Length > 1)
                    disabled = text.Split("\n").Select(l => l.Trim()).Where(l => l.Length > 0).ToArray();
            }
            string[] files = Directory.GetFiles($"{PluginInfo.BaseDirectory}/Plugins");
            foreach (string file in files)
            {
                try
                {
                    if (!file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                        continue;
                    string pluginName = file.Replace($"{PluginInfo.BaseDirectory}/Plugins/", "");
                    Assembly assembly = GetAssembly(file);
                    string[] data = GetPluginInfo(assembly);
                    Plugin plugin = new Plugin()
                    {
                        fileName = pluginName,
                        name = data[0],
                        description = data[1],
                        assembly = assembly,
                        enabled = !disabled.Contains(pluginName)
                    };
                    if (plugin.enabled)
                        EnablePlugin(plugin.assembly);
                    plugins.Add(plugin);
                }
                catch { }
            }
        }

        private static string SanitizeFileName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            string cleaned = new string(name.Where(c => !invalid.Contains(c)).ToArray());
            cleaned = cleaned.Replace("..", "");
            return cleaned;
        }

        public static void TogglePlugin(Plugin plugin)
        {
            if (plugin.enabled)
                DisablePlugin(plugin.assembly);
            else
                EnablePlugin(plugin.assembly);
            plugin.enabled = !plugin.enabled;
            string disabledPluginsString = plugins.Where(p => !p.enabled).Select(p => p.fileName).Aggregate("", (current, disabledPlugin) => current + (disabledPlugin + "\n"));
            File.WriteAllText($"{PluginInfo.BaseDirectory}/Plugins/DisabledPlugins.txt", disabledPluginsString);
            Main.GetIndex(plugin.fileName).overlapText = (plugin.enabled ? "<color=grey>[</color><color=cyan>ON</color><color=grey>]</color>" : "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>") + " " + plugin.name;
        }

        public static void ExecuteUpdate()
        {
            foreach (Plugin plugin in plugins.Where(plugin => plugin.enabled))
            {
                try
                {
                    foreach (MethodInfo method in ResolveHooks(plugin.assembly).Update)
                        method.Invoke(null, null);
                }
                catch { }
            }
        }
        public static void ExecuteOnGUI()
        {
            foreach (Plugin plugin in plugins.Where(plugin => plugin.enabled))
            {
                try
                {
                    foreach (MethodInfo method in ResolveHooks(plugin.assembly).OnGUI)
                        method.Invoke(null, null);
                }
                catch { }
            }
        }

        private static readonly Dictionary<string, Assembly> cacheAssembly = new Dictionary<string, Assembly>();
        private static Assembly GetAssembly(string dllName)
        {
            if (cacheAssembly.TryGetValue(dllName, out var assembly))
                return assembly;
            Assembly loaded = Assembly.Load(File.ReadAllBytes(dllName.Replace("/", "\\")));
            cacheAssembly.Add(dllName, loaded);
            return loaded;
        }
        private static string[] GetPluginInfo(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                FieldInfo name = type.GetField("name", BindingFlags.Public | BindingFlags.Static);
                FieldInfo description = type.GetField("description", BindingFlags.Public | BindingFlags.Static);
                if (name != null && description != null)
                    return new[] { (string)name.GetValue(null), (string)description.GetValue(null) };
            }
            return new[] { "null", "null" };
        }

        private static readonly Dictionary<Assembly, Hooks> cacheHooks = new Dictionary<Assembly, Hooks>();
        private static Hooks ResolveHooks(Assembly assembly)
        {
            if (cacheHooks.TryGetValue(assembly, out var cached))
                return cached;
            Type[] types = assembly.GetTypes();
            Hooks hooks = new Hooks
            {
                OnEnable = types.Select(type => type.GetMethod("OnEnable", BindingFlags.Public | BindingFlags.Static)).FirstOrDefault(method => method != null),
                OnDisable = types.Select(type => type.GetMethod("OnDisable", BindingFlags.Public | BindingFlags.Static)).FirstOrDefault(method => method != null),
                OnGUI = types.Select(type => type.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.Static)).Where(method => method != null).ToArray(),
                Update = types.Select(type => type.GetMethod("Update", BindingFlags.Public | BindingFlags.Static)).Where(method => method != null).ToArray()
            };
            cacheHooks.Add(assembly, hooks);
            return hooks;
        }

        private static void EnablePlugin(Assembly assembly)
        {
            try
            {
                ResolveHooks(assembly).OnEnable?.Invoke(null, null);
            }
            catch { }
        }

        private static void DisablePlugin(Assembly assembly)
        {
            try
            {
                ResolveHooks(assembly).OnDisable?.Invoke(null, null);
            }
            catch { }
        }
    }
}