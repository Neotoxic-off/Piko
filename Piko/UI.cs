using Spectre.Console;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace Piko
{
    public class UI
    {
        private bool isHelpDisplayed = false;
        private Layout layout;
        private LiveDisplayContext ctx;

        public UI()
        {
            layout = new Layout("Root");
            Setup();
        }

        public void Start(CancellationToken cancellationToken)
        {
            AnsiConsole.Live(layout)
                .Start(context =>
                {
                    ctx = context;
                    ctx.Refresh();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Thread.Sleep(100);
                    }
                });
        }

        public void ShowHelp()
        {
            if (isHelpDisplayed == false)
            {
                layout["Right"].Update(
                    new Panel(
                        @"[yellow]Commands:[/]
    [[H]] Help - Show this help menu
    [[Q]] Quit - Exit the program"
                    ).Header("Help")
                );
            }
            else
            {
                layout["Right"].Update(
                    new Panel(
                        @"PlaceHolder"
                    ).Header("Help")
                );
            }

            if (ctx != null)
            {
                ctx.Refresh();
            }

            isHelpDisplayed = !isHelpDisplayed;
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
        }
    }
}