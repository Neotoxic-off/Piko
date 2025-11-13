using Piko.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Piko
{
    public class Core
    {
        private UI ui;
        private KeybindsService keybindsService;
        private bool running;
        private CancellationTokenSource cts;

        public Core()
        {
            ui = new UI();
            keybindsService = new KeybindsService();
            running = true;
            cts = new CancellationTokenSource();

            keybindsService.OnExit += OnExitHandler;
            keybindsService.OnHelp += OnHelpHandler;
        }

        public async Task Run()
        {
            // Lancer l'UI dans un thread séparé avec le CancellationToken
            var uiTask = Task.Run(() => ui.Start(cts.Token));

            // Boucle de gestion des touches dans le thread principal
            while (running)
            {
                keybindsService.Handle();
                await Task.Delay(50);
            }

            cts.Cancel(); // Annuler l'UI quand on quitte
            await uiTask; // Attendre que l'UI se termine proprement
        }

        private void OnExitHandler()
        {
            running = false;
        }

        private void OnHelpHandler()
        {
            ui.ShowHelp();
        }
    }
}