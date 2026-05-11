using System;

namespace Piko.Models.Entities
{
    public enum EventType
    {
        PestAttack, Drought, Blessing, SurpriseGrowth,
        NutrientBoost, Heatwave, Aphids, GoodSunshine, RarePollinators
    }

    public class GameEvent
    {
        public string    Id            { get; set; } = "";
        public EventType Type          { get; set; }
        public DateTime  StartedAt     { get; set; } = DateTime.UtcNow;
        public int       DurationHours { get; set; } = 8;
        public bool      IsResolved    { get; set; } = false;

        public bool IsExpired => DateTime.UtcNow >= StartedAt.AddHours(DurationHours);
        public bool IsActive  => !IsResolved && !IsExpired;

        public string DisplayName => Type switch
        {
            EventType.PestAttack      => "[red]Pest Attack[/]",
            EventType.Drought         => "[orange3]Drought[/]",
            EventType.Blessing        => "[green]Blessing[/]",
            EventType.SurpriseGrowth  => "[lime]Surprise Growth![/]",
            EventType.NutrientBoost   => "[yellow]Nutrient Boost[/]",
            EventType.Heatwave        => "[red]Heatwave[/]",
            EventType.Aphids          => "[orange3]Aphid Invasion[/]",
            EventType.GoodSunshine    => "[yellow]Perfect Sunshine[/]",
            EventType.RarePollinators => "[green]Rare Pollinators[/]",
            _                         => "Event"
        };

        public string Description => Type switch
        {
            EventType.PestAttack      => "Pests are attacking! Tend to your plant quickly.",
            EventType.Drought         => "Drought! Water evaporates 2x faster.",
            EventType.Blessing        => "Nature smiles! Happiness and growth boosted.",
            EventType.SurpriseGrowth  => "Sudden growth spurt! Growth rate x2.5.",
            EventType.NutrientBoost   => "Rich soil! Nutrients regenerate automatically.",
            EventType.Heatwave        => "Extreme heat! Risk of accelerated wilting.",
            EventType.Aphids          => "Aphids present! Health drains slowly.",
            EventType.GoodSunshine    => "Perfect light! Growth rate accelerated.",
            EventType.RarePollinators => "Butterflies visit! Happiness maxed.",
            _                         => ""
        };
    }
}
