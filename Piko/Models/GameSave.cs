using System;
using System.Collections.Generic;
using Piko.Models.Entities;

namespace Piko.Models
{
    public class GameSave
    {
        public Plant      Plant  { get; set; } = new();
        public PlayerData Player { get; set; } = new();
        public WorldData  World  { get; set; } = new();
        public MetaData   Meta   { get; set; } = new();
    }

    public class PlayerData
    {
        public string       Name            { get; set; } = "Jardinier";
        public long         Xp              { get; set; } = 0;
        public int          Level           { get; set; } = 1;
        public List<string> Achievements    { get; set; } = new();
        public int          TotalSessions   { get; set; } = 0;
        public int          ConsecutiveDays { get; set; } = 0;
        public int          TalkCount       { get; set; } = 0;
        public int          SingCount       { get; set; } = 0;
        public DateTime?    LastSessionDate { get; set; }

        // Achievement tracking
        public bool      HealthDroppedBelow50 { get; set; } = false;
        public bool      StormSurvived        { get; set; } = false;
        public DateTime? PerfectCareStart     { get; set; }
    }

    public class WorldData
    {
        public Weather    CurrentWeather { get; set; } = new();
        public GameEvent? ActiveEvent    { get; set; }
    }

    public class MetaData
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastSaved { get; set; } = DateTime.UtcNow;
    }
}
