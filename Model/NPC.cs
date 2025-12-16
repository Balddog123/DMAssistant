using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Standard;
using DMAssistant;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DMAssistant.Model.NameRow;

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
        { "Goblinoid", _monsterTypeRate },
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
    private static Dictionary<NameRow.NameComponent, List<NameRow>> NameTables { get; }
    = new Dictionary<NameRow.NameComponent, List<NameRow>> {
        { NameRow.NameComponent.Prefix, new List<NameRow>{
            new NameRow("Al", "Al"),
            new NameRow("El", "El"),
            new NameRow("Il", "Il"),
            new NameRow("Brae", "Brae"),
            new NameRow("Bran", "Bryn"),
            new NameRow("Cael", "Cael"),
            new NameRow("Dorn", "Dan"),
            new NameRow("Eron", "Elya"),
            new NameRow("Fir", "Fel"),
            new NameRow("Gren", "Grae"),
            new NameRow("Hur", "Han"),
            new NameRow("Jar", "Jin"),
            new NameRow("Kael", "Kaela"),
            new NameRow("Lor", "Lora"),
            new NameRow("Maer", "Maera"),
            new NameRow("Nor", "Nor"),
            new NameRow("Or", "Om"),
            new NameRow("Paer", "Pyr"),
            new NameRow("Quin", "Qual"),
            new NameRow("Ron", "Ros"),
            new NameRow("Sin", "Syl"),
            new NameRow("Thaen", "Thaena"),
            new NameRow("Ur", "Um"),
            new NameRow("Von", "Ver"),
            new NameRow("Wil", "Win"),
            new NameRow("Xav", "Xan"),
            new NameRow("Yis", "Yaer"),
            new NameRow("Zed", "Zel"),
        }},
        { NameRow.NameComponent.Core, new List<NameRow>{
            new NameRow("an", "an"),
            new NameRow("ar", "ar"),
            new NameRow("en", "en"),
            new NameRow("or", "is"),
            new NameRow("ath", "il"),
            new NameRow("oth", "ol"),
            new NameRow("yn", "yn"),
            new NameRow("eir", "iel"),
            new NameRow("om", "eth"),
            new NameRow("ur", "ira"),
            new NameRow("el", "ara"),
        } },
        { NameRow.NameComponent.Suffix, new List<NameRow>{
            new NameRow("en", "a"),
            new NameRow("or", "yn"),
            new NameRow("is", "is"),
            new NameRow("eth", "iel"),
            new NameRow("ar", "al"),
            new NameRow("ion", "ine"),
            new NameRow("as", "ara"),
            new NameRow("orim", "arae"),
            new NameRow("el", "ira"),
            new NameRow("orn", "elle"),
        } },
    };
    private static List<string> PersonalityDescriptors = new List<string>
    {
        // Jovial (5)
        "They greet everyone with an easy laugh and bright eyes.",
        "They hum cheerful tunes while they work.",
        "They find humor in even the smallest things.",
        "They clap others on the shoulder with friendly enthusiasm.",
        "They speak with a buoyant energy that lifts the room.",

        // Kind (5)
        "They offer help before anyone needs to ask.",
        "They speak gently, as though choosing words that comfort.",
        "They go out of their way to make others feel included.",
        "They have a calming presence that puts others at ease.",
        "They always look for the good in people and situations.",

        // Studious (5)
        "They constantly take notes, even during casual conversations.",
        "They pause often to think through information deeply.",
        "They carry books everywhere, treating them like treasured tools.",
        "They love explaining things, often with great detail.",
        "They observe surroundings with the curiosity of a researcher.",

        // Serious (5)
        "They rarely smile, focusing intensely on their tasks.",
        "They speak in a firm, steady tone that leaves little room for humor.",
        "They analyze every situation with sharp precision.",
        "They stand rigidly, always prepared for the next step.",
        "They value discipline and expect others to meet their standards.",

        // Nervous (5)
        "They flinch slightly at unexpected noises.",
        "They constantly check over their shoulder as if expecting trouble.",
        "They wring their hands when speaking.",
        "They glance around quickly, rarely focusing on one spot.",
        "Their voice wavers when they start a conversation.",

        // Cocky (5)
        "They smirk as though already certain they’ll win any argument.",
        "They speak with casual confidence bordering on arrogance.",
        "They walk with swagger, expecting admiration.",
        "They interrupt others with self-assured corrections.",
        "They boast about minor accomplishments as if legendary.",

        // Grumpy (5)
        "They grumble under their breath about nearly everything.",
        "They refuse to sugarcoat words, delivering blunt comments.",
        "They roll their eyes often, especially when inconvenienced.",
        "They cross their arms defensively whenever addressed.",
        "They seem perpetually annoyed, even on good days.",

        // Mafioso (5)
        "They speak with an air of quiet authority, expecting obedience.",
        "They make subtle, veiled threats disguised as friendly advice.",
        "They move with deliberate confidence, like someone used to power.",
        "They treat agreements like binding contracts sealed by honor.",
        "Their gaze is sharp and assessing, always calculating alliances.",

        // Pessimistic (5)
        "They immediately expect things to go wrong.",
        "They warn others about potential failure before starting any task.",
        "They sigh often, as though anticipating disappointment.",
        "They dismiss optimistic ideas as ‘wishful thinking.’",
        "They constantly predict negative outcomes with certainty.",

        // Suspicious (5)
        "They narrow their eyes when anyone gets too friendly.",
        "They ask probing questions as though expecting lies.",
        "They keep their distance, always leaving themselves an escape.",
        "They examine belongings and surroundings with paranoid scrutiny.",
        "They distrust compliments, assuming ulterior motives."
    };
    private static List<string> AppearanceDescriptors = new List<string>
    {
        // Immaculate (5)
        "Their clothing is spotless, pressed, and meticulously arranged.",
        "Their appearance is so clean it looks intentionally perfected.",
        "Not a strand of hair or speck of dust is out of place.",
        "They maintain a polished, pristine presentation at all times.",
        "Their attire looks freshly prepared, as though tended to daily.",

        // Simple (5)
        "They wear unadorned, practical clothing with no embellishment.",
        "Their outfit is plain and straightforward, favoring function over style.",
        "They present themselves without flourish, keeping everything modest.",
        "Their clothing is uncomplicated and utilitarian.",
        "Their appearance favors minimalism, lacking any unnecessary detail.",

        // Worn (5)
        "Their clothes show clear signs of heavy use and long days.",
        "Frayed edges and faded colors mark their everyday wear.",
        "Their outfit carries the weary look of countless travels.",
        "Their attire appears weathered by time and hardship.",
        "Scuffs, patches, and wear lines tell a long story of use.",

        // Mismatched (5)
        "Their clothing seems thrown together from unrelated pieces.",
        "Patterns and colors clash boldly across their outfit.",
        "Their attire looks like a chaotic mix of whatever was available.",
        "They combine garments in a way that seems unconventional at best.",
        "Their look gives the impression of someone who dressed in a hurry.",

        // Flowing (5)
        "Their garments drape gracefully and sway with every movement.",
        "Soft fabrics ripple around them like shifting currents.",
        "Their attire features long, loose pieces that flutter as they walk.",
        "They move with a gentle rustle of flowing material.",
        "Their outfit is layered with elegant, drifting fabrics.",

        // Dark (5)
        "Their clothes are dominated by deep, shadowy tones.",
        "They favor muted, somber colors that absorb the light.",
        "Their entire outfit seems to vanish into dimness.",
        "Dark fabrics give them a quiet, brooding presence.",
        "Their attire is composed of blacks and grays, giving a stark silhouette.",

        // Uniformed (5)
        "They wear a distinct outfit that clearly represents an organization.",
        "Their attire is standardized, precise, and consistent.",
        "Every piece of their clothing is matched to a formal dress code.",
        "Badges, insignias, or standard patterns mark their uniformity.",
        "Their appearance suggests strict adherence to a common identity.",

        // Covered in Jewelry (5)
        "They wear numerous decorative pieces that catch the light.",
        "Rings, chains, and trinkets adorn nearly every part of them.",
        "Their ensemble jingles softly with the movement of many adornments.",
        "Their body is decorated with layers of shimmering accessories.",
        "They display an impressive collection of ornaments worn openly.",

        // Bright (5)
        "Their clothing is full of vibrant, eye-catching colors.",
        "They practically glow thanks to radiant, lively attire.",
        "Bold, vivid shades dominate their appearance.",
        "Their outfit is cheerful and energetic in its brightness.",
        "Even in a crowd, their vivid clothing stands out immediately.",

        // Ceremonial (5)
        "They wear elaborate attire meant for important rituals or events.",
        "Intricate patterns and symbolic elements decorate their clothing.",
        "Their outfit carries the formal weight of tradition and ceremony.",
        "They are dressed in garments reserved for special occasions.",
        "Their attire features ornate detailing with cultural significance."
    };
    private static List<string> PhysicalFeatures = new List<string>
    {
        "Crooked nose",
        "Missing tooth",
        "Scar",
        "Birthmark",
        "Tattoo",
        "Missing limb",
        "Unkempt hair",
        "Impressively tall",
        "Impressively short",
        "One eye",
        "Subtle glow or aura",
        "Weathered skin",
        "Burn scars",
        "Freckles",
        "Gap between front teeth",
        "Strong jawline",
        "Prominent cheekbones",
        "Sunken eyes",
        "Heavy bags under eyes",
        "Bushy eyebrows",
        "Thin, almost invisible eyebrows",
        "Patchy facial hair",
        "Braided beard or hair section",
        "Calloused hands",
        "Rough, cracked knuckles",
        "Long, thin fingers",
        "Short, stubby fingers",
        "Broad shoulders",
        "Narrow shoulders",
        "Limp or uneven gait",
        "Bent posture",
        "Distinctive walk pattern (bouncy, heavy, quick steps)",
        "Pierced ears",
        "Pierced nose or eyebrow",
        "Shaved head",
        "Receding hairline",
        "Thick, curly hair",
        "Fine, silky hair",
        "Ash-gray hair",
        "Split lip or old lip injury",
        "Crooked smile",
        "Overly straight posture",
        "Large, expressive eyes",
        "Small, tight-set eyes",
        "Dramatic laugh lines",
        "Dimples",
        "One ear shaped differently than the other",
        "Strong tan lines",
        "Unusual but natural eye color",
        "Ruddy complexion"
    };
    private static List<string> DefaultBonds = new List<string>
    {
        "Loyal",
        "Oath",
        "Protect",
        "Vengeance",
        "Faith",
        "Debt/Crime",
        "Knowledge/Secret",
        "Love",
        "Curse",
        "Mistake/Sin",
    };
    private static List<string> DefaultGoals = new List<string>
    {
        "Artifact/Relic",
        "Nobility/Kingdom",
        "Guild",
        "Bloodline",
        "Creature",
        "Land/Nature",
        "Element",
        "Magic/Spell",
        "Religion",
        "History/Lore",
    };



    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    public string ID { get; set; } = Guid.NewGuid().ToString();
    private string _gender;
    public string Gender
    {
        get => _gender;
        set => SetProperty(ref _gender, value);
    }
    private string _race;
    public string Race
    {
        get => _race;
        set => SetProperty(ref _race, value);
    }
    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    private string _goal;
    public string Goal
    {
        get => _goal;
        set => SetProperty(ref _goal, value);
    }
    private Location? _home;
    public Location? Home
    {
        get => _home;
        set => SetProperty(ref _home, value);
    }
    public NPC() { }
    public NPC(string name, string race, string description, string goal)
    {
        Name = name;
        Race = race;
        Description = description;
        Goal = goal;
    }
    public NPC(NPC npcToCopy)
    {
        Name = "Copy of " + npcToCopy.Name;
        Race = npcToCopy.Race;
        Description = npcToCopy.Description;
        Goal = npcToCopy.Goal;
        Home = npcToCopy.Home;
    }

    public static string GetRandomName(string gender)
    {
        Random _random = new Random();

        string name = GetRandomPart(NameComponent.Prefix, gender) + GetRandomPart(NameComponent.Core, gender);
        if (_random.Next(2) == 0) name += GetRandomPart(NameComponent.Suffix, gender);

        return name;
    }

    private static string GetRandomPart(NameComponent component, string gender)
    {
        Random _random = new Random();
        var list = NameTables[component];
        var row = list[_random.Next(list.Count)];

        return gender == "Male" ? row.Male : row.Female;
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
    public static string GetRandomGender()
    {
        Random _random = new Random();
        int val = _random.Next(2);
        Debug.WriteLine(val);
        if (val == 0) return "Male";
        else return "Female";
    }
    public static Location GetRandomLocation()
    {
        List<Location> locations = App.CampaignStore.CurrentCampaign.Locations.ToList();
        if (locations.Count == 0) return null;
        Random _random = new Random();
        return locations[_random.Next(locations.Count)];
    }

    public static string GetRandomDescription()
    {
        Random _random = new Random();
        string personality = PersonalityDescriptors[_random.Next(PersonalityDescriptors.Count)];
        string appearance = AppearanceDescriptors[_random.Next(AppearanceDescriptors.Count)];
        string physicalFeature = PhysicalFeatures[_random.Next(PhysicalFeatures.Count)];
        return personality + "\n" + appearance + "\n" + physicalFeature;
    }
    public static string GetRandomGoal()
    {
        Random random = new Random();
        string bond = DefaultBonds[random.Next(DefaultBonds.Count)];
        string goal = DefaultGoals[random.Next(DefaultGoals.Count)];
        return "Bond: " + bond + "\nGoal: " + goal;
    }
}