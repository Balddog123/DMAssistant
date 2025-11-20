using DMAssistant.Model;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

public static class CampaignSerializer
{
    private static string FilePath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Campaigns", "campaign.json");
    private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals };

    public static Campaign? LoadCampaign()
    {
        try
        {
            if (!File.Exists(FilePath))
                return null;

            string json = File.ReadAllText(FilePath);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<Campaign>(json, jsonOptions);
        }
        catch
        {
            // Corrupt file / bad JSON → treat as no campaign
            return null;
        }
    }

    public static void SaveCampaign(Campaign campaign)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        string json = JsonSerializer.Serialize(campaign, jsonOptions);
        File.WriteAllText(FilePath, json);
    }
}
