using System;
using System.Collections.Generic;
using Piko.Models;
using Piko.Models.Entities;

namespace Piko.Services
{
    public class AchievementService
    {
        public static readonly List<Achievement> All = new()
        {
            new() { Id = "first_sprout",   Name = "First Sprout",       Description = "Reach the Sprout stage",                       Icon = "+", XpReward = 20  },
            new() { Id = "seedling",       Name = "Seedling",            Description = "Reach the Seedling stage",                     Icon = "+", XpReward = 30  },
            new() { Id = "young_plant",    Name = "Young Growth",        Description = "Reach the Young Plant stage",                  Icon = "*", XpReward = 50  },
            new() { Id = "mature",         Name = "Adult Plant",         Description = "Reach the Mature Plant stage",                 Icon = "*", XpReward = 75  },
            new() { Id = "flowering",      Name = "In Bloom",            Description = "Reach the Flowering stage",                    Icon = "@", XpReward = 100 },
            new() { Id = "full_bloom",     Name = "Full Bloom",          Description = "Reach Full Bloom",                             Icon = "#", XpReward = 150 },
            new() { Id = "week_1",         Name = "First Week",          Description = "Your plant is 7 days old",                    Icon = "~", XpReward = 40  },
            new() { Id = "week_2",         Name = "Two Weeks",           Description = "Your plant is 14 days old",                   Icon = "~", XpReward = 60  },
            new() { Id = "month_1",        Name = "Gardener of the Month", Description = "Your plant is 30 days old",                 Icon = "!", XpReward = 200 },
            new() { Id = "green_thumb",    Name = "Green Thumb",         Description = "Reach stage 4 without dropping below 50% health", Icon = "^", XpReward = 80  },
            new() { Id = "dedicated",      Name = "Dedicated",           Description = "15 consecutive days of care",                 Icon = "^", XpReward = 100 },
            new() { Id = "chatterbox",     Name = "Chatterbox",          Description = "Talk to your plant 30 times",                 Icon = "~", XpReward = 50  },
            new() { Id = "singer",         Name = "Songwriter",          Description = "Sing 20 times",                               Icon = "~", XpReward = 40  },
            new() { Id = "perfect_care",   Name = "Perfect Care",        Description = "7 days without dropping below 80% health",    Icon = "!", XpReward = 90  },
            new() { Id = "storm_survivor", Name = "Storm Survivor",      Description = "Survive a stormy weather period",             Icon = "#", XpReward = 35  },
            new() { Id = "level_5",        Name = "Level 5",             Description = "Reach level 5",                               Icon = "*", XpReward = 0   },
            new() { Id = "level_10",       Name = "Level 10",            Description = "Reach level 10",                              Icon = "*", XpReward = 0   },
            new() { Id = "level_20",       Name = "Master Gardener",     Description = "Reach level 20",                              Icon = "!", XpReward = 0   },
        };

        // Checks all achievements, grants new ones, returns newly granted list
        public List<Achievement> Check(GameSave save)
        {
            // Update tracking flags
            if (save.World.CurrentWeather.Type == WeatherType.Stormy && save.Plant.Health > 0)
                save.Player.StormSurvived = true;

            if (save.Plant.Health < 50)
                save.Player.HealthDroppedBelow50 = true;

            if (save.Plant.Health >= 80 && !save.Player.PerfectCareStart.HasValue)
                save.Player.PerfectCareStart = DateTime.UtcNow;
            else if (save.Plant.Health < 80)
                save.Player.PerfectCareStart = null;

            var earned = new List<Achievement>();
            foreach (var ach in All)
            {
                if (save.Player.Achievements.Contains(ach.Id)) continue;
                if (!IsUnlocked(ach.Id, save)) continue;
                save.Player.Achievements.Add(ach.Id);
                earned.Add(ach);
            }
            return earned;
        }

        private static bool IsUnlocked(string id, GameSave s) => id switch
        {
            "first_sprout"   => s.Plant.Stage >= 1,
            "seedling"       => s.Plant.Stage >= 2,
            "young_plant"    => s.Plant.Stage >= 3,
            "mature"         => s.Plant.Stage >= 4,
            "flowering"      => s.Plant.Stage >= 5,
            "full_bloom"     => s.Plant.Stage >= 6,
            "week_1"         => s.Plant.AgeDays >= 7,
            "week_2"         => s.Plant.AgeDays >= 14,
            "month_1"        => s.Plant.AgeDays >= 30,
            "green_thumb"    => s.Plant.Stage >= 4 && !s.Player.HealthDroppedBelow50,
            "dedicated"      => s.Player.ConsecutiveDays >= 15,
            "chatterbox"     => s.Player.TalkCount >= 30,
            "singer"         => s.Player.SingCount >= 20,
            "perfect_care"   => s.Player.PerfectCareStart.HasValue &&
                                 (DateTime.UtcNow - s.Player.PerfectCareStart.Value).TotalDays >= 7,
            "storm_survivor" => s.Player.StormSurvived,
            "level_5"        => s.Player.Level >= 5,
            "level_10"       => s.Player.Level >= 10,
            "level_20"       => s.Player.Level >= 20,
            _                => false
        };
    }
}
