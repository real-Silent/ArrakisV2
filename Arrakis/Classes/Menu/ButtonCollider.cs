using UnityEngine;
using static Arrakis.Menu.Main;
using static Arrakis.Settings;

namespace Arrakis.Classes
{
	public class ButtonCollider : MonoBehaviour
	{
		public string relatedText;
		public static float buttonCooldown = 0f;
		public void OnTriggerEnter(Collider collider)
		{
			if (Time.time > buttonCooldown && collider == buttonCollider || Time.time > buttonCooldown && collider == leftButtonCollider || Time.time > buttonCooldown && collider == rightButtonCollider && menu != null)
			{
                buttonCooldown = Time.time + 0.2f;
                GorillaTagger.Instance.StartVibration(rightHanded, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
                VRRig.LocalRig.PlayHandTapLocal(buttonsound, rightHanded, buttonclickvolume);
				Toggle(relatedText);
            }
		}
	}
}