/*
 * Arrakis | Classes/Menu/ButtonCollider.cs
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

using Arrakis.Managers;
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
				if (!disablevibrations)
					GorillaTagger.Instance.StartVibration(rightHanded, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
				AudioManager.PlayButtonSound(rightHanded, buttonclickvolume);
                Toggle(relatedText);
            }
		}
	}
}