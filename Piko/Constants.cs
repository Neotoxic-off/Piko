namespace Piko
{
    public static class Constants
    {
        public const string SaveFileName  = "save.json";
        public const string SaveDirectory = ".piko";

        public const int MaxStage = 6;

        public static readonly string[] StageNames =
        {
            "Seed", "Sprout", "Seedling",
            "Young Plant", "Mature Plant", "Flowering", "Full Bloom"
        };

        // Growth progress per real hour (100 pts per stage ≈ 30 days with care)
        public const double GrowthPerHour = 0.7;

        // Stat depletion per real hour
        public const double WaterDepletionPerHour    = 1.8;
        public const double NutrientDepletionPerHour = 0.4;
        public const double HappinessDecayPerHour    = 0.25;

        // Health decays when water OR nutrients drop below this
        public const double HealthDecayThreshold = 20.0;
        public const double HealthDecayRate       = 1.2;

        // Action stat effects
        public const double WaterAmount     = 35.0;
        public const double FertilizeAmount = 40.0;
        public const double TalkHappiness   = 12.0;
        public const double SingHappiness   = 18.0;
        public const double PruneHealth     = 22.0;
        public const double PruneGrowth     = 8.0;

        // XP rewards
        public const int XpWater     = 10;
        public const int XpFertilize = 15;
        public const int XpTalk      = 5;
        public const int XpSing      = 8;
        public const int XpPrune     = 12;

        // Cooldowns in hours
        public const double WaterCooldown     = 4.0;
        public const double FertilizeCooldown = 24.0;
        public const double TalkCooldown      = 2.0;
        public const double SingCooldown      = 6.0;
        public const double PruneCooldown     = 48.0;

        // XP thresholds per level (20 levels, index = level-1)
        public static readonly long[] LevelThresholds =
        {
            0, 50, 130, 250, 420, 650, 950, 1320, 1770, 2300,
            2920, 3640, 4470, 5420, 6500, 7720, 9090, 10620, 12320, 14200
        };

        // Weather duration range in hours
        public const int WeatherMinHours = 3;
        public const int WeatherMaxHours = 9;

        // Event trigger settings
        public const double EventChance     = 0.22;
        public const double EventMinHourGap = 3.0;
    }
}
