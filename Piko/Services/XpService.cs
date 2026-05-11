using Piko.Models;

namespace Piko.Services
{
    public class XpService
    {
        // Returns true if player leveled up
        public bool AddXp(GameSave save, int amount)
        {
            save.Player.Xp += amount;
            int newLevel = CalcLevel(save.Player.Xp);
            if (newLevel <= save.Player.Level) return false;
            save.Player.Level = newLevel;
            return true;
        }

        public long XpToNext(GameSave save)
        {
            int lv = save.Player.Level;
            if (lv >= Constants.LevelThresholds.Length) return 0;
            return Constants.LevelThresholds[lv] - save.Player.Xp;
        }

        public double LevelProgress(GameSave save)
        {
            int lv = save.Player.Level;
            if (lv >= Constants.LevelThresholds.Length) return 100;
            long cur    = save.Player.Xp - Constants.LevelThresholds[lv - 1];
            long needed = Constants.LevelThresholds[lv] - Constants.LevelThresholds[lv - 1];
            return (double)cur / needed * 100.0;
        }

        private static int CalcLevel(long xp)
        {
            for (int i = Constants.LevelThresholds.Length - 1; i >= 0; i--)
                if (xp >= Constants.LevelThresholds[i]) return i + 1;
            return 1;
        }
    }
}
