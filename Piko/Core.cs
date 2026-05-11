using System;
using System.Threading.Tasks;
using Spectre.Console;
using Piko.Models;
using Piko.Services;
using Piko.UI;

namespace Piko
{
    public class Core
    {
        private readonly SaveService        _save    = new();
        private readonly WeatherService     _weather = new();
        private readonly EventService       _events  = new();
        private readonly PlantService       _plant   = new();
        private readonly XpService          _xp      = new();
        private readonly AchievementService _achieve = new();
        private readonly Renderer           _ui      = new();

        public async Task Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            GameSave save = _save.SaveExists() ? _save.Load() : await NewGame();
            bool isNew = !_save.SaveExists();

            // Session start
            save.Player.TotalSessions++;
            UpdateConsecutiveDays(save);

            // Simulate elapsed real time
            var elapsed = DateTime.UtcNow - save.Meta.LastSaved;
            if (elapsed.TotalMinutes > 5)
                _plant.SimulateTime(save, elapsed.TotalHours);

            // Update weather (may have changed while away)
            bool weatherChanged = _weather.Update(save.World);

            // Try to trigger a random event
            var newEvent = _events.TryTrigger(save.World, save.Meta.LastSaved);

            // Grant any newly earned achievements
            var newAch = _achieve.Check(save);
            foreach (var a in newAch) _xp.AddXp(save, a.XpReward);

            _save.Save(save);

            // Show session-start notifications
            if (newEvent != null)  _ui.ShowEvent(newEvent);
            if (newAch.Count > 0)  _ui.ShowNewAchievements(newAch);

            string? lastMsg = null;

            // ── Main loop ────────────────────────────────────────────────────
            bool running = true;
            while (running)
            {
                if (save.Plant.IsDead)
                {
                    _ui.ShowDeath(save);
                    AnsiConsole.MarkupLine("\n  [white][[R]][/] Start over   [white][[Q]][/] Quit");
                    var k = Console.ReadKey(true);
                    if (k.KeyChar is 'r' or 'R')
                    {
                        _save.Delete();
                        save = await NewGame();
                    }
                    else
                    {
                        running = false;
                    }
                    continue;
                }

                _ui.Render(save, _xp, lastMsg);
                lastMsg = null;

                var key = Console.ReadKey(true);

                switch (char.ToLower(key.KeyChar))
                {
                    case '1':
                        lastMsg = DoAction(save, _plant.Water(save), Constants.XpWater);
                        break;
                    case '2':
                        lastMsg = DoAction(save, _plant.Fertilize(save), Constants.XpFertilize);
                        break;
                    case '3':
                        lastMsg = DoAction(save, _plant.Talk(save), Constants.XpTalk);
                        break;
                    case '4':
                        lastMsg = DoAction(save, _plant.Sing(save), Constants.XpSing);
                        break;
                    case '5':
                        lastMsg = DoAction(save, _plant.Prune(save), Constants.XpPrune);
                        break;
                    case '6':
                        _ui.ShowAllAchievements(save);
                        break;
                    case '7':
                        _ui.ShowStats(save);
                        break;
                    case 'q':
                        running = false;
                        break;
                }

                if (running)
                {
                    var earned = _achieve.Check(save);
                    if (earned.Count > 0)
                    {
                        foreach (var a in earned) _xp.AddXp(save, a.XpReward);
                        _ui.ShowNewAchievements(earned);
                    }
                    _save.Save(save);
                }
            }

            _ui.ShowGoodbye(save);
            _save.Save(save);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private string DoAction(GameSave save, (bool ok, string msg) result, int xpAmount)
        {
            if (result.ok)
            {
                bool leveledUp = _xp.AddXp(save, xpAmount);
                if (leveledUp) _ui.ShowLevelUp(save.Player.Level);
                return $"[green]{Spectre.Console.Markup.Escape(result.msg)}[/]";
            }
            return $"[yellow]{Spectre.Console.Markup.Escape(result.msg)}[/]";
        }

        private static void UpdateConsecutiveDays(GameSave save)
        {
            var today = DateTime.UtcNow.Date;
            if (!save.Player.LastSessionDate.HasValue)
            {
                save.Player.ConsecutiveDays = 1;
            }
            else
            {
                var lastDate = save.Player.LastSessionDate.Value.Date;
                if (lastDate == today) { /* same day, no change */ }
                else if (lastDate == today.AddDays(-1)) save.Player.ConsecutiveDays++;
                else save.Player.ConsecutiveDays = 1;
            }
            save.Player.LastSessionDate = DateTime.UtcNow;
        }

        private async Task<GameSave> NewGame()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Spectre.Console.Rule("[bold green] PIKO - New Game [/]").RuleStyle("green"));
            AnsiConsole.MarkupLine("\n  [grey]Welcome! You will care for a plant over 30 real days.[/]\n");

            string playerName = AnsiConsole.Ask<string>("  [grey]Your name:[/] ");
            string plantName  = AnsiConsole.Ask<string>("  [grey]Your plant's name:[/] ");

            if (string.IsNullOrWhiteSpace(playerName)) playerName = "Jardinier";
            if (string.IsNullOrWhiteSpace(plantName))  plantName  = "Piko";

            var save = new GameSave();
            save.Player.Name = playerName.Trim();
            save.Plant.Name  = plantName.Trim();
            save.Plant.BornAt = DateTime.UtcNow;

            // Initial weather
            var weatherService = new WeatherService();
            save.World.CurrentWeather = weatherService.Generate();

            _save.Save(save);
            _ui.ShowWelcome(save.Player.Name, save.Plant.Name);
            return save;
        }
    }
}

