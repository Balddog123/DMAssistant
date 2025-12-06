using DMAssistant.Helpers;
using DMAssistant.Model;
using System.IO;
using System.Text.Json;

namespace DMAssistant.Repository
{
    public static class DataRepository
    {
        private static List<Monster>? _monsterCache;
        private static List<Spell>? _spellsCache;

        public static IReadOnlyList<Monster> GetAllMonsters()
        {
            if (_monsterCache != null)
                return _monsterCache;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "monsters.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("Monsters JSON not found:", path);

            var json = File.ReadAllText(path);
            _monsterCache = JsonSerializer.Deserialize<List<Monster>>(json)
                    ?? new List<Monster>();

            return _monsterCache;
        }

        public static IReadOnlyList<Spell> GetAllSpells()
        {
            if (_spellsCache != null)
                return _spellsCache;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "spells.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("Spells JSON not found:", path);

            var json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            options.Converters.Add(new CastingTimeConverter());
            options.Converters.Add(new ComponentsConverter());
            options.Converters.Add(new SchoolConverter());

            _spellsCache = JsonSerializer.Deserialize<List<Spell>>(json, options) ?? new List<Spell>();

            return _spellsCache;
        }
    }
}
