/*
 * Arrakis | Mods/CustomMaps/ChimpCombat.cs
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

using System.Collections.Generic;
using Arrakis.Classes;
using Arrakis.Managers.CustomMaps;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Arrakis.Managers.CustomMaps.Manager;

namespace Arrakis.Mods.CustomMaps
{
    public class ChimpCombat : CustomMap
    {
        public override long MapID => 5135423;
        public override ButtonInfo[] Buttons => new[]
        {
            new ButtonInfo { buttonText = "Chimp Combat Crash All", method = ChimpCombatCrashAll, isTogglable = false, toolTip = "Crashes everyone in the chimp combat if they have the map loaded." },
        };
        public static float crashDelay;
        public static void CrashPlayer(int ActorNumber)
        {
            if (Time.time > crashDelay)
            {
                PhotonNetwork.RaiseEvent(180, new object[] { "leaveGame", (double)ActorNumber, false, (double)ActorNumber }, new RaiseEventOptions
                {
                    TargetActors = new[]
                    {
                        ActorNumber
                    }
                }, SendOptions.SendReliable);
                Safety.RPCProc();
                crashDelay = Time.time + 0.1f;
            }
        }
        public static void ChimpCombatCrashAll()
        {
            foreach (NetPlayer player in NetworkSystem.Instance.PlayerListOthers)
                CrashPlayer(player.ActorNumber);
        }
    }
}
