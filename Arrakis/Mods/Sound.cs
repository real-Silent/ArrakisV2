namespace Arrakis.Mods
{
    public class Sound
    {
        private static void PlaySound(int soundIndex, bool leftHand, float volume, bool button)
        {
            if (button)
            {
                if (NetworkSystem.Instance.InRoom)
                {
                    GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", Photon.Pun.RpcTarget.All, new object[] { soundIndex, leftHand, volume });
                    Safety.RPCProc();
                }
                else
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(soundIndex, leftHand, volume);
            }
        }
        public static void SoundSpam(int index) =>
            PlaySound(index, false, 99f, ControllerInputPoller.instance.rightControllerSecondaryButton);
    }
}