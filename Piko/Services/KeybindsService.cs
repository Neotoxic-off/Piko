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
            ConsoleKeyInfo keyInfo;
            ConsoleKey key;
            Action action;

            if (Console.KeyAvailable)
            {
                keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (keybinds.TryGetValue(key, out action))
                    action.Invoke();                
            }

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
