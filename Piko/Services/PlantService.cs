using System;
using Piko.Models;
using Piko.Models.Entities;

namespace Piko.Services
{
    public class PlantService
    {
        private readonly Random _rng = new();

        public void SimulateTime(GameSave save, double hours)
        {
            if (save.Plant.IsDead || save.Plant.Stage >= Constants.MaxStage) return;

            int chunks = Math.Max(1, (int)Math.Ceiling(hours));
            double frac = hours / chunks;
            for (int i = 0; i < chunks; i++) Tick(save, frac);
        }

        private void Tick(GameSave save, double h)
        {
            var p  = save.Plant;
            var w  = save.World.CurrentWeather;
            var ev = save.World.ActiveEvent?.IsActive == true ? save.World.ActiveEvent : null;

            // Water
            double wd = Constants.WaterDepletionPerHour * w.WaterMultiplier * h;
            if (ev?.Type == EventType.Drought)  wd *= 2.0;
            if (ev?.Type == EventType.Heatwave) wd *= 1.5;
            p.Water = Math.Clamp(p.Water - wd, 0, 100);

            // Nutrients
            double nd = Constants.NutrientDepletionPerHour * h;
            if (ev?.Type == EventType.NutrientBoost) nd = 0;
            p.Nutrients = Math.Clamp(p.Nutrients - nd, 0, 100);

            // Health
            bool lowW = p.Water     < Constants.HealthDecayThreshold;
            bool lowN = p.Nutrients < Constants.HealthDecayThreshold;
            if (lowW || lowN)
            {
                double decay = Constants.HealthDecayRate * h * (lowW && lowN ? 2.0 : 1.0);
                p.Health = Math.Max(0, p.Health - decay);
            }
            else
            {
                p.Health = Math.Min(100, p.Health + 0.4 * h);
            }

            if (ev?.Type == EventType.PestAttack) p.Health = Math.Max(0, p.Health - 2.5 * h);
            if (ev?.Type == EventType.Aphids)     p.Health = Math.Max(0, p.Health - 1.0 * h);
            if (w.StormDamageChance > 0 && _rng.NextDouble() < w.StormDamageChance * h)
                p.Health = Math.Max(0, p.Health - 10.0);

            // Happiness
            double hd = Constants.HappinessDecayPerHour * h;
            if (ev?.Type is EventType.Blessing or EventType.RarePollinators) hd = 0;
            p.Happiness = Math.Max(0, p.Happiness - hd);
            if (ev?.Type == EventType.Blessing)        p.Happiness = Math.Min(100, p.Happiness + 1.0 * h);
            if (ev?.Type == EventType.RarePollinators) p.Happiness = Math.Min(100, p.Happiness + 2.0 * h);

            // Growth
            if (p.Water > 20 && p.Health > 30)
            {
                double g = Constants.GrowthPerHour * w.GrowthMultiplier * h;
                if (ev?.Type == EventType.SurpriseGrowth) g *= 2.5;
                if (ev?.Type == EventType.GoodSunshine)   g *= 1.5;
                if (p.Happiness > 70) g *= 1.1;
                if (p.Happiness < 30) g *= 0.7;
                p.Growth = Math.Min(100, p.Growth + g);
            }

            if (p.Growth >= 100 && p.Stage < Constants.MaxStage)
            {
                p.Stage++;
                p.Growth = 0;
            }

            if (p.Health <= 0) p.IsDead = true;
        }

        // ── Player actions ───────────────────────────────────────────────────

        public (bool ok, string msg) Water(GameSave save)
        {
            var p = save.Plant;
            if (!Cooldown(p.LastWatered, Constants.WaterCooldown, out double r))
                return (false, $"Wait {r:F1}h more before watering.");

            p.Water       = Math.Min(100, p.Water + Constants.WaterAmount);
            p.Health      = Math.Min(100, p.Health + 2.0);
            p.LastWatered = DateTime.UtcNow;
            return (true, $"{p.Name} drinks happily. Water: {p.Water:F0}%");
        }

        public (bool ok, string msg) Fertilize(GameSave save)
        {
            var p = save.Plant;
            if (!Cooldown(p.LastFed, Constants.FertilizeCooldown, out double r))
                return (false, $"Next fertilization in {r:F1}h.");

            p.Nutrients = Math.Min(100, p.Nutrients + Constants.FertilizeAmount);
            p.Growth    = Math.Min(100, p.Growth + 5.0);
            p.LastFed   = DateTime.UtcNow;
            return (true, $"{p.Name} absorbs the nutrients. Nutrients: {p.Nutrients:F0}%");
        }

        public (bool ok, string msg) Talk(GameSave save)
        {
            var p = save.Plant;
            if (!Cooldown(p.LastTalked, Constants.TalkCooldown, out double r))
                return (false, $"Your plant is resting. Come back in {r:F1}h.");

            p.Happiness   = Math.Min(100, p.Happiness + Constants.TalkHappiness);
            p.LastTalked  = DateTime.UtcNow;
            save.Player.TalkCount++;
            return (true, $"{p.Name} shivers with joy. Happiness: {p.Happiness:F0}%");
        }

        public (bool ok, string msg) Sing(GameSave save)
        {
            var p = save.Plant;
            if (!Cooldown(p.LastSung, Constants.SingCooldown, out double r))
                return (false, $"Sing again in {r:F1}h.");

            p.Happiness  = Math.Min(100, p.Happiness + Constants.SingHappiness);
            p.Growth     = Math.Min(100, p.Growth + 3.0);
            p.LastSung   = DateTime.UtcNow;
            save.Player.SingCount++;
            return (true, $"Your melodies enchant {p.Name}! Happiness: {p.Happiness:F0}%");
        }

        public (bool ok, string msg) Prune(GameSave save)
        {
            var p = save.Plant;
            if (p.Stage < 3)
                return (false, "Too early to prune. Wait for the plant to grow bigger.");
            if (!Cooldown(p.LastPruned, Constants.PruneCooldown, out double r))
                return (false, $"Next pruning in {r:F1}h.");

            p.Health     = Math.Min(100, p.Health + Constants.PruneHealth);
            p.Growth     = Math.Min(100, p.Growth + Constants.PruneGrowth);
            p.LastPruned = DateTime.UtcNow;
            return (true, $"{p.Name} is pruned with care. Health: {p.Health:F0}%");
        }

        private static bool Cooldown(DateTime? last, double hours, out double remaining)
        {
            remaining = 0;
            if (!last.HasValue) return true;
            var elapsed = (DateTime.UtcNow - last.Value).TotalHours;
            if (elapsed >= hours) return true;
            remaining = hours - elapsed;
            return false;
        }
    }
}
