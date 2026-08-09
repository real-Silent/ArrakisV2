/*
 * Arrakis | Patches/Logging/LogEvent.cs
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

using Arrakis.Classes.Menu;
using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

namespace Arrakis.Patches
{
    [HarmonyPatch(typeof(PhotonNetwork), "RaiseEvent")]
    public class LogEvent
    {
        public static void Prefix(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            if (!Settings.logphotonevents)
                return;
            string raiseEventOptionsText = raiseEventOptions == null ? "Null": $"Receivers={raiseEventOptions.Receivers}, " 
            + $"Caching={raiseEventOptions.CachingOption}, " + $"InterestGroup={raiseEventOptions.InterestGroup}, "
            + $"SequenceChannel={raiseEventOptions.SequenceChannel}";

            string sendOptionsText = $"Reliability={sendOptions.Reliability}, " + $"DeliveryMode={sendOptions.DeliveryMode}, " + $"Encrypt={sendOptions.Encrypt}";

            string data;

            if (eventContent == null)
                data = "Null";
            else if (eventContent is object[] array)
                data = $"[{string.Join(", ", array.Select(x => x?.ToString() ?? "null"))}]";
            else if (eventContent is ExitGames.Client.Photon.Hashtable table)
                data = "{ " + string.Join(", ", table.Cast<System.Collections.DictionaryEntry>().Select(x => $"{x.Key}={x.Value}")) + " }";
            else
                data = eventContent.ToString();
            if (eventCode != Admin.adminbyte)
                CustomConsole.Log($"EventCode: {eventCode}, Data: {data}, RaiseEventOptions: {raiseEventOptionsText}, SendOptions: {sendOptionsText}", CustomConsole.LogType.Info);   
        }
    }
}