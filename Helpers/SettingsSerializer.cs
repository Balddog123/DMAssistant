using DMAssistant.Model;
using System;
using System.Collections.Generic;
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
        private static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "settings.json");
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

        public static Settings? LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsFilePath))
                    return null;

                string json = File.ReadAllText(SettingsFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonSerializer.Deserialize<Settings>(json, jsonOptions);
            }
            catch
            {
                // Corrupt file / bad JSON → treat as no campaign
                return null;
            }
        }

        public static void SaveSettings(Settings settings)
        {
            if (settings == null) return;

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFilePath)!);

            // Serialize and write
            string json = JsonSerializer.Serialize(settings, jsonOptions);
            File.WriteAllText(SettingsFilePath, json);
        }
    }
}
