using System;
using Piko.Models;
using Piko.Models.Entities;

namespace Piko.Services
{
    public class WeatherService
    {
        private readonly Random _rng = new();

        private static readonly (WeatherType Type, int Weight)[] Weights =
        {
            (WeatherType.Sunny,  25),
            (WeatherType.Cloudy, 20),
            (WeatherType.Rainy,  20),
            (WeatherType.Misty,  10),
            (WeatherType.Hot,    10),
            (WeatherType.Cold,   10),
            (WeatherType.Stormy,  5),
        };

        // Returns true if weather changed
        public bool Update(WorldData world)
        {
            if (!world.CurrentWeather.IsExpired) return false;
            world.CurrentWeather = Generate(world.CurrentWeather.Type);
            return true;
        }

        public Weather Generate(WeatherType? exclude = null)
        {
            int total = 0;
            foreach (var (t, w) in Weights)
                if (t != exclude) total += w;

            int roll = _rng.Next(total), cumul = 0;
            WeatherType chosen = WeatherType.Sunny;

            foreach (var (type, weight) in Weights)
            {
                if (type == exclude) continue;
                cumul += weight;
                if (roll < cumul) { chosen = type; break; }
            }

            return new Weather
            {
                Type          = chosen,
                ChangedAt     = DateTime.UtcNow,
                DurationHours = _rng.Next(Constants.WeatherMinHours, Constants.WeatherMaxHours + 1)
            };
        }
    }
}
