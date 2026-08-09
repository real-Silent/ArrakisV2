/*
 * Arrakis | Mods/Settings.cs
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
using Arrakis.Classes.Menu;
using Arrakis.Menu;
using Arrakis.Notifications;
using GorillaLocomotion;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void GlobalReturn()
        {
            NotificationManager.ClearAllNotifications();
            Toggle(Buttons.buttons[currentCategoryIndex][GetCategory("Main")].buttonText);
            if (prompts.Count > 0)
                StopCurrentPrompt();
        }

        public static Font currentFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        public static bool fpsCounter = true;
        public static bool disconnectButton = true;
        public static bool rightHanded;
        public static bool disableNotifications;
        public static bool outlineMenu;
        public static bool menusounds = true;
        public static bool gunline = true;
        public static bool gunpointer = true;
        public static bool cosmeticfinder = true;

        public static bool showanticheatreports = false;
        public static bool showanticheatreportself = false;

        public static bool shouldBePC;

        public static bool followheadmesh = false;
        public static bool followmenutheme = false;
        public static bool fliparraylist = false;
        public static bool flipnotifications = false;
        public static bool menuanimation = false;
        public static bool highqualitytext = false;
        public static bool menutrail = false;
        public static bool pointertrail = false;
        public static bool disablepointer = false;
        public static bool disablemenudrop = false;
        public static bool disableautosave = false;
        public static bool disableReturnButton = false;
        public static bool lowgravitymenu = false;
        public static bool custommenutitle = false;
        public static bool disablemenutitle = false;
        public static bool disablepagenumber = false;
        public static bool disablecustomboards = false;
        public static bool disableroomnotifications = false;
        public static bool disableFavBinds = false;
        public static bool disableQuickactionsBinds = false;

        public static KeyCode keyboardButton = KeyCode.Q;

        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f); // Depth, width, height
        public static int buttonsPerPage = 8;

        public static bool logphotonevents = false;

        public static bool disablevibrations;

        public static void JoinDiscord()
        {
            Prompt("Join Arrakis Discord", () => 
            {
                Process.Start("https://novax.lol/d");
            }, () =>
            {
                CurrentCategoryName = "Main";
                StopCurrentPrompt();
            });
        }

        public static void Players()
        {
            List<ButtonInfo> buttons = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "Exit Players", method =() => CurrentCategoryName = "Main", isTogglable = false, toolTip = "Returns to the main page for the menu." }
            };
            if (!NetworkSystem.Instance.InRoom)
                buttons.Add(new ButtonInfo { buttonText = "not in a room", label = true });
            else
            {
                for (int i = 0; i < NetworkSystem.Instance.PlayerListOthers.Length; i++)
                {
                    NetPlayer plr = NetworkSystem.Instance.PlayerListOthers[i];
                    buttons.Add(new ButtonInfo { buttonText = $"plr{i}", overlapText = plr.NickName, isTogglable = false, method =() => GoToPlayer(plr), toolTip = "Lets you see player info and more." });
                }
            }
            Buttons.buttons[GetCategory("Players")] = buttons.ToArray();
            CurrentCategoryName = "Players";
        }

        private static void GoToPlayer(NetPlayer plr)
        {
            string plrName = plr.NickName;
            VRRig rig = RigManager.GetVRRigFromPlayer(plr);
            List<ButtonInfo> buttons = new List<ButtonInfo>
            {
                new ButtonInfo { buttonText = "exit plr", overlapText = $"Exit {plrName}", method =() => Players(), isTogglable = false, toolTip = "Returns to the players page for the menu." },
                //new ButtonInfo { buttonText = "Tag plr", overlapText = $"Tag {plrName}", method =() => Advantage.TagPlr(plr, $"Tag {plrName}"), isTogglable = true, toolTip = "Tags the person." },
                new ButtonInfo { buttonText = "plr name", overlapText = $"Name: {plrName}", label = true },
                new ButtonInfo { buttonText = "plr id", overlapText = $"UserID: {plr.UserId}", label = true },
                new ButtonInfo { buttonText = "plr fps", overlapText = $"FPS: {rig.fps}", label = true },
            };
            if (Admin.Admins.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
            {
                buttons.AddRange(new[] 
                { 
                    new ButtonInfo { buttonText = "admin kick plr", overlapText = $"Admin Kick {plrName}", isTogglable = false, method =() => Admin.ExecuteCommand("kick", ReceiverGroup.All, plr.UserId) }
                });
            }
            Buttons.buttons[GetCategory("Temporary")] = buttons.ToArray();
            CurrentCategoryName = "Temporary";
        }

        public static float gradientSpeed = 0.5f;
        public static bool FloatMenu = false;
        public static int buttonsound = 67;
        private static int clicksound = 0;
        public static void ChangeClickSound() // Making it get its name from GTPlayer.Instance.materialData[buttonsound].matName
        {
            clicksound = (clicksound + 1) % 11;
            switch (clicksound)
            {
                case 0:
                    buttonsound = 67;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 1:
                    buttonsound = 66;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 2:
                    buttonsound = 8;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 3:
                    buttonsound = 84;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 4:
                    buttonsound = 32;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 5:
                    buttonsound = 106;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 6:
                    buttonsound = 189;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 7:
                    buttonsound = 22;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 8:
                    buttonsound = 43;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
                case 9:
                    buttonsound = 210;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>"; 
                    break;
                case 10:
                    buttonsound = 217;
                    GetIndex("Change Click Sound").overlapText = $"Change Click Sound <color=grey>[<color=cyan>{GTPlayer.Instance.materialData[buttonsound].matName}</color>]</color>";
                    break;
            }
        }

        public static float buttonclickvolume = 0.4f;
        private static int buttonclickvolumeindex = 0;
        public static void ChangeButtonClickVolume()
        {
            buttonclickvolumeindex = (buttonclickvolumeindex + 1) % 10;
            switch (buttonclickvolumeindex)
            {
                case 0: buttonclickvolume = 0.1f; break;
                case 1: buttonclickvolume = 0.2f; break;
                case 2: buttonclickvolume = 0.3f; break;
                case 3: buttonclickvolume = 0.4f; break;
                case 4: buttonclickvolume = 0.5f; break;
                case 5: buttonclickvolume = 0.6f; break;
                case 6: buttonclickvolume = 0.7f; break;
                case 7: buttonclickvolume = 0.8f; break;
                case 8: buttonclickvolume = 0.9f; break;
                case 9: buttonclickvolume = 1f; break;
            }
            GetIndex("Change Click Volume").overlapText = $"Change Click Volume <color=grey>[<color=cyan>{buttonclickvolume}f</color>]</color>";
        }
        private static int GunLineindex = 0;
        public static void ChangeGunline()
        {
            GunLineindex = (GunLineindex + 1) % 5;
            switch (GunLineindex)
            {
                case 0: 
                    gunLineStyle = 0;
                    GetIndex("Change Gun Line").overlapText = "Change Gun Line <color=grey>[<color=cyan>Default</color>]</color>";
                    break;
                case 1: 
                    gunLineStyle = 1;
                    GetIndex("Change Gun Line").overlapText = "Change Gun Line <color=grey>[<color=cyan>Straight</color>]</color>";
                    break;
                case 2: 
                    gunLineStyle = 2;
                    GetIndex("Change Gun Line").overlapText = "Change Gun Line <color=grey>[<color=cyan>Rainbow</color>]</color>";
                    break;
                case 3: 
                    gunLineStyle = 3;
                    GetIndex("Change Gun Line").overlapText = "Change Gun Line <color=grey>[<color=cyan>ZigZag</color>]</color>";
                    break;
                case 4:
                    gunLineStyle = 4;
                    GetIndex("Change Gun Line").overlapText = "Change Gun Line <color=grey>[<color=cyan>Pulse</color>]</color>";
                    break;
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

        public static int currentTheme = 0;

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
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Default</color>]</color>";
                    break;
                case 1: // Blue
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.blue) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Blue</color>]</color>";
                    break;
                case 2: // Red
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.blue) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Red</color>]</color>";
                    break;
                case 3: // Transparent
                    backgroundColor = new ExtGradient { transparent = true };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Transparent</color>]</color>";
                    break;
                case 4: // Pastel
                    backgroundColor = new ExtGradient { pastelRainbow = true };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Pastel</color>]</color>";
                    break;
                case 5: // Black Grey Fade
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.black, Color.grey) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>BG Fade</color>]</color>";
                    break;
                case 6: // White Black Fade
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.white, Color.black) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>WB Fade</color>]</color>";
                    break;
                case 7: // Full Black
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.grey;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Black</color>]</color>";
                    break;
                case 8: // Full White
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.white) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.white) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.white) };
                    textColors[0] = Color.black;
                    textColors[1] = Color.grey;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>White</color>]</color>";
                    break;
                case 9: // Rainbow
                    backgroundColor = new ExtGradient { rainbow = true };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.red) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Rainbow</color>]</color>";
                    break;
                case 10: // Porn Hub cause sleepy wanted it -nova
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.orange, Color.black) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.orange) };
                    textColors[0] = Color.orange;
                    textColors[1] = Color.black;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>PH</color>]</color>";
                    break;
                case 11: // Grey
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.grey) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.grey) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Grey</color>]</color>";
                    break;
                case 12: // Dark Grey
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(new Color(0.1f, 0.1f, 0.1f)) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(new Color(0.1f, 0.1f, 0.1f)) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Dark Grey</color>]</color>";
                    break;
                case 13: // Yellow
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.yellow) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.yellow) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=cyan>Yellow</color>]</color>";
                    break;
                case 14: // Cyan
                    backgroundColor = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.cyan) };
                    buttonColors[0] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.cyan) };
                    buttonColors[1] = new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) };
                    textColors[0] = Color.white;
                    textColors[1] = Color.white;
                    GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[<color=black>Cyan</color>]</color>";
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
                    GetIndex("Change Font Style").overlapText = "Change Font Style <color=grey>[<color=cyan>Italic</color>]</color>";
                    break;
                case 1:
                    currentStyle = FontStyle.Bold;
                    GetIndex("Change Font Style").overlapText = "Change Font Style <color=grey>[<color=cyan>Bold</color>]</color>";
                    break;
                case 2:
                    currentStyle = FontStyle.Normal;
                    GetIndex("Change Font Style").overlapText = "Change Font Style <color=grey>[<color=cyan>Normal</color>]</color>";
                    break;
                case 3:
                    currentStyle = FontStyle.BoldAndItalic;
                    GetIndex("Change Font Style").overlapText = "Change Font Style <color=grey>[<color=cyan>Bold & Italic</color>]</color>";
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
            public int buttonclickvolumeindex { get; set; }
            public int currentTheme { get; set; }
            public int fontstyle { get; set; }
            public int currentFlySpeed { get; set; }
            public int currentDigSize { get; set; }
            public int currentprojectilecolor { get; set; }
            public int GunLineindex { get; set; }
            public List<string> enabledMods { get; set; } = new List<string>();
            public List<string> favorites { get; set; } = new List<string>();
            public List<string> quickactions { get; set; } = new List<string>();
        }

        public static void SaveSettings()
        {
            Directory.CreateDirectory(PluginInfo.BaseDirectory);

            SavedSettings settings = new SavedSettings
            {
                clicksound = clicksound,
                buttonclickvolumeindex = buttonclickvolumeindex,
                currentTheme = currentTheme,
                fontstyle = fontstyle,
                currentFlySpeed = currentFlySpeed,
                currentDigSize = currentDigSize,
                currentprojectilecolor = currentprojectilecolor,
                GunLineindex = GunLineindex,
                enabledMods = Buttons.buttons.SelectMany(x => x).Where(x => x.enabled).Select(x => x.buttonText).ToList(),
                favorites = favorites,
                quickactions = quickactions
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

                buttonclickvolumeindex = settings.buttonclickvolumeindex - 1;
                ChangeButtonClickVolume();

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

                GunLineindex = settings.GunLineindex - 1;
                ChangeGunline();

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

                quickactions.Clear();
                foreach (var quick in settings.quickactions)
                    quickactions.Add(quick);
            }
            catch (Exception ex)
            {
                CustomConsole.Log($"Failed to load settings: {ex}", CustomConsole.LogType.Warning);
            }
        }
    }
}