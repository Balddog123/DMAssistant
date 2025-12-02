using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NPC : ObservableObject
{
    private static float _monsterTypeRate = 0.5f;
    public static Dictionary<string, float> AvailableRaces = new Dictionary<string, float>
    {
        // Normal races
        { "Human", 50f },
        { "Aarakocra", 10f },
        { "Dwarf", 10f },
        { "Elf", 10f },
        { "Half-Orc", 10f },
        { "Halfling", 10f },
        { "Orc", 10f },
        { "Tiefling", 5f },

        // Monster Types (all low weight)
        { "Aberration", _monsterTypeRate },
        { "Beast", _monsterTypeRate },
        { "Celestial", _monsterTypeRate },
        { "Construct", _monsterTypeRate },
        { "Dragon", _monsterTypeRate },
        { "Elemental", _monsterTypeRate },
        { "Fey", _monsterTypeRate },
        { "Fiend", _monsterTypeRate },
        { "Giant", _monsterTypeRate },
        { "Humanoid", _monsterTypeRate },
        { "Monstrosity", _monsterTypeRate },
        { "Ooze", _monsterTypeRate },
        { "Plant", _monsterTypeRate },
        { "Undead", _monsterTypeRate },
    };
    public static float AvailableRacesTotalSum
    {
        get
        {
            return AvailableRaces.Values.Sum();
        }
    }

    public string Name { get; set; }
    public string ID { get; set; } = Guid.NewGuid().ToString();
    public string Race { get; set; }
    public string Description { get; set; }
    public string Goal { get; set; }
    private Location? _home;
    public Location? Home
    {
        get => _home;
        set => SetProperty(ref _home, value);   // must raise property changed
    }

    public NPC(string name, string race, string description, string goal)
    {
        Name = name;
        Race = race;
        Description = description;
        Goal = goal;
    }

    public static string GetRandomRace()
    {
        Random _random = new Random();
        float total = AvailableRaces.Values.Sum();
        float roll = (float)_random.NextDouble() * total;

        foreach (var kvp in AvailableRaces)
        {
            string race = kvp.Key;
            float weight = kvp.Value;

            if (roll < weight)
                return race;

            roll -= weight;
        }

        // Fallback (should never hit)
        return AvailableRaces.Keys.First();
    }
    public static Location GetRandomLocation()
    {
        List<Location> locations = App.CampaignStore.CurrentCampaign.Locations.ToList();
        if (locations.Count == 0) return null;
        Random _random = new Random();
        return locations[_random.Next(locations.Count)];
    }
}