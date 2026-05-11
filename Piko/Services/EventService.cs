using System;
using Piko.Models;
using Piko.Models.Entities;

namespace Piko.Services
{
    public class EventService
    {
        private readonly Random _rng = new();

        private static readonly (EventType Type, int Weight, int Hours)[] Pool =
        {
            (EventType.PestAttack,       10,  8),
            (EventType.Drought,           8, 12),
            (EventType.Blessing,         12,  6),
            (EventType.SurpriseGrowth,    8,  4),
            (EventType.NutrientBoost,    10, 12),
            (EventType.Heatwave,          8,  8),
            (EventType.Aphids,           10, 10),
            (EventType.GoodSunshine,     15,  6),
            (EventType.RarePollinators,   6,  4),
        };

        public GameEvent? TryTrigger(WorldData world, DateTime lastSaved)
        {
            if (world.ActiveEvent?.IsActive == true) return null;

            var hoursSince = (DateTime.UtcNow - lastSaved).TotalHours;
            if (hoursSince < Constants.EventMinHourGap) return null;
            if (_rng.NextDouble() > Constants.EventChance) return null;

            var ev = Spawn();
            world.ActiveEvent = ev;
            return ev;
        }

        private GameEvent Spawn()
        {
            int total = 0;
            foreach (var (_, w, _) in Pool) total += w;

            int roll = _rng.Next(total), cumul = 0;
            EventType chosen = EventType.Blessing;
            int hours = 6;

            foreach (var (type, weight, dur) in Pool)
            {
                cumul += weight;
                if (roll < cumul) { chosen = type; hours = dur; break; }
            }

            return new GameEvent
            {
                Id            = Guid.NewGuid().ToString("N")[..8],
                Type          = chosen,
                StartedAt     = DateTime.UtcNow,
                DurationHours = hours
            };
        }
    }
}
