using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Arrakis.Managers
{
    public class AudioManager
    {
        public static AudioClip buttonclick;

        public static void CacheSounds()
        {
            buttonclick = LoadAudioClip("smooth");
        }

        public static void Play(AudioClip clip, float volume = 1f)
        {
            GameObject holder = new GameObject();
            AudioSource source = holder.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.Play();
            GameObject.Destroy(holder, clip.length);
        }

        private static AudioClip LoadAudioClip(string soundName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(soundName));
            if (resourceName == null)
            {
                CustomConsole.Log($"Embedded resource '{soundName}' not found.", CustomConsole.LogType.Error);
                return null;
            }
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return null;
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return WavToAudioClip(data);
        }

        private static AudioClip WavToAudioClip(byte[] fileBytes)
        {
            const int headerSize = 44;
            if (fileBytes.Length < headerSize)
                return null;
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);
            int channels = BitConverter.ToInt16(fileBytes, 22);
            int dataSize = fileBytes.Length - headerSize;
            int sampleCount = dataSize / 2;
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                short sample = BitConverter.ToInt16(fileBytes, headerSize + (i * 2));
                samples[i] = sample / 32768f;
            }
            AudioClip clip = AudioClip.Create("sound", sampleCount / channels, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}