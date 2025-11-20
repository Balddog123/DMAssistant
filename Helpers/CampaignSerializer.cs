using DMAssistant.Model;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

public static class CampaignSerializer
{
    public static string CampaignsFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Campaigns");
    private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

    public static Campaign? LoadCampaign(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"File for campaign doesn't exist: {filePath}");
                return null;
            }

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.WriteLine($"File for campaign is null: {json}");
                return null;
            }

            return JsonSerializer.Deserialize<Campaign>(json, jsonOptions);
        }
        catch
        {
            // Corrupt file / bad JSON → treat as no campaign
            Debug.WriteLine($"Could not load campaign from {filePath}");
            return null;
        }
    }

    public static void SaveCampaign(Campaign campaign)
    {
        if (campaign == null) return;

        // Full file path
        string filePath = Path.Combine(CampaignsFolderPath, campaign.Name + ".json");

        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        // Serialize and write
        string json = JsonSerializer.Serialize(campaign, jsonOptions);
        File.WriteAllText(filePath, json);
    }
}
