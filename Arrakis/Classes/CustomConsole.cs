using UnityEngine;

namespace Arrakis
{
    public class CustomConsole
    {
        public enum LogType
        {
            Info, Debug, Warning, Error
        }
        private static string type = "";
        public static void Log(string message, LogType logType = LogType.Info, string prefix = "ARRAKIS")
        {
            if (string.IsNullOrEmpty(message))
                return;
            switch (logType)
            {
                case LogType.Info: type = "Info"; break;
                case LogType.Debug: type = "Debug"; break;
                case LogType.Warning: type = "Warning"; break;
                case LogType.Error: type = "Error"; break;
            }
            Debug.Log($"[{prefix}] {type}: {message}");
        }
        public static void LogNoType(string message) => 
            Debug.Log(message);

        private static string ascii = @$"
              ___                 _    _     
             / _ \               | |  (_)    
            / /_\ \_ __ _ __ __ _| | ___ ___ 
            |  _  | '__| '__/ _` | |/ / / __|
            | | | | |  | | | (_| |   <| \__ \
            \_| |_/_|  |_|  \__,_|_|\_\_|___/
                             
                     Version: {PluginInfo.Version} 
                    By Nova, Sleepy
                                 
";
        public static void LoadStart()
        {
            Log("Thank you for using Arrakis.", LogType.Info);
            LogNoType(ascii);
        }
    }
}