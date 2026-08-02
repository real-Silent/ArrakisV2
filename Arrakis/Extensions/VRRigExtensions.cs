using Photon.Pun;
using System.Linq;

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
        public static NetPlayer GetNetPlayer(this VRRig rig) =>
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
    }
}