using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DMAssistant.Helpers
{
    public static class SettingsSerializer
    {
        private static string MainSettingsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "settings.json");
        private static string AudioSettingsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "audiosettings.json");
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };
        public enum SettingsType
        {
            Main,
            Audio
        }

        public static object? LoadSettings(SettingsType type)
        {
            string filePath = type switch
            {
                SettingsType.Main => MainSettingsPath,
                SettingsType.Audio => AudioSettingsPath,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };

            try
            {
                if (!File.Exists(filePath))
                    return null;

                string json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                if (type == SettingsType.Main) return JsonSerializer.Deserialize<MainSettings>(json, jsonOptions);
                if (type == SettingsType.Audio) return JsonSerializer.Deserialize<AudioSettings>(json, jsonOptions);
                else return null;
            }
            catch
            {
                // Corrupt file / bad JSON → treat as no campaign
                return null;
            }
        }

        public static void SaveSettings(object settings, SettingsType type)
        {
            Debug.WriteLine(settings);
            if (settings == null) return;

            string filePath = type switch
            {
                SettingsType.Main => MainSettingsPath,
                SettingsType.Audio => AudioSettingsPath,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            // Serialize and write
            string json = JsonSerializer.Serialize(settings, jsonOptions);
            File.WriteAllText(filePath, json);
        }
    }
}
