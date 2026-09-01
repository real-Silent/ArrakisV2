/*
 * Arrakis | Extensions/VRRigExtensions.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;

namespace Arrakis.Extensions
{
    public static class VRRigExtensions
    {
        public static bool IsLocal(this VRRig rig, bool ghostRig = true) =>
            rig != null && rig.isLocal;
        public static bool IsSteam(this VRRig rig) =>
            rig.GetPlatform() != "Standalone";
        public static bool Active(this VRRig rig) =>
            rig != null && VRRigCache.ActiveRigs.Contains(rig);
        public static Photon.Realtime.Player GetPhotonPlayer(this VRRig rig) =>
            rig.Creator.GetPlayerRef();
        public static NetPlayer GetPlayer(this VRRig rig) =>
            rig.Creator;
        public static string GetNickName(this VRRig rig) =>
            rig.Creator.NickName;
        public static string GetUserID(this VRRig rig) =>
            rig.Creator.UserId;
        public static NetworkView GetNetView(this VRRig rig) =>
            rig.netView;
        public static PhotonView GetPhotonView(this VRRig rig) =>
            rig.netView.GetView;
        public static string Cosmetics(this VRRig rig) =>
            rig._playerOwnedCosmetics.Concat();
        public static string GetPlatform(this VRRig rig)
        {
            int suspiciouslySteam = 0;
            int suspiciouslyPC = 0;
            int suspiciouslyQuest = 0;
            string concatStringOfCosmeticsAllowed = rig.Cosmetics();
            if (concatStringOfCosmeticsAllowed.Contains("S. FIRST LOGIN"))
                suspiciouslySteam++;
            if (concatStringOfCosmeticsAllowed.Contains("FIRST LOGIN") || rig.GetPhotonPlayer().CustomProperties.Count >= 2)
                suspiciouslyPC++;
            if (rig.currentRankedSubTierPC > 0)
                suspiciouslyPC++;
            else if (rig.currentRankedSubTierQuest > 0)
                suspiciouslyQuest++;
            if (suspiciouslySteam > suspiciouslyPC && suspiciouslySteam > suspiciouslyQuest) return "Steam";
            if (suspiciouslyPC > suspiciouslySteam && suspiciouslyPC > suspiciouslyQuest) return "PC";
            if (suspiciouslyQuest > suspiciouslySteam && suspiciouslyQuest > suspiciouslyPC) return "Standalone";
            return "Standalone";
        }
        public static bool IsTagged(this VRRig rig) 
        {
            if (GorillaGameManager.instance == null) return false;
            if (rig == null) return false;
            List<NetPlayer> infectedPlayers = ((GorillaTagManager)GorillaGameManager.instance).currentInfected;
            NetPlayer targetPlayer = rig.GetPlayer();
            return infectedPlayers.Contains(targetPlayer);
        }
        public static int GetPing(this VRRig rig)
        {
            double ping = Math.Abs((rig.velocityHistoryList[0].time - PhotonNetwork.Time) * 1000);
            int safePing = (int)Math.Clamp(Math.Round(ping), 0, int.MaxValue);

            return safePing;
        }
    }
}