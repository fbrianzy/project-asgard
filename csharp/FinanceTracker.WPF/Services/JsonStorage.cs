using System.Text.Json;
using FinanceTracker.Models;

namespace FinanceTracker.Services
{
    public class JsonStorage
    {
        private readonly string _path;
        private readonly JsonSerializerOptions _opt = new() { WriteIndented = true };
        public JsonStorage(string path) { _path = path; }

        public List<Transaction> Load()
        {
            if (!File.Exists(_path)) return new();
            var json = File.ReadAllText(_path);
            return JsonSerializer.Deserialize<List<Transaction>>(json) ?? new();
        }

        public void Save(IEnumerable<Transaction> items)
        {
            var json = JsonSerializer.Serialize(items, _opt);
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(_path, json);
        }
    }
}
