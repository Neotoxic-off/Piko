using System;

namespace Piko.Models.Entities
{
    public class Plant
    {
        public string   Name      { get; set; } = "Piko";
        public int      Stage     { get; set; } = 0;
        public double   Health    { get; set; } = 100.0;
        public double   Happiness { get; set; } = 80.0;
        public double   Water     { get; set; } = 80.0;
        public double   Nutrients { get; set; } = 70.0;
        public double   Growth    { get; set; } = 0.0;   // 0–100 within current stage
        public DateTime BornAt    { get; set; } = DateTime.UtcNow;
        public bool     IsDead    { get; set; } = false;

        // Action cooldown timestamps
        public DateTime? LastWatered { get; set; }
        public DateTime? LastFed     { get; set; }
        public DateTime? LastTalked  { get; set; }
        public DateTime? LastSung    { get; set; }
        public DateTime? LastPruned  { get; set; }

        public int    AgeDays   => (int)(DateTime.UtcNow - BornAt).TotalDays;
        public string StageName => Constants.StageNames[Stage];
    }
}
