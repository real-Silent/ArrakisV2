using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;
using static Arrakis.Menu.Main;

namespace Arrakis.Mods
{
    public class Movement
    {
        public static void Fly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * Settings.flyspeed;
                GTPlayer.Instance.bodyCollider.attachedRigidbody.linearVelocity = Vector3.zero;
            }
        }
        public static void TriggerFly()
        {
            if (ControllerInputPoller.instance.rightControllerTriggerButton)
            {
                GTPlayer.Instance.transform.position += GTPlayer.Instance.headCollider.transform.forward * Time.deltaTime * Settings.flyspeed;
                GTPlayer.Instance.bodyCollider.attachedRigidbody.linearVelocity = Vector3.zero;
            }
        }
        public static void HandFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GTPlayer.Instance.RightHand.controllerTransform.transform.forward * Time.deltaTime * Settings.flyspeed;
                GTPlayer.Instance.bodyCollider.attachedRigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static void MosaBoost() =>
            GTPlayer.Instance.maxJumpSpeed = 7.5f;
        public static void SpeedBoost() =>
            GTPlayer.Instance.maxJumpSpeed = 9.5f;
        public static void ExtremeSpeedBoost() =>
            GTPlayer.Instance.maxJumpSpeed = 15f;

        public static void SlideControl() =>
            GTPlayer.Instance.slideControl = 1.5f;
        public static void FixSlideControl() =>
            GTPlayer.Instance.slideControl = 0.0035429f;

        public static void PSA()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GTPlayer.Instance.RightHand.controllerTransform.transform.forward * Time.deltaTime * Settings.flyspeed;
            }
        }

        public static void ExcelFly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
                GTPlayer.Instance.bodyCollider.attachedRigidbody.linearVelocity += GTPlayer.Instance.RightHand.controllerTransform.right / 2f;
            if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                GTPlayer.Instance.bodyCollider.attachedRigidbody.linearVelocity += -GTPlayer.Instance.LeftHand.controllerTransform.right / 2f;
        }

        private static float y;
        private static float p;
        public static void WasdFly()
        {
            GTPlayer.Instance.bodyCollider.attachedRigidbody.useGravity = false;
            GTPlayer.Instance.bodyCollider.attachedRigidbody.linearVelocity = Vector3.zero;
            if (Keyboard.current.wKey.isPressed)
                GTPlayer.Instance.transform.position += GTPlayer.Instance.LeftHand.controllerTransform.parent.forward * Time.deltaTime * Settings.wasdflyspeed;
            if (Keyboard.current.sKey.isPressed)
                GTPlayer.Instance.transform.position += -GTPlayer.Instance.LeftHand.controllerTransform.parent.forward * Time.deltaTime * Settings.wasdflyspeed;
            if (Keyboard.current.aKey.isPressed)
                GTPlayer.Instance.transform.position += -GTPlayer.Instance.LeftHand.controllerTransform.parent.right * Time.deltaTime * Settings.wasdflyspeed;
            if (Keyboard.current.dKey.isPressed)
                GTPlayer.Instance.transform.position += GTPlayer.Instance.LeftHand.controllerTransform.parent.right * Time.deltaTime * Settings.wasdflyspeed;
            if (Keyboard.current.spaceKey.isPressed)
                GTPlayer.Instance.transform.position += GTPlayer.Instance.LeftHand.controllerTransform.parent.up * Time.deltaTime * Settings.wasdflyspeed;
            if (Keyboard.current.leftCtrlKey.isPressed)
                GTPlayer.Instance.transform.position += -GTPlayer.Instance.LeftHand.controllerTransform.parent.up * Time.deltaTime * Settings.wasdflyspeed;
            if (Keyboard.current.leftShiftKey.isPressed)
                Settings.wasdflyspeed = 20f;
            else
                Settings.wasdflyspeed = 10f;
            if (Mouse.current.rightButton.isPressed)
            {
                Vector2 m = Mouse.current.delta.ReadValue();
                y += m.x * 0.2f;
                p -= m.y * 0.2f;
                p = Mathf.Clamp(p, -89f, 89f);
                GTPlayer.Instance.LeftHand.controllerTransform.parent.rotation = Quaternion.Euler(p, y, 0f);
            }
            VRRig.LocalRig.head.rigTarget.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
        }

        private static GameObject platR = null;
        private static GameObject platL = null;
        public static void Platforms(bool trigger = false, bool invis = false)
        {
            if (trigger ? ControllerInputPoller.instance.rightControllerTriggerButton : ControllerInputPoller.instance.rightGrab)
            {
                if (platR == null)
                {
                    platR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platR.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    platR.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    platR.transform.localScale = new Vector3(0.0125f, 0.28f, 0.3825f); // Took scale from my other menu scaling hurts my head
                    platR.GetComponent<Renderer>().material.color = Settings.backgroundColor.GetCurrentColor();
                    platR.GetComponent<Renderer>().enabled = !invis;
                    if (Settings.stickyplats)
                        FixStickyColliders(platR);
                }
            }
            else
            {
                if (platR != null)
                {
                    GameObject.Destroy(platR);
                    platR = null;
                }
            }
            if (trigger ? ControllerInputPoller.instance.leftControllerTriggerButton : ControllerInputPoller.instance.leftGrab)
            {
                if (platL == null)
                {
                    platL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platL.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    platL.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                    platL.transform.localScale = new Vector3(0.0125f, 0.28f, 0.3825f); // Took scale from my other menu scaling hurts my head
                    platL.GetComponent<Renderer>().material.color = Settings.backgroundColor.GetCurrentColor();
                    platL.GetComponent<Renderer>().enabled = !invis;
                    if (Settings.stickyplats)
                        FixStickyColliders(platL);
                }
            }
            else
            {
                if (platL != null)
                {
                    GameObject.Destroy(platL);
                    platL = null;
                }
            }
        }

        public static void NoClip()
        {
            foreach (MeshCollider collider in GameObject.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None))
                collider.enabled = !ControllerInputPoller.instance.rightControllerTriggerButton;
        }

        public static void NoTagFreeze(bool notagfreeze) =>
            GTPlayer.Instance.disableMovement = !notagfreeze;

        public static void SteamLongArms() =>
            GTPlayer.Instance.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        public static void LongArms() =>
            GTPlayer.Instance.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        public static void DisableLongArms() =>
            GTPlayer.Instance.transform.localScale = new Vector3(1f, 1f, 1f);

        private static bool tp = false;
        public static void TPGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    if (!tp)
                    {
                        GTPlayer.Instance.transform.position = NewPointer.transform.position;
                        tp = true;
                    }
                }
                else
                    tp = false;
            }
        }

        public static void RigGun()
        {
            if (GetGunInput(false))
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                if (GetGunInput(true))
                {
                    VRRig.LocalRig.enabled = false;
                    VRRig.LocalRig.transform.position = NewPointer.transform.position;
                }
                else
                    VRRig.LocalRig.enabled = true;
            }
        }

        public static void GrabRig()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
            }
            if (ControllerInputPoller.instance.leftGrab)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                VRRig.LocalRig.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
            }
            if (!ControllerInputPoller.instance.rightGrab && !ControllerInputPoller.instance.leftGrab)
                VRRig.LocalRig.enabled = true;
        }


        public static void FixRig() =>
            VRRig.LocalRig.enabled = true;
        
        public static void GhostMonkey()
        {
            if (ControllerInputPoller.instance.rightControllerSecondaryButton)
                VRRig.LocalRig.enabled = false;
            else
                VRRig.LocalRig.enabled = true;
        }

        private static bool toggled;
        private static bool ghostButtonLast;
        private static bool invisButtonLast;
        public static void ToggleGhostMonkey()
        {
            bool button = ControllerInputPoller.instance.rightControllerSecondaryButton;
            if (button && !ghostButtonLast)
                VRRig.LocalRig.enabled = !VRRig.LocalRig.enabled;
            ghostButtonLast = button;
        }

        public static void InvisMonke()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                VRRig.LocalRig.enabled = false;
                VRRig.LocalRig.transform.position = new Vector3(3423f, 32432f, 324324f);
            }
            else
                VRRig.LocalRig.enabled = true;
        }
        public static void ToggleInvisMonkey()
        {
            bool button = ControllerInputPoller.instance.rightControllerPrimaryButton;
            if (button && !invisButtonLast)
            {
                bool enabled = !VRRig.LocalRig.enabled;
                VRRig.LocalRig.enabled = enabled;
                if (!enabled)
                    VRRig.LocalRig.transform.position = new Vector3(3423f, 32432f, 324324f);
            }
            invisButtonLast = button;
        }

        public static void SpazRig()
        {
            VRRig.LocalRig.head.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
            VRRig.LocalRig.leftHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
            VRRig.LocalRig.rightHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
        }
        public static void SpazHands()
        {
            VRRig.LocalRig.leftHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
            VRRig.LocalRig.rightHand.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
        }
        public static void SpazHead() =>
            VRRig.LocalRig.head.rigTarget.eulerAngles = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));

        public static void Rotate(Quaternion rot) =>
            VRRig.LocalRig.transform.rotation = rot;

        public static void FakeFBT()
        {
            Rotate(Camera.main.transform.rotation);
            VRRig.LocalRig.head.MapMine(VRRig.LocalRig.lastScaleFactor, VRRig.LocalRig.playerOffsetTransform);
            VRRig.LocalRig.leftHand.MapMine(VRRig.LocalRig.lastScaleFactor, VRRig.LocalRig.playerOffsetTransform);
            VRRig.LocalRig.rightHand.MapMine(VRRig.LocalRig.lastScaleFactor, VRRig.LocalRig.playerOffsetTransform);
        }
        public static void SpazBody() => Rotate(Random.rotation);


        static Vector3 normal2;
        static Vector3 vel1;
        static Vector3 vel2;
        static float dist2;
        static int layers;
        static bool LeftClose2;
        static bool DoOnce2;
        static float maxD2;
        public static void WallWalk()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                if (!DoOnce2)
                {
                    maxD2 = 1f;
                    layers = int.MaxValue;
                    DoOnce2 = true;
                }
                RaycastHit raycastHit;
                Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, -GorillaTagger.Instance.rightHandTransform.right, out raycastHit, 1f, layers);
                RaycastHit raycastHit2;
                Physics.Raycast(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.leftHandTransform.right, out raycastHit2, 1f, layers);
                if (raycastHit2.distance > raycastHit.distance)
                {
                    normal2 = raycastHit.normal;
                    dist2 = raycastHit.distance;
                }
                else
                {
                    normal2 = raycastHit2.normal;
                    dist2 = raycastHit2.distance;
                    LeftClose2 = true;
                }
                if (dist2 < maxD2)
                {
                    vel2 = normal2 * (8.8f * Time.deltaTime);
                    GorillaTagger.Instance.bodyCollider.attachedRigidbody.linearVelocity -= vel2;
                }
                else
                {
                    GorillaTagger.Instance.bodyCollider.attachedRigidbody.useGravity = true;
                }
            }
            else
            {
                GorillaTagger.Instance.bodyCollider.attachedRigidbody.useGravity = true;
            }
        }
    }
}