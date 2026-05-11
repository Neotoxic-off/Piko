using System;
using System.IO;
using Newtonsoft.Json;
using Piko.Models;

namespace Piko.Services
{
    public class SaveService
    {
        private readonly string _savePath;

        public SaveService()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                Constants.SaveDirectory);
            Directory.CreateDirectory(dir);
            _savePath = Path.Combine(dir, Constants.SaveFileName);
        }

        public bool   SaveExists() => File.Exists(_savePath);
        public void   Delete()     { if (File.Exists(_savePath)) File.Delete(_savePath); }

        public GameSave Load()
        {
            if (!File.Exists(_savePath)) return new GameSave();
            try
            {
                var json = File.ReadAllText(_savePath);
                return JsonConvert.DeserializeObject<GameSave>(json) ?? new GameSave();
            }
            catch { return new GameSave(); }
        }

        public void Save(GameSave save)
        {
            save.Meta.LastSaved = DateTime.UtcNow;
            File.WriteAllText(_savePath, JsonConvert.SerializeObject(save, Formatting.Indented));
        }
    }
}
