using System;
using System.Threading.Tasks;

namespace Piko
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            await new Core().Run();
        }
    }
}
