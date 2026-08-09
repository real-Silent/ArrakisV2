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
    public static class Soundboard // Cleaned up code -nova
    {
        private static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();
        private static GameObject audioObject;
        private static AudioSource audioSource;
        public static bool LoopAudio = false;
        public static bool HearSelf = true;
        public static float LocalVolume = 0.2f;
        public static bool IsPlaying { get; private set; }
        public static void Play(string url)
        {
            if (!IsSupportedUrl(url))
                return;
            Stop();
            EnsureObject();
            CRunner.instance.StartCoroutine(Load(url));
        }
        private static IEnumerator Load(string url)
        {
            AudioClip clip;
            if (Cache.TryGetValue(url, out clip) && clip != null)
            {
                PlayClip(clip);
                yield break;
            }
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, GetAudioType(url)))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                    yield break;
                clip = DownloadHandlerAudioClip.GetContent(request);
                if (clip == null)
                    yield break;
                clip.name = "ArrakisClip";
                Cache[url] = clip;
            }
            PlayClip(clip);
        }
        private static void PlayClip(AudioClip clip)
        {
            if (clip == null)
                return;
            if (HearSelf)
                PlayLocal(clip);
            if (PhotonNetwork.InRoom)
                PlayPhoton(clip);
            IsPlaying = true;
        }
        private static void PlayPhoton(AudioClip clip)
        {
            try
            {
                Recorder recorder = GorillaTagger.Instance.myRecorder;
                if (recorder == null)
                    return;
                recorder.StopRecording();
                recorder.SourceType = Recorder.InputSourceType.AudioClip;
                recorder.AudioClip = clip;
                recorder.LoopAudioClip = LoopAudio;
                recorder.IsRecording = true;
                recorder.RestartRecording(true);
            }
            catch (Exception e)
            {
                CustomConsole.Log("Error playing soundboard through mic: " + e, CustomConsole.LogType.Error);
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
            catch { }
        }
        private static bool IsSupportedUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            string lower = url.ToLower();
            return lower.Contains("cdn.discordapp.com") || lower.Contains("media.discordapp.net") || lower.EndsWith(".mp3") ||  lower.EndsWith(".wav") || lower.EndsWith(".ogg");
        }
        private static AudioType GetAudioType(string url)
        {
            string ext = "";
            try { ext = Path.GetExtension(new Uri(url).AbsolutePath).ToLower(); }
            catch { }

            switch (ext)
            {
                case ".wav":
                    return AudioType.WAV;
                case ".ogg":
                    return AudioType.OGGVORBIS;
                default:
                    return AudioType.MPEG;
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