using System;

namespace Piko.UI
{
    public static class AsciiArt
    {
        public static string Get(int stage) => stage switch
        {
            0 => Stage0,
            1 => Stage1,
            2 => Stage2,
            3 => Stage3,
            4 => Stage4,
            5 => Stage5,
            _ => Stage6
        };

        // Stage 0 – Graine (seed in soil)
        private const string Stage0 =
            "               \n" +
            "               \n" +
            "               \n" +
            "    [tan]. . .[/]        \n" +
            "   [tan]([/][#c8a06e] o [/][tan])[/]        \n" +
            "   [tan]~~~~~[/]        \n" +
            "[green] ~~~~~~~~~~~ [/]";

        // Stage 1 – Pousse (first sprout)
        private const string Stage1 =
            "               \n" +
            "               \n" +
            "      [lime]|[/]          \n" +
            "     [lime]\\|/[/]         \n" +
            "      [green]|[/]          \n" +
            "      [green]|[/]          \n" +
            "[green] ~~~~~~~~~~~ [/]";

        // Stage 2 – Plantule (seedling with first leaves)
        private const string Stage2 =
            "               \n" +
            "  [lime]v[/]         [lime]v[/]  \n" +
            "   [lime]\\[/]       [lime]/[/]   \n" +
            "    [lime]\\[/]     [lime]/[/]    \n" +
            "     [green]\\[/]   [green]/[/]     \n" +
            "      [green]|[/]          \n" +
            "[green] ~~~~~~~~~~~ [/]";

        // Stage 3 – Jeune plante (young plant, multiple leaves)
        private const string Stage3 =
            "  [lime]v[/] [lime](   )[/] [lime]v[/]  \n" +
            "   [lime])(   )([/]    \n" +
            "  [lime](     )[/]     \n" +
            "    [green]\\   /[/]      \n" +
            "     [green]|||[/]       \n" +
            "     [green]|||[/]       \n" +
            "[green] ~~~~~~~~~~~ [/]";

        // Stage 4 – Plante mature (full bushy plant)
        private const string Stage4 =
            " [lime]v[/][lime](  )[/][lime]v[/][lime](  )[/][lime]v[/] \n" +
            "  [lime])(    )(   [/]  \n" +
            " [lime]v[/][lime](  )[/][lime]v[/][lime](  )[/][lime]v[/] \n" +
            "   [green]\\[/] [green]||||[/] [green]/[/]   \n" +
            "    [green]|||||[/]      \n" +
            "    [green]|||||[/]      \n" +
            "[green] ~~~~~~~~~~~ [/]";

        // Stage 5 – En fleurs (flowering)
        private const string Stage5 =
            " [deeppink3]*[/] [lime](  )[/] [deeppink3]*[/]  \n" +
            "[deeppink3]*[/] [lime](    )[/] [deeppink3]*[/]  \n" +
            " [deeppink3]*[/] [lime](  )[/] [deeppink3]*[/]  \n" +
            "  [deeppink3]*[/][green]\\||||/[/][deeppink3]*[/]  \n" +
            "    [green]|||||[/]       \n" +
            "    [green]|||||[/]       \n" +
            "[green] ~~~~~~~~~~~ [/]";

        // Stage 6 – Pleine floraison (full bloom)
        private const string Stage6 =
            "[deeppink1]*[/][lime]([/][gold1]@[/][lime]  )[/][deeppink1]*[/][lime]([/][gold1]@[/][lime])[/][deeppink1]*[/]\n" +
            "[lime]([/][gold1]@[/][lime] )[/][deeppink1]*[/][lime]([/][gold1]@@[/][lime])[/][deeppink1]*[/][lime]( [/][gold1]@[/][lime])[/]\n" +
            "[deeppink1]*[/][lime]([/][gold1]@[/][lime]  )[/][deeppink1]*[/][lime]([/][gold1]@[/][lime])[/][deeppink1]*[/]\n" +
            "  [deeppink1]*[/]  [green]\\|/[/]  [deeppink1]*[/]   \n" +
            "  [deeppink1]*[/][green]\\||||||/[/][deeppink1]*[/]  \n" +
            "    [green]||||||[/]       \n" +
            "[green] ~~~~~~~~~~~ [/]";
    }
}
