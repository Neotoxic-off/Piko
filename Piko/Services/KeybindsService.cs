using System;
using System.Collections.Generic;

namespace Piko.Services
{
    public class KeybindsService
    {
        private readonly Dictionary<ConsoleKey, Action> keybinds;

        public event Action OnExit = delegate { };
        public event Action OnHelp = delegate { };

        public KeybindsService()
        {
            keybinds = new Dictionary<ConsoleKey, Action>
            {
                { ConsoleKey.H, Help },
                { ConsoleKey.Q, Exit }
            };
        }

        public void Handle()
        {
            if (!Console.KeyAvailable)
                return;

            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            ConsoleKey key = keyInfo.Key;

            Action action;
            if (keybinds.TryGetValue(key, out action))
                action.Invoke();
        }

        private void Exit()
        {
            OnExit.Invoke();
        }

        private void Help()
        {
            OnHelp.Invoke();
        }
    }
}
