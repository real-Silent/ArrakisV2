/*
 * Arrakis | Classes/Server/UserCount.cs
 *
 * Copyright (C) 2026 Arrakis
 * https://github.com/real-Silent/Arrakis
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
using Newtonsoft.Json.Linq;
using PlayFab;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Arrakis.Classes.Menu
{
    public class UserCount : MonoBehaviour
    {
        public static int CurrentUsers;
        private void Start()
        {
            StartCoroutine(UpdateHeartbeat());
        }
        private IEnumerator UpdateHeartbeat()
        {
            while (true)
            {
                if (GorillaComputer.instance.isConnectedToMaster)
                {
                    yield return Send();
                    yield return GetUserCount();
                }
                yield return new WaitForSeconds(10f);
            }
        }
        private IEnumerator Send()
        {
            JObject body = new JObject
            {
                ["hwid"] = PlayFabSettings.DeviceUniqueIdentifier
            };
            string json = body.ToString();
            byte[] data = Encoding.UTF8.GetBytes(json);
            using (UnityWebRequest request = new UnityWebRequest($"{PluginInfo.ServerApi}/updatecount", "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(data);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    CustomConsole.Log($"Post failed unable to update usercount: {request.error}", CustomConsole.LogType.Error);
                    yield break;
                }
            }
        }
        private IEnumerator GetUserCount()
        {
            string url = $"{PluginInfo.ServerApi}/getusercount";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                if (request.result != UnityWebRequest.Result.Success)
                {
                    CustomConsole.Log($"Request failed: {request.error}", CustomConsole.LogType.Error);
                    yield break;
                }
                JObject json = JObject.Parse(request.downloadHandler.text);
                CurrentUsers = json["users"]?.Value<int>() ?? 0;
            }
        }
    }
}