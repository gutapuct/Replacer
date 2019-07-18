using ActsConsoleKirill.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActsConsoleKirill
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var reporter = new ConsoleReporter();
                var actMaker = new ActMaker(reporter);
                actMaker.Run();
                Console.WriteLine("Конец");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Исправьте ошибку и перезапустите программу!");
            }

            Console.ReadKey();
        }
    }
}
