using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static Arrakis.Menu.Main;

namespace Arrakis.Managers
{
    public static class AudioManager
    {
        private const string ResourcePrefix = "Arrakis.Resources.Audio.";
        private static readonly Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<int, string> customSoundNames = new Dictionary<int, string>();
        private static AudioSource audioSource;
        public static int buttonsound = 67;
        public static int clicksound = 0;
        public static AudioClip currentAudio;

        private static readonly string[] SoundNames =
        {
            "smooth",
            "creamy",
            "windows"
        };
        private static readonly int[] ButtonSounds =
        {
            67, 66, 8, 84, 32, 106, 189, 22, 43, 210, 217
        };

        public static void Init() => 
            LoadAllSounds();
        public static void LoadAllSounds()
        {
            for (int i = 0; i < SoundNames.Length; i++)
                LoadSound(SoundNames[i]);
        }
        public static void PlaySound(string soundName, float volume = 0.5f)
        {
            if (string.IsNullOrEmpty(soundName)) return;
            AudioClip clip = LoadSound(soundName);
            if (clip == null) return;
            AudioSource source = GetAudioSource();
            if (source == null) return;
            source.PlayOneShot(clip, volume);
        }
        public static void MenuSound(string soundName) => 
            PlaySound(soundName, 0.5f);
        public static AudioClip LoadSoundFromFile(string filePath, string cacheAlias = null)
        {
            if (string.IsNullOrEmpty(filePath))
                return null;
            string alias = string.IsNullOrEmpty(cacheAlias) ? Path.GetFileNameWithoutExtension(filePath) : cacheAlias;
            if (audioClips.TryGetValue(alias, out AudioClip cached)) return cached;
            if (!File.Exists(filePath)) return null;
            byte[] bytes;
            try { bytes = File.ReadAllBytes(filePath); }
            catch { return null; }
            AudioClip clip = WavToAudioClip(bytes);
            if (clip == null)
                return null;
            clip.name = $"Arrakis_{alias}";
            audioClips[alias] = clip;
            return clip;
        }
        public static void PlaySoundFromFile(string filePath, float volume = 0.5f, string cacheAlias = null)
        {
            AudioClip clip = LoadSoundFromFile(filePath, cacheAlias);
            if (clip == null)
                return;
            AudioSource source = GetAudioSource();
            if (source == null)
                return;
            source.PlayOneShot(clip, volume);
        }
        public static bool IsLoaded(string name) =>
            !string.IsNullOrEmpty(name) && audioClips.ContainsKey(name);
        public static AudioClip LoadSound(string soundName)
        {
            if (string.IsNullOrEmpty(soundName))
                return null;
            if (audioClips.TryGetValue(soundName, out AudioClip cached))
                return cached;
            byte[] soundBytes = LoadEmbeddedBytes(soundName);
            if (soundBytes == null)
                return null;
            AudioClip clip = WavToAudioClip(soundBytes);
            if (clip == null)
                return null;
            clip.name = $"Arrakis_{soundName}";
            audioClips[soundName] = clip;
            return clip;
        }
        public static void PlayButtonSound(bool lefthand, float volume)
        {
            if (clicksound < ButtonSounds.Length)
            {
                PlayRigAudio(buttonsound, lefthand, volume);
                return;
            }
            if (currentAudio != null)
            {
                AudioSource source = GetAudioSource();
                if (source != null)
                    source.PlayOneShot(currentAudio, volume);
            }
        }
        public static void ChangeClickSound()
        {
            int totalSlots = ButtonSounds.Length + SoundNames.Length;
            clicksound++;
            if (clicksound >= totalSlots) clicksound = 0;
            if (clicksound < ButtonSounds.Length)
            {
                buttonsound = ButtonSounds[clicksound];
                currentAudio = null;
            }
            else
            {
                int embedIndex = clicksound - ButtonSounds.Length;
                string soundName = SoundNames[embedIndex];
                currentAudio = LoadSound(soundName);
                customSoundNames[embedIndex] = soundName;
            }
            UpdateClickSoundText();
        }
        public static void PlayRigAudio(int index, bool lefthand, float volume)
        {
            if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null) return;
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(index, lefthand, volume);
        }
        public static void ClearCache()
        {
            foreach (AudioClip clip in audioClips.Values)
                if (clip != null) UnityEngine.Object.Destroy(clip);
            audioClips.Clear();
            customSoundNames.Clear();
            currentAudio = null;
        }
        private static AudioSource GetAudioSource()
        {
            if (audioSource != null)
                return audioSource;
            if (GorillaTagger.Instance == null || GorillaTagger.Instance.offlineVRRig == null)
                return null;
            GameObject obj = GorillaTagger.Instance.offlineVRRig.gameObject;
            audioSource = obj.GetComponent<AudioSource>() ?? obj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            return audioSource;
        }
        private static byte[] LoadEmbeddedBytes(string soundName)
        {
            string resourcePath = $"{ResourcePrefix}{soundName}.wav";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null) return null;
                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
        private static AudioClip WavToAudioClip(byte[] fileBytes)
        {
            if (fileBytes == null || fileBytes.Length < 44)
                return null;
            if (fileBytes[0] != 'R' || fileBytes[1] != 'I' || fileBytes[2] != 'F' || fileBytes[3] != 'F')
                return null;
            if (fileBytes[8] != 'W' || fileBytes[9] != 'A' || fileBytes[10] != 'V' || fileBytes[11] != 'E')
                return null;
            int channels = BitConverter.ToInt16(fileBytes, 22);
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);
            int bitsPerSample = BitConverter.ToInt16(fileBytes, 34);
            if (channels <= 0 || sampleRate <= 0)
                return null;
            if (bitsPerSample != 16)
                return null;
            int dataPosition = FindDataChunk(fileBytes);
            if (dataPosition < 0)
                return null;
            int dataSize = BitConverter.ToInt32(fileBytes, dataPosition + 4);
            int dataStart = dataPosition + 8;
            if (dataSize <= 0 || dataStart + dataSize > fileBytes.Length)
                return null;
            int sampleCount = dataSize / 2;
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short s = BitConverter.ToInt16(fileBytes, dataStart + (i * 2));
                samples[i] = s / 32768f;
            }
            int frameCount = sampleCount / channels;
            AudioClip clip = AudioClip.Create("ArrakisAudio", frameCount, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
        private static int FindDataChunk(byte[] bytes)
        {
            int pos = 12;
            while (pos + 8 <= bytes.Length)
            {
                if (bytes[pos] == 'd' && bytes[pos + 1] == 'a' && bytes[pos + 2] == 't' && bytes[pos + 3] == 'a')
                    return pos;
                int chunkSize = BitConverter.ToInt32(bytes, pos + 4);
                if (chunkSize < 0) return -1;
                pos += 8 + chunkSize;
                if ((chunkSize & 1) != 0) pos++;
            }
            return -1;
        }
        private static void UpdateClickSoundText()
        {
            var btn = GetIndex("Change Click Sound");
            if (btn == null) 
                return;
            string name;
            if (clicksound < ButtonSounds.Length)
            {
                name = buttonsound.ToString();
                if (GTPlayer.Instance?.materialData != null && buttonsound >= 0 && buttonsound < GTPlayer.Instance.materialData.Count)
                    name = GTPlayer.Instance.materialData[buttonsound].matName;
            }
            else
            {
                int customIndex = clicksound - ButtonSounds.Length;
                if (customSoundNames.TryGetValue(customIndex, out string registered))
                {
                    name = registered;
                }
                else if (currentAudio != null)
                {
                    name = currentAudio.name.StartsWith("Arrakis_") ? currentAudio.name.Substring(8) : currentAudio.name;
                }
                else
                {
                    name = "Unknown";
                }
            }
            btn.overlapText = $"Change Click Sound <color=grey>[<color=cyan>{name}</color>]</color>";
        }
    }
}