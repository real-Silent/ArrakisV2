/*
 * Arrakis | Managers/FileManager.cs
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

using System.IO;

namespace Arrakis.Managers
{
    public class FileManager
    {
        public static string[] GetFiles(string path) =>
            Directory.GetFiles(path);
        public static string[] GetFilesWithSubdirectories(string path) =>
            Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        public static bool FileExists(string path) =>
            File.Exists(path);
        public static bool DeleteFile(string path, string filename)
        {
            string filePath = Path.Combine(path, filename);
            if (!File.Exists(filePath))
            {
                return false;
            }
            File.Delete(filePath);
            return true;
        }
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static bool DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            Directory.Delete(path, true);
            return true;
        }
        public static bool CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            if (!File.Exists(sourcePath))
            {
                return false;
            }
            File.Copy(sourcePath, destinationPath, overwrite);
            return true;
        }
        public static bool MoveFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            if (!File.Exists(sourcePath))
            {
                return false;
            }
            if (overwrite && File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }
            File.Move(sourcePath, destinationPath);
            return true;
        }
        public static string ReadFile(string path) =>
            File.ReadAllText(path);
        public static void WriteFile(string path, string content) =>
            File.WriteAllText(path, content);
        public static void AppendFile(string path, string content) =>
            File.AppendAllText(path, content);
        public static string GetFileName(string path) =>
            Path.GetFileName(path);
        public static string GetExtension(string path) =>
            Path.GetExtension(path);
    }
}