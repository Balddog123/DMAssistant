using DMAssistant.Model;
using System.IO;
using System.Text.Json;

namespace DMAssistant.Repository
{
    public static class MonsterRepository
    {
        private static List<Monster>? _cache;

        public static IReadOnlyList<Monster> GetAllMonsters()
        {
            if (_cache != null)
                return _cache;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "monsters.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("Monsters JSON not found:", path);

            var json = File.ReadAllText(path);
            _cache = JsonSerializer.Deserialize<List<Monster>>(json)
                    ?? new List<Monster>();

            return _cache;
        }
    }
}
