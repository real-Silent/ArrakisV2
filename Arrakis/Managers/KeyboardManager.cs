using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Arrakis.Menu.Main;
using static Arrakis.Settings;

namespace Arrakis.Classes // nova forced me to do this -sleepy
{
    public class KeyboardManager : MonoBehaviour
    {
        public static KeyboardManager Instance;

        private GameObject keyboard;
        private GameObject searchMenu;
        private TextMeshPro searchText;

        private readonly List<GameObject> results = new List<GameObject>();

        private string search = "";

        private const float SpawnDistance = 0.75f;
        private const float MoveDistance = 1.5f;

        private readonly string[] rows =
        {
            "1234567890",
            "QWERTYUIOP",
            "ASDFGHJKL",
            "ZXCVBNM"
        };

        public bool IsOpen => keyboard != null;

        public static void Toggle()
        {
            if (Instance == null)
            {
                GameObject obj = new GameObject("KeyboardManager");
                DontDestroyOnLoad(obj);
                Instance = obj.AddComponent<KeyboardManager>();
            }

            if (Instance.IsOpen)
                Instance.Close();
            else
                Instance.Open();
        }

        private void Open()
        {
            if (keyboard != null)
                return;

            keyboard = new GameObject("Keyboard");

            searchMenu = new GameObject("SearchResults");
            searchMenu.transform.SetParent(keyboard.transform);
            searchMenu.transform.localPosition = Vector3.zero;
            searchMenu.transform.localRotation = Quaternion.identity;

            MoveKeyboard();

            CreateSearch();
            CreateKeys();
        }

        private void Close()
        {
            if (keyboard != null)
                Destroy(keyboard);

            keyboard = null;
            searchMenu = null;
            searchText = null;

            results.Clear();
            search = "";
        }

        private void Update()
        {
            if (keyboard == null)
                return;

            if (GorillaTagger.Instance == null ||
                GorillaTagger.Instance.offlineVRRig == null)
                return;

            Transform player =
                GorillaTagger.Instance.offlineVRRig.transform;

            if (Vector3.Distance(
                    player.position,
                    keyboard.transform.position
                ) > MoveDistance)
            {
                MoveKeyboard();
            }

            ReadDesktopInput();
        }

        private void MoveKeyboard()
        {
            Transform player =
                GorillaTagger.Instance.offlineVRRig.transform;

            Vector3 forward = player.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.01f)
                return;

            forward.Normalize();

            keyboard.transform.position =
                player.position +
                forward * SpawnDistance +
                Vector3.up * 0.08f;

            keyboard.transform.rotation =
                Quaternion.LookRotation(
                    -forward,
                    Vector3.up
                );
        }

        private void ReadDesktopInput()
        {
            Keyboard input = Keyboard.current;

            if (input == null)
                return;

            if (input.escapeKey.wasPressedThisFrame)
            {
                Close();
                return;
            }

            if (input.backspaceKey.wasPressedThisFrame)
                RemoveCharacter();

            if (input.spaceKey.wasPressedThisFrame)
                AddCharacter(" ");

            if (input.enterKey.wasPressedThisFrame)
                Search();

            CheckLetters(input);
            CheckNumbers(input);
        }

        private void CheckLetters(Keyboard input)
        {
            if (input.aKey.wasPressedThisFrame)
                AddCharacter("A");
            if (input.bKey.wasPressedThisFrame)
                AddCharacter("B");
            if (input.cKey.wasPressedThisFrame)
                AddCharacter("C");
            if (input.dKey.wasPressedThisFrame)
                AddCharacter("D");
            if (input.eKey.wasPressedThisFrame)
                AddCharacter("E");
            if (input.fKey.wasPressedThisFrame)
                AddCharacter("F");
            if (input.gKey.wasPressedThisFrame)
                AddCharacter("G");
            if (input.hKey.wasPressedThisFrame)
                AddCharacter("H");
            if (input.iKey.wasPressedThisFrame)
                AddCharacter("I");
            if (input.jKey.wasPressedThisFrame)
                AddCharacter("J");
            if (input.kKey.wasPressedThisFrame)
                AddCharacter("K");
            if (input.lKey.wasPressedThisFrame)
                AddCharacter("L");
            if (input.mKey.wasPressedThisFrame)
                AddCharacter("M");
            if (input.nKey.wasPressedThisFrame)
                AddCharacter("N");
            if (input.oKey.wasPressedThisFrame)
                AddCharacter("O");
            if (input.pKey.wasPressedThisFrame)
                AddCharacter("P");
            if (input.qKey.wasPressedThisFrame)
                AddCharacter("Q");
            if (input.rKey.wasPressedThisFrame)
                AddCharacter("R");
            if (input.sKey.wasPressedThisFrame)
                AddCharacter("S");
            if (input.tKey.wasPressedThisFrame)
                AddCharacter("T");
            if (input.uKey.wasPressedThisFrame)
                AddCharacter("U");
            if (input.vKey.wasPressedThisFrame)
                AddCharacter("V");
            if (input.wKey.wasPressedThisFrame)
                AddCharacter("W");
            if (input.xKey.wasPressedThisFrame)
                AddCharacter("X");
            if (input.yKey.wasPressedThisFrame)
                AddCharacter("Y");
            if (input.zKey.wasPressedThisFrame)
                AddCharacter("Z");
        }

        private void CheckNumbers(Keyboard input)
        {
            if (input.digit0Key.wasPressedThisFrame)
                AddCharacter("0");
            if (input.digit1Key.wasPressedThisFrame)
                AddCharacter("1");
            if (input.digit2Key.wasPressedThisFrame)
                AddCharacter("2");
            if (input.digit3Key.wasPressedThisFrame)
                AddCharacter("3");
            if (input.digit4Key.wasPressedThisFrame)
                AddCharacter("4");
            if (input.digit5Key.wasPressedThisFrame)
                AddCharacter("5");
            if (input.digit6Key.wasPressedThisFrame)
                AddCharacter("6");
            if (input.digit7Key.wasPressedThisFrame)
                AddCharacter("7");
            if (input.digit8Key.wasPressedThisFrame)
                AddCharacter("8");
            if (input.digit9Key.wasPressedThisFrame)
                AddCharacter("9");
        }

        private void AddCharacter(string character)
        {
            search += character;
            UpdateSearch();
        }

        private void RemoveCharacter()
        {
            if (search.Length == 0)
                return;

            search = search.Substring(
                0,
                search.Length - 1
            );

            UpdateSearch();
        }

        private void UpdateSearch()
        {
            if (searchText != null)
            {
                searchText.text =
                    string.IsNullOrEmpty(search)
                        ? "_"
                        : search;
            }

            Search();
        }

        private void CreateSearch()
        {
            GameObject obj =
                new GameObject("SearchText");

            obj.transform.SetParent(
                keyboard.transform
            );

            obj.transform.localPosition =
                new Vector3(
                    0f,
                    0.25f,
                    -0.01f
                );

            obj.transform.localRotation =
                Quaternion.identity;

            obj.transform.localScale =
                Vector3.one * 0.025f;

            searchText =
                obj.AddComponent<TextMeshPro>();

            searchText.fontSize = 3.2f;
            searchText.alignment =
                TextAlignmentOptions.Center;

            searchText.color = Color.white;

            searchText.rectTransform.sizeDelta =
                new Vector2(
                    500f,
                    50f
                );

            searchText.text = "_";
        }

        private void CreateKeys()
        {
            for (int row = 0; row < rows.Length; row++)
            {
                string rowText = rows[row];

                float spacing = 0.068f;

                float start =
                    -((rowText.Length - 1) * spacing) / 2f;

                for (int i = 0; i < rowText.Length; i++)
                {
                    CreateKey(
                        rowText[i].ToString(),
                        new Vector3(
                            start + i * spacing,
                            0.07f - row * spacing,
                            0f
                        )
                    );
                }
            }

            CreateKey(
                "SPACE",
                new Vector3(
                    -0.18f,
                    -0.21f,
                    0f
                ),
                0.19f
            );

            CreateKey(
                "BACK",
                new Vector3(
                    0.035f,
                    -0.21f,
                    0f
                ),
                0.095f
            );

            CreateKey(
                "CLEAR",
                new Vector3(
                    0.145f,
                    -0.21f,
                    0f
                ),
                0.095f
            );

            CreateKey(
                "ENTER",
                new Vector3(
                    0.255f,
                    -0.21f,
                    0f
                ),
                0.095f
            );
        }

        private void CreateKey(
            string text,
            Vector3 position,
            float width = 0.05f)
        {
            GameObject key =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cube
                );

            key.name = "Key_" + text;

            key.transform.SetParent(
                keyboard.transform
            );

            key.transform.localPosition =
                position;

            key.transform.localRotation =
                Quaternion.identity;

            key.transform.localScale =
                new Vector3(
                    width,
                    0.042f,
                    0.023f
                );

            Renderer renderer =
                key.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color =
                    new Color(
                        0.13f,
                        0.13f,
                        0.13f
                    );
            }

            GameObject label =
                new GameObject("Text");

            label.transform.SetParent(
                key.transform
            );

            label.transform.localPosition =
                new Vector3(
                    0f,
                    0f,
                    -0.51f
                );

            label.transform.localRotation =
                Quaternion.identity;

            label.transform.localScale =
                Vector3.one * 0.025f;

            TextMeshPro textMesh =
                label.AddComponent<TextMeshPro>();

            textMesh.text = text;
            textMesh.fontSize = 3.2f;

            textMesh.alignment =
                TextAlignmentOptions.Center;

            textMesh.color = Color.white;

            textMesh.rectTransform.sizeDelta =
                new Vector2(
                    500f,
                    100f
                );

            KeyboardKey button =
                key.AddComponent<KeyboardKey>();

            button.key = text;
        }

        private void Search()
        {
            ClearResults();

            if (string.IsNullOrWhiteSpace(search))
                return;

            string query =
                StripTags(search).Trim();

            if (Arrakis.Menu.Buttons.buttons == null)
                return;

            foreach (
                ButtonInfo[] category
                in Arrakis.Menu.Buttons.buttons)
            {
                if (category == null)
                    continue;

                foreach (ButtonInfo button in category)
                {
                    if (button == null ||
                        string.IsNullOrEmpty(
                            button.buttonText))
                        continue;

                    string name =
                        StripTags(
                            button.buttonText
                        );

                    if (name.IndexOf(
                            query,
                            StringComparison.OrdinalIgnoreCase
                        ) == -1)
                        continue;

                    CreateResult(button);

                    if (results.Count >= 6)
                        return;
                }
            }
        }

        private void CreateResult(ButtonInfo button)
        {
            int index = results.Count;

            GameObject result =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cube
                );

            result.name =
                "Result_" +
                StripTags(button.buttonText);

            result.transform.SetParent(
                searchMenu.transform
            );

            result.transform.localPosition =
                new Vector3(
                    0f,
                    0.36f +
                    index * 0.065f,
                    0f
                );

            result.transform.localRotation =
                Quaternion.identity;

            result.transform.localScale =
                new Vector3(
                    0.52f,
                    0.043f,
                    0.024f
                );

            Renderer renderer =
                result.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color =
                    new Color(
                        0.11f,
                        0.11f,
                        0.11f
                    );
            }

            GameObject label =
                new GameObject("Text");

            label.transform.SetParent(
                result.transform
            );

            label.transform.localPosition =
                new Vector3(
                    0f,
                    0f,
                    -0.51f
                );

            label.transform.localRotation =
                Quaternion.identity;

            label.transform.localScale =
                Vector3.one * 0.022f;

            TextMeshPro text =
                label.AddComponent<TextMeshPro>();

            text.text =
                StripTags(
                    button.buttonText
                );

            text.fontSize = 3f;

            text.alignment =
                TextAlignmentOptions.Center;

            text.color = Color.white;

            text.rectTransform.sizeDelta =
                new Vector2(
                    500f,
                    100f
                );

            SearchResult resultButton =
                result.AddComponent<SearchResult>();

            resultButton.button = button;

            results.Add(result);
        }

        private void ClearResults()
        {
            foreach (GameObject result in results)
            {
                if (result != null)
                    Destroy(result);
            }

            results.Clear();
        }

        public void PressKey(string key)
        {
            switch (key)
            {
                case "BACK":
                    RemoveCharacter();
                    break;

                case "CLEAR":
                    search = "";
                    UpdateSearch();
                    break;

                case "SPACE":
                    AddCharacter(" ");
                    break;

                case "ENTER":
                    Search();
                    break;

                default:
                    AddCharacter(key);
                    break;
            }
        }

        public void Select(ButtonInfo button)
        {
            if (button == null)
                return;

            try
            {
                if (button.method != null)
                    button.method();
            }
            catch (Exception e)
            {
                Debug.LogError(
                    "[Arrakis] " + e
                );
            }
        }

        private string StripTags(string text)
        {
            return Regex.Replace(
                text ?? "",
                "<.*?>",
                ""
            );
        }

        private void OnDestroy()
        {
            if (keyboard != null)
                Destroy(keyboard);

            if (Instance == this)
                Instance = null;
        }
    }

    public class KeyboardKey : MonoBehaviour
    {
        public string key;

        private float cooldown;

        private void OnTriggerEnter(
            Collider collider)
        {
            if (Time.time < cooldown)
                return;

            if (collider != buttonCollider &&
                collider != leftButtonCollider &&
                collider != rightButtonCollider)
                return;

            cooldown = Time.time + 0.2f;

            if (!disablevibrations)
            {
                GorillaTagger.Instance.StartVibration(
                    rightHanded,
                    GorillaTagger.Instance.tagHapticStrength / 2f,
                    GorillaTagger.Instance.tagHapticDuration / 2f
                );
            }

            VRRig.LocalRig.PlayHandTapLocal(
                buttonsound,
                rightHanded,
                buttonclickvolume
            );

            KeyboardManager.Instance?.PressKey(
                key
            );
        }
    }

    public class SearchResult : MonoBehaviour
    {
        public ButtonInfo button;

        private float cooldown;

        private void OnTriggerEnter(
            Collider collider)
        {
            if (Time.time < cooldown)
                return;

            if (collider != buttonCollider &&
                collider != leftButtonCollider &&
                collider != rightButtonCollider)
                return;

            cooldown = Time.time + 0.2f;

            if (!disablevibrations)
            {
                GorillaTagger.Instance.StartVibration(
                    rightHanded,
                    GorillaTagger.Instance.tagHapticStrength / 2f,
                    GorillaTagger.Instance.tagHapticDuration / 2f
                );
            }

            VRRig.LocalRig.PlayHandTapLocal(
                buttonsound,
                rightHanded,
                buttonclickvolume
            );

            KeyboardManager.Instance?.Select(
                button
            );
        }
    }
}