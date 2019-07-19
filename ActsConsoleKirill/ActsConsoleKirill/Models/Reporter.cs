using ActsConsoleKirill.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActsConsoleKirill.Models
{
    public class ConsoleReporter : IReporter
    {
        public void Write(string message)
        {
            Console.Write(message + " ");
        }

        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }
    }
}
