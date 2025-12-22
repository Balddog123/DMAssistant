using DMAssistant.Helpers;
using DMAssistant.Model;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

public static class CampaignSerializer
{
    public static string CampaignsFolderPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Campaigns");
    public static JsonSerializerOptions JsonOptions = new JsonSerializerOptions 
    { 
        WriteIndented = true, 
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals ,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };

    static CampaignSerializer()
    {
        JsonOptions.Converters.Add(new SafeEnumConverter<Item.ItemType>());
        JsonOptions.Converters.Add(new SafeEnumConverter<Item.ItemRank>());
        JsonOptions.Converters.Add(new LocationJsonConverter());
    }

    public static Campaign? LoadCampaign(string filePath)
    {
        Debug.WriteLine($"----LoadCampaign(string filePath)----");
        Debug.WriteLine($"Attempting to load file at filepath: {filePath}");

        try
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"File for campaign doesn't exist: {filePath}");
                return null;
            }
            Debug.WriteLine($"File exists...");

            string json = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.WriteLine($"File for campaign is null: {json}");
                return null;
            }

            return JsonSerializer.Deserialize<Campaign>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("=== JSON DESERIALIZATION ERROR ===");
            Debug.WriteLine(ex.ToString());
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
        string json = JsonSerializer.Serialize(campaign, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}
