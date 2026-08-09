/*
 * Arrakis | Managers/AudioManager.cs
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
using System.Reflection;
using UnityEngine;

namespace Arrakis.Managers
{
    public class AudioManager
    {
        public static void MenuSound(string soundname) =>
            PlaySound(soundname);
        private static void PlaySound(string soundName, float volume = 0.5f)
        {
            if (GetAudioClip(soundName) == null)
                return;
            AudioSource source = GorillaTagger.Instance.offlineVRRig.gameObject.AddComponent<AudioSource>();
            source.clip = GetAudioClip(soundName);
            source.volume = volume;
            source.loop = false;
            source.Play();
            UnityEngine.Object.Destroy(source, GetAudioClip(soundName).length + 0.1f);
        }

        private static AudioClip GetAudioClip(string resourceName)
        {
            byte[] soundBytes = LoadEmbeddedSounds(resourceName);
            if (soundBytes == null)
                return null;
            AudioClip clip = WavToAudioClip(soundBytes);
            if (clip == null)
                return null;
            return clip;
        }
        private static byte[] LoadEmbeddedSounds(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Arrakis.Resources.Audio.{resourceName}.wav"))
            {
                if (stream == null)
                    return null;
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
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