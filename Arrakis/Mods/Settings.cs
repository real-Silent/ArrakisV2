using Arrakis.Classes;
using Arrakis.Menu;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Arrakis.Menu.Main;

namespace Arrakis
{
    public class Settings
    {
        public static ExtGradient backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.cyan, Color.magenta) };
        public static ExtGradient[] buttonColors = new ExtGradient[]
        {
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) }, // Disabled
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) } // Enabled
        };
        public static Color[] textColors = new Color[]
        {
            Color.white, // Disabled
            Color.white // Enabled
        };

        public static Font currentFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        public static bool fpsCounter = true;
        public static bool disconnectButton = true;
        public static bool rightHanded;
        public static bool disableNotifications;
        public static bool outlineMenu;
        public static bool gunline = true;
        public static bool cosmeticfinder = true;

        public static bool showanticheatreports = false;
        public static bool showanticheatreportself = false;

        public static bool shouldBePC;

        public static bool followheadmesh = false;
        public static bool followmenutheme = false;
        public static bool fliparraylist = false;
        public static bool menuanimation = false;
        public static bool menutrail = false;
        public static bool pointertrail = false;
        public static bool disablepointer = false;
        public static bool disablemenudrop = false;
        public static bool lowgravitymenu = false;

        public static KeyCode keyboardButton = KeyCode.Q;

        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f); // Depth, width, height
        public static int buttonsPerPage = 8;

        public static bool logphotonevents = false;

        public static float gradientSpeed = 0.5f;
        public static bool FloatMenu = false;
        public static int buttonsound = 67;
        private static int clicksound = 0;
        public static void ChangeClickSound()
        {
            clicksound = (clicksound + 1) % 11;
            switch (clicksound)
            {
                case 0: buttonsound = 67; break;
                case 1: buttonsound = 66; break;
                case 2: buttonsound = 8; break;
                case 3: buttonsound = 84; break;
                case 4: buttonsound = 32; break;
                case 5: buttonsound = 106; break;
                case 6: buttonsound = 189; break;
                case 7: buttonsound = 22; break;
                case 8: buttonsound = 43; break;
                case 9: buttonsound = 210; break;
                case 10: buttonsound = 217; break;
            }
        }

        public static void FreezePlayerInMenu()
        {
            if (menu != null)
                GorillaTagger.Instance.bodyCollider.attachedRigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
        }
        public static void FreezeRigInMenu()
        {
            if (menu != null)
                VRRig.LocalRig.enabled = false;
            else
                VRRig.LocalRig.enabled = true;
        }

        private static int currentTheme = 0;

        public static void ChangeMenuTheme()
        {
            currentTheme = (currentTheme + 1) % 15; // Always make this number above the last case so if its case 14: make it 15
            switch (currentTheme)
            {
                case 0: // Default
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.cyan, Color.magenta) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 1: // Blue
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.blue) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 2: // Red
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.blue) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 3: // Transparent
                    backgroundColor = new ExtGradient { transparent = true };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 4: // Pastel
                    backgroundColor = new ExtGradient { pastelRainbow = true };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 5: // Black Grey Fade
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.black, Color.grey) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 6: // White Black Fade
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.white, Color.black) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 7: // Full Black
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.grey;
                    break;
                case 8: // Full White
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.white) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.white) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.white) };
                    textColors[0] = Color.black;
                    textColors[1] = Color.grey;
                    break;
                case 9: // Rainbow
                    backgroundColor = new ExtGradient { rainbow = true };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 10: // Porn Hub cause sleepy wanted it -nova
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.orange, Color.black) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.orange) };
                    textColors[0] = Color.orange;
                    textColors[1] = Color.black;
                    break;
                case 11: // Grey
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.grey) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.grey) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 12: // Dark Grey
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(new Color(0.1f, 0.1f, 0.1f)) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(new Color(0.1f, 0.1f, 0.1f)) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 13: // Yellow
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.yellow) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.yellow) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
                case 14: // Cyan
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.cyan) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.cyan) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    break;
            }
        }

        private static int fontstyle = 0;
        public static FontStyle currentStyle = FontStyle.Italic;
        public static void ChangeFontStyle()
        {
            fontstyle = (fontstyle + 1) % 4;
            switch (fontstyle)
            {
                case 0:
                    currentStyle = FontStyle.Italic;
                    break;
                case 1:
                    currentStyle = FontStyle.Bold;
                    break;
                case 2:
                    currentStyle = FontStyle.Normal;
                    break;
                case 3:
                    currentStyle = FontStyle.BoldAndItalic  ;
                    break;
            }
        }

        // Projectile Settings
        public static bool allowbigsnowballcolor = false;
        private static int currentprojectilecolor = 0;
        public static Color projectileColor = Color.white;
        public static void ChangeProjectilesColor()
        {
            currentprojectilecolor = (currentprojectilecolor + 1) % 5;
            switch (currentprojectilecolor)
            {
                case 0: projectileColor = Color.white; break;
                case 1: projectileColor = Color.red; break;
                case 2: projectileColor = Color.blue; break;
                case 3: projectileColor = Color.black; break;
                case 4: projectileColor = Color.yellow; break;
            }
        }


        // Movement Settings
        public static bool stickyplats = false;

        public static float wasdflyspeed = 10f;
        public static float flyspeed = 10f;
        private static int currentFlySpeed = 0;
        public static void ChangeFlySpeed()
        {
            string[] speeds = { "Normal", "Very Slow", "Slow", "Fast", "Very Fast" };
            float[] values = { 10f, 2f, 5f, 20f, 40f };
            currentFlySpeed = (currentFlySpeed + 1) % speeds.Length;
            flyspeed = values[currentFlySpeed];
            GetIndex("Change Fly Speed").overlapText = $"Change Fly Speed <color=grey>[<color=cyan>{speeds[currentFlySpeed]}</color>]</color>";
        }

        public static float digsize = 3f;
        private static int currentDigSize = 0;
        public static void ChangeDigSize()
        {
            string[] sizes = { "Normal", "Very Small", "Small", "Large", "Very Large" };
            float[] values = { 3f, 0.3f, 1f, 10f, 50f };
            currentDigSize = (currentDigSize + 1) % sizes.Length;
            digsize = values[currentDigSize];
            GetIndex("Change Dig Size").overlapText = $"Change Dig Size <color=grey>[<color=cyan>{sizes[currentDigSize]}</color>]</color>";
        }



        public class SavedSettings
        {
            public int clicksound { get; set; }
            public int currentTheme { get; set; }
            public int fontstyle { get; set; }
            public int currentFlySpeed { get; set; }
            public int currentDigSize { get; set; }
            public int currentprojectilecolor { get; set; }
            public List<string> enabledMods { get; set; } = new List<string>();
            public List<string> favorites { get; set; } = new List<string>();
        }

        public static void SaveSettings()
        {
            Directory.CreateDirectory(PluginInfo.BaseDirectory);

            SavedSettings settings = new SavedSettings
            {
                clicksound = clicksound,
                currentTheme = currentTheme,
                fontstyle = fontstyle,
                currentFlySpeed = currentFlySpeed,
                currentDigSize = currentDigSize,
                currentprojectilecolor = currentprojectilecolor,
                enabledMods = Buttons.buttons.SelectMany(x => x).Where(x => x.enabled).Select(x => x.buttonText).ToList(),
                favorites = favorites
            };
            settings.enabledMods = Buttons.buttons.SelectMany(x => x).Where(x => x.enabled).Select(x => x.buttonText).ToList();
            File.WriteAllText(Path.Combine(PluginInfo.BaseDirectory, "SavedSettings.json"), JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public static void LoadSettings() // Finally fixed this we love json -nova
        {
            string path = Path.Combine(PluginInfo.BaseDirectory, "SavedSettings.json");
            if (!File.Exists(path))
                return;
            try
            {
                SavedSettings settings = JsonConvert.DeserializeObject<SavedSettings>(File.ReadAllText(path));
                if (settings == null)
                    return;
                clicksound = settings.clicksound - 1;
                ChangeClickSound();

                currentTheme = settings.currentTheme - 1;
                ChangeMenuTheme();

                fontstyle = settings.fontstyle - 1;
                ChangeFontStyle();

                currentFlySpeed = settings.currentFlySpeed - 1;
                ChangeFlySpeed();

                currentDigSize = settings.currentDigSize - 1;
                ChangeDigSize();

                currentprojectilecolor = settings.currentprojectilecolor - 1;
                ChangeProjectilesColor();

                HashSet<string> enabled = settings.enabledMods.ToHashSet();
                foreach (ButtonInfo button in Buttons.buttons.SelectMany(x => x))
                {
                    bool shouldBeEnabled = enabled.Contains(button.buttonText);
                    if (button.enabled != shouldBeEnabled)
                        Toggle(button.buttonText);
                }

                favorites.Clear();
                foreach (var fav in settings.favorites)
                    favorites.Add(fav);
            }
            catch (Exception ex)
            {
                CustomConsole.Log($"Failed to load settings: {ex}", CustomConsole.LogType.Warning);
            }
        }
    }
}