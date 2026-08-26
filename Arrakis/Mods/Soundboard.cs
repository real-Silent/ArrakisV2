 /*
 * Arrakis | Mods/Soundboard.cs
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Arrakis.Classes;
using Photon.Pun;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace Arrakis.Mods
{
    public static class Soundboard
    {
        private static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();
        private static GameObject audioObject;
        private static AudioSource audioSource;

        public static bool LoopAudio = false;
        public static bool HearSelf = true;
        public static float LocalVolume = 0.2f;
        public static bool IsPlaying { get; private set; }

        private static string SoundsPath
        {
            get { return Path.Combine(Application.dataPath, "..", "Arrakis", "Sounds"); }
        }

        public static void Play(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            fileName = Path.GetFileName(fileName);

            if (!IsSupportedFile(fileName))
                return;

            string path = Path.Combine(SoundsPath, fileName);

            if (!File.Exists(path))
            {
                CustomConsole.Log("Sound not found: " + path, CustomConsole.LogType.Error);
                return;
            }

            Stop();
            EnsureObject();

            CRunner.instance.StartCoroutine(Load(path));
        }

        private static IEnumerator Load(string path)
        {
            string fileName = Path.GetFileName(path);
            AudioClip clip;

            if (Cache.TryGetValue(fileName, out clip) && clip != null)
            {
                PlayClip(clip);
                yield break;
            }

            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(new Uri(path).AbsoluteUri, GetAudioType(path)))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    CustomConsole.Log("Failed to load sound: " + request.error, CustomConsole.LogType.Error);
                    yield break;
                }

                clip = DownloadHandlerAudioClip.GetContent(request);

                if (clip == null)
                    yield break;

                clip.name = "ArrakisClip";
                Cache[fileName] = clip;
            }

            PlayClip(clip);
        }

        private static void PlayClip(AudioClip clip)
        {
            if (clip == null)
                return;

            if (HearSelf)
                PlayLocal(clip);

            if (NetworkSystem.Instance.InRoom)
                PlayPhoton(clip);

            IsPlaying = true;
        }

        private static void PlayPhoton(AudioClip clip)
        {
            try
            {
                Recorder recorder = GorillaTagger.Instance.myRecorder;

                if (recorder == null)
                {
                    CustomConsole.Log("Soundboard: Recorder is null. (fucking what)", CustomConsole.LogType.Error);
                    return;
                }
                recorder.StopRecording();

                recorder.AudioClip = clip;
                recorder.LoopAudioClip = LoopAudio;
                recorder.SourceType = Recorder.InputSourceType.AudioClip;

                recorder.RestartRecording(true);
            }
            catch (Exception e)
            {
                CustomConsole.Log("Soundboard: Photon playback error: " + e, CustomConsole.LogType.Error);
            }
        }
        private static void PlayLocal(AudioClip clip)
        {
            EnsureObject();

            audioSource.clip = clip;
            audioSource.loop = LoopAudio;
            audioSource.volume = Mathf.Clamp(LocalVolume, 0f, 5f);
            audioSource.Play();
        }

        public static void Stop()
        {
            IsPlaying = false;

            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }

            try
            {
                if (PhotonNetwork.InRoom)
                {
                    Recorder recorder = GorillaTagger.Instance.myRecorder;

                    if (recorder != null)
                    {
                        recorder.SourceType = Recorder.InputSourceType.Microphone;
                        recorder.IsRecording = false;
                        recorder.AudioClip = null;
                        recorder.RestartRecording(true);
                    }
                }
            }
            catch
            {
            }
        }

        private static bool IsSupportedFile(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension == ".mp3" ||
                   extension == ".wav" ||
                   extension == ".ogg";
        }
        public static void StartSoundboard()
        {
            Arrakis.Menu.Main.CurrentCategoryName = "Soundboard";

            string soundsPath = Path.Combine(Environment.CurrentDirectory, "Arrakis", "Sounds");

            List<ButtonInfo> buttons = new List<ButtonInfo>();

            buttons.Add(new ButtonInfo
            {
                buttonText = "Exit Soundboard",
                method = () => Arrakis.Menu.Main.CurrentCategoryName = "Main",
                isTogglable = false,
                toolTip = "Returns to the main page for the menu."
            });

            if (!Directory.Exists(soundsPath))
                Directory.CreateDirectory(soundsPath);

            foreach (string file in Directory.GetFiles(soundsPath))
            {
                string extension = Path.GetExtension(file).ToLowerInvariant();

                if (extension != ".mp3" &&
                    extension != ".wav" &&
                    extension != ".ogg")
                    continue;

                string fileName = Path.GetFileName(file);

                ButtonInfo soundButton = new ButtonInfo
                {
                    buttonText = Path.GetFileNameWithoutExtension(file),
                    enableMethod =() => Soundboard.Play(fileName),
                    disableMethod =() => Soundboard.Stop(),
                    isTogglable = true,
                    toolTip = "Play " + Path.GetFileNameWithoutExtension(file)
                };

                buttons.Add(soundButton);
            }

            Arrakis.Menu.Buttons.buttons[Array.IndexOf( Arrakis.Menu.Buttons.categoryNames, "Soundboard")] = buttons.ToArray();
        }
        private static AudioType GetAudioType(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            switch (extension)
            {
                case ".wav":
                    return AudioType.WAV;
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".mp3":
                    return AudioType.MPEG;
                default:
                    return AudioType.UNKNOWN;
            }
        }

        private static void EnsureObject()
        {
            if (audioObject != null)
                return;

            audioObject = new GameObject("Arrakis Soundboard");
            UnityEngine.Object.DontDestroyOnLoad(audioObject);

            audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        public static void ClearCache()
        {
            foreach (AudioClip clip in Cache.Values)
            {
                if (clip != null)
                    UnityEngine.Object.Destroy(clip);
            }

            Cache.Clear();
        }
    }
}