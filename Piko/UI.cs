using Spectre.Console;
using System.Threading.Tasks;

namespace Piko
{
    public class UI
    {
        private Layout layout;
        private bool isRunning;

        public UI()
        {
            layout = new Layout("Root");
            Setup();
            isRunning = true;
        }

        public async Task StartAsync()
        {
            await AnsiConsole.Live(layout)
                .StartAsync(async ctx =>
                {
                    while (isRunning)
                    {
                        ctx.Refresh();
                        await Task.Delay(100);
                    }
                });
        }

        public void ShowHelp()
        {
            layout["Right"].Update(
                new Panel(
                    "[yellow]Commands:[/]\n" +
                    "[H] Help - Show this help menu\n" +
                    "[Q] Quit - Exit the program"
                ).Header("Help")
            );
        }

        private void Setup()
        {
            layout.SplitRows(
                new Layout("Header").Size(3),
                new Layout("Body").SplitColumns(
                    new Layout("Left").Size(30),
                    new Layout("Right")),
                new Layout("Footer").Size(3)
            );

            layout["Header"].Update(new Panel("[bold yellow]Piko Console[/]"));
            layout["Left"].Update(new Panel("[green]Waiting for input...[/]"));
            layout["Right"].Update(new Panel("[grey]Press H for help[/]"));
            layout["Footer"].Update(new Panel("[grey]Press Q to quit[/]"));
        }
    }
}
