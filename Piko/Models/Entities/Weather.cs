using System;

namespace Piko.Models.Entities
{
    public enum WeatherType { Sunny, Cloudy, Rainy, Stormy, Hot, Cold, Misty }

    public class Weather
    {
        public WeatherType Type          { get; set; } = WeatherType.Sunny;
        public DateTime    ChangedAt     { get; set; } = DateTime.UtcNow;
        public int         DurationHours { get; set; } = 6;

        public bool IsExpired => DateTime.UtcNow >= ChangedAt.AddHours(DurationHours);

        public string DisplayName => Type switch
        {
            WeatherType.Sunny  => "[yellow]Sunny[/]",
            WeatherType.Cloudy => "[grey]Cloudy[/]",
            WeatherType.Rainy  => "[blue]Rainy[/]",
            WeatherType.Stormy => "[red]Stormy[/]",
            WeatherType.Hot    => "[orange3]Heatwave[/]",
            WeatherType.Cold   => "[cyan]Cold[/]",
            WeatherType.Misty  => "[grey62]Misty[/]",
            _                  => "Unknown"
        };

        public string Icon => Type switch
        {
            WeatherType.Sunny  => "[yellow]☀[/]",
            WeatherType.Cloudy => "[grey]☁[/]",
            WeatherType.Rainy  => "[blue]~[/]",
            WeatherType.Stormy => "[red]#[/]",
            WeatherType.Hot    => "[orange3]^[/]",
            WeatherType.Cold   => "[cyan]*[/]",
            WeatherType.Misty  => "[grey62].[/]",
            _                  => "?"
        };

        // Negative value = water refills
        public double WaterMultiplier => Type switch
        {
            WeatherType.Sunny  =>  1.2,
            WeatherType.Cloudy =>  0.9,
            WeatherType.Rainy  => -0.6,
            WeatherType.Stormy =>  0.8,
            WeatherType.Hot    =>  2.0,
            WeatherType.Cold   =>  0.5,
            WeatherType.Misty  =>  0.7,
            _                  =>  1.0
        };

        public double GrowthMultiplier => Type switch
        {
            WeatherType.Sunny  => 1.3,
            WeatherType.Cloudy => 0.9,
            WeatherType.Rainy  => 1.4,
            WeatherType.Stormy => 0.3,
            WeatherType.Hot    => 0.5,
            WeatherType.Cold   => 0.4,
            WeatherType.Misty  => 1.0,
            _                  => 1.0
        };

        public double StormDamageChance => Type == WeatherType.Stormy ? 0.25 : 0.0;
    }
}
