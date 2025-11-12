using Spectre.Console;
using System.Numerics;
using System.Threading.Tasks;

namespace Piko
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Core core = new Core();
            await core.Run();
        }
    }
}
