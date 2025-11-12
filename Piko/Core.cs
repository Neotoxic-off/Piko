using Piko.Services;
using System;
using System.Threading.Tasks;

namespace Piko
{
    public class Core
    {
        private UI ui;
        private KeybindsService keybindsService;
        private bool running;

        public Core()
        {
            ui = new UI();
            keybindsService = new KeybindsService();
            running = true;

            keybindsService.OnExit += OnExitHandler;
            keybindsService.OnHelp += OnHelpHandler;
        }

        public async Task Run()
        {
            await ui.StartAsync();

            while (running)
            {
                keybindsService.Handle();
                await Task.Delay(50);
            }
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
