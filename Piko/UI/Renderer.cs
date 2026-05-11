using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;
using Piko.Models;
using Piko.Models.Entities;
using Piko.Services;

namespace Piko.UI
{
    public class Renderer
    {
        // ── Main screen ───────────────────────────────────────────────────────

        public void Render(GameSave save, XpService xp, string? message = null)
        {
            AnsiConsole.Clear();
            var p  = save.Plant;
            var pl = save.Player;

            // Header rule
            string header = p.IsDead
                ? "[bold red] PIKO [/][grey]· Your plant has died[/]"
                : $"[bold green] PIKO [/][grey]· Day {p.AgeDays} · Lv.{pl.Level} · {Markup.Escape(pl.Name)}[/]";
            AnsiConsole.Write(new Rule(header).RuleStyle("green dim"));
            AnsiConsole.WriteLine();

            // ── Row 1: Plant art | Status ─────────────────────────────────────
            var r1 = RowTable(22);
            r1.AddRow(
                BentoPanel(AsciiArt.Get(p.Stage), "Plant"),
                BentoPanel(StatsContent(save, xp), "Status", expand: true));
            AnsiConsole.Write(r1);

            // ── Row 2: Progress | World ───────────────────────────────────────
            var r2 = RowTable(34);
            r2.AddRow(
                BentoPanel(ProgressContent(save, xp), "Progress"),
                BentoPanel(WorldContent(save), "World", expand: true));
            AnsiConsole.Write(r2);

            // ── Row 3: Actions ────────────────────────────────────────────────
            AnsiConsole.Write(BentoPanel(ActionsContent(save), "Actions", expand: true));

            // ── Message from last action ─────────────────────────────────────
            if (message != null)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"  [grey]>[/] {message}");
            }

            AnsiConsole.WriteLine();
        }

        // ── Panel factory ─────────────────────────────────────────────────────

        private static Panel BentoPanel(string content, string title, bool expand = false)
        {
            var panel = new Panel(content);
            panel.Header = new PanelHeader($" [grey]{Markup.Escape(title)}[/] ");
            panel.Border(BoxBorder.Rounded);
            panel.Padding(new Padding(1, 0, 1, 0));
            if (expand) panel.Expand();
            return panel;
        }

        private static Table RowTable(int col1Width)
        {
            var t = new Table();
            t.Border(TableBorder.None);
            t.ShowHeaders = false;
            t.AddColumn(new TableColumn("").Width(col1Width));
            t.AddColumn(new TableColumn(""));
            return t;
        }

        // ── Content builders ──────────────────────────────────────────────────

        private static string StatsContent(GameSave save, XpService xp)
        {
            var p  = save.Plant;
            var pl = save.Player;

            (string color, string label) = (p.Health, p.Happiness) switch
            {
                _ when p.IsDead              => ("red",     "Dead"),
                _ when p.Health >= 80
                    && p.Happiness >= 70     => ("green",   "Thriving"),
                _ when p.Health >= 50        => ("yellow",  "Okay"),
                _ when p.Health >= 30        => ("orange3", "Struggling"),
                _                            => ("red",     "Critical!")
            };

            var sb = new StringBuilder();
            sb.AppendLine($"[bold]{Markup.Escape(p.Name)}[/]  [grey]{p.StageName}[/]");
            sb.AppendLine($"[{color}]{label}[/]  [grey]Age: {p.AgeDays}d[/]");
            sb.AppendLine();
            sb.AppendLine($"[grey]Health    [/]{Bar(p.Health)}");
            sb.AppendLine($"[grey]Water     [/]{Bar(p.Water,     "dodgerblue1")}");
            sb.AppendLine($"[grey]Nutrients [/]{Bar(p.Nutrients, "yellow")}");
            sb.AppendLine($"[grey]Happiness [/]{Bar(p.Happiness, "deeppink3")}");
            sb.Append    ($"[grey]Growth    [/]{Bar(p.Growth,    "lime")}");
            return sb.ToString();
        }

        private static string ProgressContent(GameSave save, XpService xp)
        {
            var pl     = save.Player;
            bool maxed = pl.Level >= Constants.LevelThresholds.Length;
            long toNext = xp.XpToNext(save);
            double pct  = xp.LevelProgress(save);

            var sb = new StringBuilder();
            sb.AppendLine($"[grey]Level[/] [bold yellow]{pl.Level}[/]" +
                          (maxed ? " [grey](max)[/]" : $"  [grey dim]{toNext} XP to next[/]"));
            sb.AppendLine($"{XpBar(pct, 16)}  [grey]{pct:F0}%[/]");
            sb.AppendLine();
            sb.Append($"[grey]Sessions[/] {pl.TotalSessions}   [grey]Streak[/] {pl.ConsecutiveDays}d");
            return sb.ToString();
        }

        private static string WorldContent(GameSave save)
        {
            var w  = save.World.CurrentWeather;
            var ev = save.World.ActiveEvent?.IsActive == true ? save.World.ActiveEvent : null;

            var left     = w.ChangedAt.AddHours(w.DurationHours) - DateTime.UtcNow;
            string timer = left.TotalMinutes > 0 ? $" [grey dim]({left.TotalHours:F1}h)[/]" : "";

            var sb = new StringBuilder();
            sb.AppendLine($"[grey]Weather[/]  {w.DisplayName}{timer}");
            sb.AppendLine();
            if (ev != null)
            {
                sb.AppendLine($"[grey]Event[/]    {ev.DisplayName}");
                sb.Append($"[grey dim]{Markup.Escape(ev.Description)}[/]");
            }
            else
            {
                sb.Append("[grey]Event[/]    [grey dim]none[/]");
            }
            return sb.ToString();
        }

        private static string ActionsContent(GameSave save)
        {
            var p = save.Plant;

            string Btn(string key, string label, DateTime? last, double cd, bool cond = true)
            {
                double r = 0;
                bool ok = cond && !p.IsDead && CooldownOk(last, cd, out r);
                if (ok)                   return $"[white][[{key}]][/] [green]{label}[/]";
                if (p.IsDead || !cond)    return $"[grey][[{key}]] {label}[/]";
                return                           $"[grey][[{key}]] {label}[/] [grey dim]({r:F1}h)[/]";
            }

            var sb = new StringBuilder();
            sb.AppendLine(
                $"  {Btn("1", "Water    ", p.LastWatered, Constants.WaterCooldown)}    " +
                $"{Btn("2", "Fertilize", p.LastFed,     Constants.FertilizeCooldown)}    " +
                $"{Btn("3", "Talk     ", p.LastTalked,  Constants.TalkCooldown)}");
            sb.Append(
                $"  {Btn("4", "Sing     ", p.LastSung,   Constants.SingCooldown)}    " +
                $"{Btn("5", "Prune    ", p.LastPruned,  Constants.PruneCooldown, p.Stage >= 3)}    " +
                "[white][[6]][/] [grey]Achievements[/]    [white][[7]][/] [grey]Stats[/]    [white][[Q]][/] [grey]Quit[/]");
            return sb.ToString();
        }

        // ── Progress bars ─────────────────────────────────────────────────────

        private static string Bar(double pct, string? fill = null, int w = 10)
        {
            pct = Math.Clamp(pct, 0, 100);
            int filled = (int)(pct / 100.0 * w);
            string c = fill ?? (pct >= 60 ? "green" : pct >= 30 ? "yellow" : "red");
            return $"[{c}]{new string('█', filled)}[/][grey]{new string('░', w - filled)}[/] [bold]{pct,3:F0}%[/]";
        }

        private static string XpBar(double pct, int w = 10)
        {
            pct = Math.Clamp(pct, 0, 100);
            int filled = (int)(pct / 100.0 * w);
            return $"[gold1]{new string('█', filled)}[/][grey]{new string('░', w - filled)}[/]";
        }

        // ── Cooldown helper ───────────────────────────────────────────────────

        private static bool CooldownOk(DateTime? last, double hours, out double remaining)
        {
            remaining = 0;
            if (!last.HasValue) return true;
            double elapsed = (DateTime.UtcNow - last.Value).TotalHours;
            if (elapsed >= hours) return true;
            remaining = hours - elapsed;
            return false;
        }

        // ── Special screens ───────────────────────────────────────────────────

        public void ShowNewAchievements(List<Achievement> earned)
        {
            if (earned.Count == 0) return;
            var sb = new StringBuilder();
            foreach (var a in earned)
                sb.AppendLine(
                    $"  [yellow]{Markup.Escape(a.Icon)}[/] [bold]{Markup.Escape(a.Name)}[/]  " +
                    $"[grey]{Markup.Escape(a.Description)}[/]" +
                    (a.XpReward > 0 ? $"  [yellow]+{a.XpReward} XP[/]" : ""));
            AnsiConsole.WriteLine();
            AnsiConsole.Write(BentoPanel(sb.ToString().TrimEnd(), "Achievement Unlocked!", expand: true));
            Pause();
        }

        public void ShowLevelUp(int level)
        {
            AnsiConsole.MarkupLine($"\n  [bold yellow]Level {level} reached![/]");
        }

        public void ShowEvent(GameEvent? ev)
        {
            if (ev == null) return;
            AnsiConsole.Clear();
            string content =
                $"  {ev.DisplayName}\n\n" +
                $"  [grey]{Markup.Escape(ev.Description)}[/]\n\n" +
                $"  [grey dim]Duration: {ev.DurationHours}h[/]";
            AnsiConsole.Write(BentoPanel(content, "New Event!", expand: true));
            Pause();
        }

        public void ShowAllAchievements(GameSave save)
        {
            AnsiConsole.Clear();
            var sb = new StringBuilder();
            sb.AppendLine($"  [grey]{save.Player.Achievements.Count}/{AchievementService.All.Count} unlocked[/]\n");
            foreach (var a in AchievementService.All)
            {
                bool earned = save.Player.Achievements.Contains(a.Id);
                string line = earned
                    ? $"  [yellow]{Markup.Escape(a.Icon)}[/] [bold]{Markup.Escape(a.Name)}[/]  [grey]{Markup.Escape(a.Description)}[/]"
                    : $"  [grey dim]? ???  {Markup.Escape(a.Description)}[/]";
                sb.AppendLine(line);
            }
            AnsiConsole.Write(BentoPanel(sb.ToString().TrimEnd(), "Achievements", expand: true));
            Pause();
        }

        public void ShowStats(GameSave save)
        {
            AnsiConsole.Clear();
            var p = save.Plant; var pl = save.Player;
            var sb = new StringBuilder();
            sb.AppendLine($"  [grey]Player[/]    {Markup.Escape(pl.Name)}  Lv.{pl.Level}  ({pl.Xp} XP)");
            sb.AppendLine($"  [grey]Sessions[/]  {pl.TotalSessions}   [grey]Streak[/] {pl.ConsecutiveDays} day(s)");
            sb.AppendLine($"  [grey]Talks[/]     {pl.TalkCount}   [grey]Songs[/] {pl.SingCount}");
            sb.AppendLine();
            sb.AppendLine($"  [grey]Plant[/]     {Markup.Escape(p.Name)}  Stage {p.Stage} — {p.StageName}");
            sb.AppendLine($"  [grey]Age[/]       {p.AgeDays} day(s)");
            sb.Append    ($"  [grey]Born[/]      {p.BornAt.ToLocalTime():dd/MM/yyyy HH:mm}");
            AnsiConsole.Write(BentoPanel(sb.ToString(), "Statistics", expand: true));
            Pause();
        }

        public void ShowWelcome(string playerName, string plantName)
        {
            AnsiConsole.Clear();
            string content =
                $"  Welcome, [bold]{Markup.Escape(playerName)}[/]!\n\n" +
                $"  Your seed [bold]{Markup.Escape(plantName)}[/] is waiting for your care...\n\n" +
                "  [grey]Water it, fertilize it, talk and sing to it.[/]\n" +
                "  [grey]Watch it grow over 30 real days.[/]";
            AnsiConsole.Write(BentoPanel(content, "New Game", expand: true));
            Pause();
        }

        public void ShowDeath(GameSave save)
        {
            AnsiConsole.Clear();
            var p = save.Plant; var pl = save.Player;
            string content =
                $"{AsciiArt.Get(0)}\n\n" +
                $"  [grey]{Markup.Escape(p.Name)} lived for {p.AgeDays} day(s).[/]\n" +
                $"  [grey]Stage reached:[/]  {p.StageName}\n" +
                $"  [grey]XP earned:[/]      {pl.Xp}  (Lv.{pl.Level})";
            AnsiConsole.Write(BentoPanel(content, "Your plant died", expand: true));
        }

        public void ShowGoodbye(GameSave save)
        {
            AnsiConsole.MarkupLine(
                $"\n  [grey]See you soon, {Markup.Escape(save.Player.Name)}! " +
                $"{Markup.Escape(save.Plant.Name)} will miss you...[/]\n");
        }

        public static void Pause()
        {
            AnsiConsole.MarkupLine("\n  [grey dim]Press any key...[/]");
            Console.ReadKey(true);
        }
    }
}
