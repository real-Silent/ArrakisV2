/*
 * Arrakis | Managers/InputManager.cs
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

using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Valve.VR;
using CommonUsages = UnityEngine.XR.CommonUsages;

namespace Arrakis.Managers
{
    public class InputManager
    {
        public static bool GetInput(InputType type, Hand hand, bool pc = false)
        {
            if (pc && !CheckPC())
                return false;
            if (pc && CheckPC())
            {
                return PCControl(hand, type);
            }
            return GetHandControl(hand, type);
        }

        private static bool CheckPC()
        {
            return XRSettings.isDeviceActive && UnityEngine.Application.platform != RuntimePlatform.Android;
        }

        private static bool PCControl(Hand hand, InputType type)
        {
            switch (type)
            {
                case InputType.Joystick:
                    if (hand == Hand.Left)
                    {
                        return Keyboard.current.leftAltKey.isPressed;
                    }
                    else
                    {
                        return Keyboard.current.rightAltKey.isPressed;
                    }
                case InputType.Trigger:
                    if (hand == Hand.Left)
                    {
                        return Keyboard.current.minusKey.isPressed;
                    }
                    else
                    {
                        return Keyboard.current.equalsKey.isPressed;
                    }
                case InputType.Grip:
                    if (hand == Hand.Left)
                    {
                        return Keyboard.current.leftBracketKey.isPressed;
                    }
                    else
                    {
                        return Keyboard.current.rightBracketKey.isPressed;
                    }
                case InputType.Secondary:
                    if (hand == Hand.Left)
                    {
                        return Keyboard.current.rKey.isPressed;
                    }
                    else
                    {
                        return Keyboard.current.tKey.isPressed;
                    }
                case InputType.Primary:
                    if (hand == Hand.Left)
                    {
                        return Keyboard.current.cKey.isPressed;
                    }
                    else
                    {
                        return Keyboard.current.vKey.isPressed;
                    }
                default:
                    return false;
            }
        }

        private static bool GetHandControl(Hand hand, InputType type)
        {
            bool isSteam = PlayFabAuthenticator.instance.platform.PlatformTag == "Steam";
            switch (type)
            {
                case InputType.Joystick:
                    if (hand == Hand.Left)
                    {
                        if (isSteam)
                        {
                            return SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState(SteamVR_Input_Sources.LeftHand);
                        }
                        return ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out bool leftJoystick) && leftJoystick;
                    }
                    if (hand == Hand.Right)
                    {
                        if (isSteam)
                        {
                            return SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand);
                        }
                        return ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out bool rightJoystick) && rightJoystick;
                    }
                    break;
                case InputType.Trigger:
                    if (hand == Hand.Left)
                    {
                        return ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTrigger) && leftTrigger;
                    }
                    if (hand == Hand.Right)
                    {
                        return ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTrigger) && rightTrigger;
                    }
                    break;
                case InputType.Grip:
                    if (hand == Hand.Left)
                    {
                        return ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool leftGrip) && leftGrip;
                    }
                    if (hand == Hand.Right)
                    {
                        return ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool rightGrip) && rightGrip;
                    }
                    break;
                case InputType.Secondary:
                    if (hand == Hand.Left)
                    {
                        return ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool leftSecondary) && leftSecondary;
                    }
                    if (hand == Hand.Right)
                    {
                        return ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool rightSecondary) && rightSecondary;
                    }
                    break;
                case InputType.Primary:
                    if (hand == Hand.Left)
                    {
                        return ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool leftPrimary) && leftPrimary;
                    }
                    if (hand == Hand.Right)
                    {
                        return ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool rightPrimary) && rightPrimary;
                    }
                    break;
            }
            return false;
        }

        public static Vector2 GetAxis(Hand hand)
        {
            Vector2 axis = Vector2.zero;
            bool isSteam = PlayFabAuthenticator.instance.platform.PlatformTag == "Steam";
            switch (hand)
            {
                case Hand.Left:
                    if (isSteam)
                    {
                        axis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
                    }
                    else
                    {
                        ControllerInputPoller.instance.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axis);
                    }
                    break;
                case Hand.Right:
                    if (isSteam)
                    {
                        axis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
                    }
                    else
                    {
                        ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out axis);
                    }
                    break;
            }
            return axis;
        }

        public enum Hand
        {
            Left,
            Right
        }
        public enum InputType
        {
            JoystickAxis,
            Joystick,
            Trigger,
            Grip,
            Secondary,
            Primary
        }
    }
}